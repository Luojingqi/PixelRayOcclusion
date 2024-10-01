using Data;
using UnityEngine;

public class BlockMaterial
{

    /// <summary>
    /// 相机中心所在的块坐标
    /// </summary>
    public Vector2Int CameraCenterBlockPos;
    /// <summary>
    /// 光缓存的大小，(宽与高，最小为1,1)，只有在此范围内的才会被渲染线程提交给cpu
    /// </summary>
    public static Vector2Int LightBufferBlockSize = new Vector2Int(3, 3);
    /// <summary>
    /// 每个区块接收多大范围内的光照,最小1,1,更改后需要去Shader中同步更改Block缓冲区数量
    /// </summary>
    public static Vector2Int EachBlockReceiveLightSize = new Vector2Int(5, 5);
    public static int BlockBufferLength = (EachBlockReceiveLightSize.x - 1 + LightBufferBlockSize.x) * (EachBlockReceiveLightSize.y - 1 + LightBufferBlockSize.y);
    public static int LightBufferLength = LightBufferBlockSize.x * LightBufferBlockSize.y;

    #region 公共材质
    /// <summary>
    /// block的公共材质
    /// </summary>
    public Material ShareMaterial_block { get { return shareMaterial_block; } }
    private Material shareMaterial_block;
    /// <summary>
    /// background的公共材质
    /// </summary>
    public Material ShareMaterial_background { get { return shareMaterial_background; } }
    private Material shareMaterial_background;
    /// <summary>
    /// 用于更新光照缓存的计算着色器
    /// </summary>
    private ComputeShader[] computeLightBuffer = new ComputeShader[LightBufferLength];
    private int[] kernelHandle = new int[LightBufferLength];
    #endregion

    public void Init()
    {
        LoadAllMaterial();
        LoadAllPixelColorInfo();

        #region 缓存数组初始化
        for (int i = 0; i < BlockBufferLength; i++)
        {
            blockBuffer[i] = new ComputeBuffer(Block.Size.x * Block.Size.y, sizeof(int));
        }
        for (int i = 0; i < LightBufferLength; i++)
        {
            backgroundBuffer[i] = new ComputeBuffer(Block.Size.x * Block.Size.y, sizeof(int));
            lightBuffer[i] = new ComputeBuffer(Block.Size.x * Block.Size.y, sizeof(float) * 4);
        }

        unsafe { sunBuffer = new ComputeBuffer(sunArray.Length, sizeof(Sun)); }
        #endregion
    }
    private void LoadAllMaterial()
    {
        shareMaterial_block = Resources.Load<Material>("PixelRayOcclusion/PRO_Block");
        shareMaterial_background = Resources.Load<Material>("PixelRayOcclusion/PRO_Background");
        computeLightBuffer[0] = Resources.Load<ComputeShader>("PixelRayOcclusion/PRO_SetLightBuffer");
        kernelHandle[0] = computeLightBuffer[0].FindKernel("CSMain");
        for (int i = 1; i < LightBufferLength; i++)
        {
            computeLightBuffer[i] = GameObject.Instantiate<ComputeShader>(computeLightBuffer[0]);
            kernelHandle[i] = computeLightBuffer[i].FindKernel("CSMain");
        }
    }
    /// <summary>
    /// 绑定缓存到shader
    /// </summary>
    public void BindBufferToShader()
    {
        shareMaterial_block.SetBuffer("AllPixelColorInfo", allPixelColorInfoBufffer);
        shareMaterial_background.SetBuffer("AllPixelColorInfo", allPixelColorInfoBufffer);
        for (int i = 0; i < LightBufferLength; i++)
        {
            computeLightBuffer[i].SetBuffer(kernelHandle[i], "AllPixelColorInfo", allPixelColorInfoBufffer);
            computeLightBuffer[i].SetBuffer(kernelHandle[i], "SunBuffer", sunBuffer);
            computeLightBuffer[i].SetInts("EachBlockReceiveLightSize", new int[] { EachBlockReceiveLightSize.x, EachBlockReceiveLightSize.y });
        }
        Vector2Int nowCameraPos = Block.WorldToBlock(Camera.main.transform.position);
        CameraCenterBlockPos = nowCameraPos;
        Vector2Int minLightBufferBlockPos = CameraCenterBlockPos - LightBufferBlockSize / 2;
        Vector2Int minBlockBufferPos = minLightBufferBlockPos - EachBlockReceiveLightSize / 2;

        for (int y = 0; y < LightBufferBlockSize.y; y++)
            for (int x = 0; x < LightBufferBlockSize.x; x++)
            {
                Vector2Int gloabBlockPos = minLightBufferBlockPos + new Vector2Int(x, y);
                Vector2Int localBlockBufferPos = gloabBlockPos - minBlockBufferPos;
                int blockIndex = localBlockBufferPos.x + localBlockBufferPos.y * (EachBlockReceiveLightSize.x - 1 + LightBufferBlockSize.x);
                int lightIndex = x + y * LightBufferBlockSize.x;
                Block block = BlockManager.Inst.BlockCrossList[gloabBlockPos];
                BackgroundBlock background = BlockManager.Inst.BackgroundCrossList[gloabBlockPos];


                block.materialPropertyBlock.SetBuffer("BlockBuffer", blockBuffer[blockIndex]);
                block.materialPropertyBlock.SetBuffer("LightBuffer", lightBuffer[lightIndex]);
                background.materialPropertyBlock.SetBuffer("BackgroundBuffer", backgroundBuffer[lightIndex]);
                background.materialPropertyBlock.SetBuffer("LightBuffer", lightBuffer[lightIndex]);
                block.spriteRenderer.SetPropertyBlock(block.materialPropertyBlock);
                background.spriteRenderer.SetPropertyBlock(background.materialPropertyBlock);


                for (int ey = 0; ey < EachBlockReceiveLightSize.y; ey++)
                    for (int ex = 0; ex < EachBlockReceiveLightSize.x; ex++)
                    {
                        int index = ex + ey * EachBlockReceiveLightSize.x;
                        Vector2Int nowLocalBlockBufferPos = localBlockBufferPos - EachBlockReceiveLightSize / 2 + new Vector2Int(ex, ey);
                        int eBlcokIndex = nowLocalBlockBufferPos.x + nowLocalBlockBufferPos.y * (EachBlockReceiveLightSize.x - 1 + LightBufferBlockSize.x);
                        computeLightBuffer[lightIndex].SetBuffer(kernelHandle[lightIndex], $"BlockBuffer{index}", blockBuffer[eBlcokIndex]);
                    }
                computeLightBuffer[lightIndex].SetBuffer(kernelHandle[lightIndex], $"LightBuffer", lightBuffer[lightIndex]);
                computeLightBuffer[lightIndex].SetInts("BlockPos", new int[] { gloabBlockPos.x, gloabBlockPos.y });


                SetBlock(block);
                SetBackgroundBlock(background);
            }
        for (int i = 0; i < LightBufferLength; i++)
            computeLightBuffer[i].Dispatch(kernelHandle[i], Block.Size.x / 10, Block.Size.y / 10, 1);
    }

    #region 点的颜色属性的加载与缓冲区
    //所有点的颜色数据
    private PixelColorInfo[] allPixelColorInfo;
    //所有点的颜色数据
    public PixelColorInfoToShader[] allPixelColorInfoToShader;
    public ComputeBuffer allPixelColorInfoBufffer;
    private void LoadAllPixelColorInfo()
    {
        JsonTool.LoadingText(Application.streamingAssetsPath + @"\Json\AllPixelColorInfo.json", out string infoText);
        allPixelColorInfo = JsonTool.ToObject<PixelColorInfo[]>(infoText);
        allPixelColorInfoToShader = new PixelColorInfoToShader[allPixelColorInfo.Length];
        for (int i = 0; i < allPixelColorInfo.Length; i++)
        {
            var info = allPixelColorInfo[i];
            var infoToShader = new PixelColorInfoToShader();
            infoToShader.color = info.color;
            infoToShader.shininess = info.shininess;

            allPixelColorInfoToShader[i] = infoToShader;
        }
        unsafe
        {
            allPixelColorInfoBufffer = new ComputeBuffer(allPixelColorInfo.Length, sizeof(PixelColorInfoToShader));
            allPixelColorInfoBufffer.SetData(allPixelColorInfoToShader);
        }
    }
    public PixelColorInfo GetPixelColorInfo(int id) => allPixelColorInfo[id];
    #endregion

    #region 块缓存&光照缓存  设置缓冲区数据
    private ComputeBuffer[] blockBuffer = new ComputeBuffer[BlockBufferLength];
    private ComputeBuffer[] backgroundBuffer = new ComputeBuffer[LightBufferLength];
    private ComputeBuffer[] lightBuffer = new ComputeBuffer[LightBufferLength];
    public void SetBlock(Block block)
    {
        Vector2Int minLightBufferBlockPos = CameraCenterBlockPos - LightBufferBlockSize / 2;
        Vector2Int minBlockBufferPos = minLightBufferBlockPos - EachBlockReceiveLightSize / 2;
        Vector2Int maxBlockBufferPos = minBlockBufferPos + LightBufferBlockSize - new Vector2Int(1, 1) + EachBlockReceiveLightSize - new Vector2Int(1, 1);

        if (block.BlockPos.x < minBlockBufferPos.x || block.BlockPos.x > maxBlockBufferPos.x
            && block.BlockPos.y < minBlockBufferPos.y || block.BlockPos.y > maxBlockBufferPos.y)
            return;

        Vector2Int localBlockBufferPos = block.BlockPos - minBlockBufferPos;
        int index = localBlockBufferPos.x + localBlockBufferPos.y * (EachBlockReceiveLightSize.x - 1 + LightBufferBlockSize.x);
        if (index < 0 || index >= BlockBufferLength) return;
        blockBuffer[index].SetData(block.textureData.PixelIDToShader);
    }
    public void SetBackgroundBlock(BackgroundBlock background)
    {
        Vector2Int minLightBufferBlockPos = CameraCenterBlockPos - LightBufferBlockSize / 2;
        Vector2Int maxLightBufferBlockPos = minLightBufferBlockPos + LightBufferBlockSize - new Vector2Int(1, 1);

        if (background.BlockPos.x < minLightBufferBlockPos.x || background.BlockPos.x > maxLightBufferBlockPos.x
            && background.BlockPos.y < minLightBufferBlockPos.y || background.BlockPos.y > maxLightBufferBlockPos.y)
            return;

        Vector2Int blockPos = background.BlockPos - minLightBufferBlockPos;
        int index = blockPos.y * LightBufferBlockSize.x + blockPos.x;

        if (index < 0 || index >= LightBufferLength) return;

        backgroundBuffer[index].SetData(background.textureData.PixelIDToShader);
    }
    #endregion

    #region 太阳相关
    public struct Sun
    {
        public Vector2Int gloabPos;
        public int r;
        public Vector4 color;

        public Sun(Vector2Int gloabPos, int r, Vector4 color)
        {
            this.gloabPos = gloabPos;
            this.r = r;
            this.color = color;
        }
    };
    private ComputeBuffer sunBuffer;
    private Sun[] sunArray = new Sun[10];
    public void SetSun(Sun sun, int i)
    {
        sunArray[i] = sun;
        sunBuffer.SetData(sunArray);
    }
    #endregion


    public void Update()
    {
        Vector2Int nowCameraPos = Block.WorldToBlock(Camera.main.transform.position);
        if (nowCameraPos != CameraCenterBlockPos)
        {
            CameraCenterBlockPos = nowCameraPos;
            Vector2Int minLightBufferBlockPos = CameraCenterBlockPos - LightBufferBlockSize / 2;
            Vector2Int minBlockBufferPos = minLightBufferBlockPos - EachBlockReceiveLightSize / 2;

            for (int y = 0; y < LightBufferBlockSize.y; y++)
                for (int x = 0; x < LightBufferBlockSize.x; x++)
                {
                    Vector2Int gloabBlockPos = minLightBufferBlockPos + new Vector2Int(x, y);
                    Vector2Int localBlockBufferPos = gloabBlockPos - minBlockBufferPos;
                    int blockIndex = localBlockBufferPos.x + localBlockBufferPos.y * (EachBlockReceiveLightSize.x - 1 + LightBufferBlockSize.x);
                    int lightIndex = x + y * LightBufferBlockSize.x;
                    Block block = BlockManager.Inst.BlockCrossList[gloabBlockPos];
                    BackgroundBlock background = BlockManager.Inst.BackgroundCrossList[gloabBlockPos];


                    block.materialPropertyBlock.SetBuffer("BlockBuffer", blockBuffer[blockIndex]);
                    block.materialPropertyBlock.SetBuffer("LightBuffer", lightBuffer[lightIndex]);
                    background.materialPropertyBlock.SetBuffer("BackgroundBuffer", backgroundBuffer[lightIndex]);
                    background.materialPropertyBlock.SetBuffer("LightBuffer", lightBuffer[lightIndex]);
                    block.spriteRenderer.SetPropertyBlock(block.materialPropertyBlock);
                    background.spriteRenderer.SetPropertyBlock(background.materialPropertyBlock);


                    for (int ey = 0; ey < EachBlockReceiveLightSize.y; ey++)
                        for (int ex = 0; ex < EachBlockReceiveLightSize.x; ex++)
                        {
                            int index = ex + ey * EachBlockReceiveLightSize.x;
                            Vector2Int nowLocalBlockBufferPos = localBlockBufferPos - EachBlockReceiveLightSize / 2 + new Vector2Int(ex, ey);
                            int eBlcokIndex = nowLocalBlockBufferPos.x + nowLocalBlockBufferPos.y * (EachBlockReceiveLightSize.x - 1 + LightBufferBlockSize.x);
                            computeLightBuffer[lightIndex].SetBuffer(kernelHandle[lightIndex], $"BlockBuffer{index}", blockBuffer[eBlcokIndex]);
                        }
                    computeLightBuffer[lightIndex].SetInts("BlockPos", new int[] { gloabBlockPos.x, gloabBlockPos.y });

                    SetBlock(block);
                    SetBackgroundBlock(background);
                }
        }
        for (int i = 0; i < LightBufferLength; i++)
            computeLightBuffer[i].Dispatch(kernelHandle[i], Block.Size.x / 10, Block.Size.y / 10, 1);
    }


    public void Dispose()
    {
        allPixelColorInfoBufffer.Dispose();
        allPixelColorInfoBufffer = null;
        for (int i = 0; i < blockBuffer.Length; i++)
        {
            blockBuffer[i].Dispose();
            blockBuffer[i] = null;
        }
        sunBuffer.Dispose();
        sunBuffer = null;
    }
}
