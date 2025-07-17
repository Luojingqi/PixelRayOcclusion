using PRO.Disk;
using PRO.Renderer;
using PRO.Tool;
using PRO.Tool.Serialize.IO;
using PRO.Tool.Serialize.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
namespace PRO
{
    public static class BlockMaterial
    {
        private static Vector2Int _CameraCenterBlockPos;
        /// <summary>
        /// 相机中心所在的块坐标
        /// </summary>
        public static Vector2Int CameraCenterBlockPos
        {
            get { return _CameraCenterBlockPos; }
            private set
            {
                LastCameraCenterBlockPos = _CameraCenterBlockPos;
                _CameraCenterBlockPos = value;
                MinLightBufferBlockPos = CameraCenterBlockPos - LightResultBufferBlockSize / 2;
                MaxLightBufferBlockPos = MinLightBufferBlockPos + LightResultBufferBlockSize;
                MinBlockBufferPos = MinLightBufferBlockPos - EachBlockReceiveLightSize / 2;
                MaxBlockBufferPos = MinBlockBufferPos + LightResultBufferBlockSize - new Vector2Int(1, 1) + EachBlockReceiveLightSize - new Vector2Int(1, 1);
            }
        }
        public static Vector2Int MinLightBufferBlockPos { get; private set; }
        public static Vector2Int MaxLightBufferBlockPos { get; private set; }
        public static bool IsInLightBufferBlock(Vector2Int blockPos) => !(blockPos.x > MaxLightBufferBlockPos.x || blockPos.y > MaxLightBufferBlockPos.y || blockPos.x < MinLightBufferBlockPos.x || blockPos.y < MinLightBufferBlockPos.y);
        public static Vector2Int MinBlockBufferPos { get; private set; }
        public static Vector2Int MaxBlockBufferPos { get; private set; }
        /// <summary>
        /// 上一帧相机中心所在的块坐标
        /// </summary>
        public static Vector2Int LastCameraCenterBlockPos { get; private set; }
        /// <summary>
        /// 最大光源半径，修改完后需要调用编辑器扩展PRO>2.创建hlsl
        /// 将生成LightRadiusMax个计算着色器函数  每个代表一个不同半径的光源函数
        /// </summary>
        public static readonly int LightRadiusMax = 128;
        /// <summary>
        /// 光缓存的大小，(宽与高，最小为1,1)，只有在此范围内的才会被渲染线程提交给gpu
        /// 从相机所在的区块开始算起，超出的部分不会显示光照效果
        /// 更改后需要 调用编辑器扩展PRO>2.创建hlsl
        /// </summary>
        public static readonly Vector2Int LightResultBufferBlockSize = new Vector2Int(7, 5);
        /// <summary>
        /// 每个区块接收多大范围内的光照,最小1,1
        /// Buffer_LightBuffer.hlsl 中StructuredBuffer<int> BlockBuffer0;
        /// 更改后需要 调用编辑器扩展PRO>2.创建hlsl
        /// </summary>
        public static readonly Vector2Int EachBlockReceiveLightSize = new Vector2Int(5, 5);
        /// <summary>
        /// 每帧更新的区块数量
        /// </summary>
        public static int FrameUpdateBlockNum;

        public static int BlockBufferLength { get; private set; }
        public static int LightResultBufferLength { get; private set; }
        public static PROconfig proConfig;
        /// <summary>
        /// 为空的材质区块
        /// </summary>
        public static MaterialPropertyBlock NullMaterialPropertyBlock;
        #region 公共材质

        public static BlockShareMaterialManager blockShareMaterialManager = new BlockShareMaterialManager();
        public static BackgroundShareMaterialManager backgroundShareMaterialManager = new BackgroundShareMaterialManager();
        public static ComputeShaderManager computeShaderManager = new ComputeShaderManager();

        #endregion

        public static void Init()
        {
            CameraCenterBlockPos = Block.WorldToBlock(Camera.main.transform.position);
            if (!IOTool.LoadText(Application.streamingAssetsPath + @"\Json\PROconfig.json", out string proConfigText))
            {
                Debug.Log("PROconfig.json加载失败，BlockMaterial无法初始化");
                return;
            }
            proConfig = JsonTool.ToObject<PROconfig>(proConfigText);
            FrameUpdateBlockNum = proConfig.FrameUpdateBlockNum;
            BlockBufferLength = (EachBlockReceiveLightSize.x - 1 + LightResultBufferBlockSize.x) * (EachBlockReceiveLightSize.y - 1 + LightResultBufferBlockSize.y);
            LightResultBufferLength = LightResultBufferBlockSize.x * LightResultBufferBlockSize.y;

            LoadAllPixelColorInfo();

            NullMaterialPropertyBlock = new MaterialPropertyBlock();

            blockShareMaterialManager.Init();
            backgroundShareMaterialManager.Init();
            computeShaderManager.Init();

            new Thread(() => DrawThread.LoopDraw()).Start();
        }

        #region 像素点的颜色属性的加载与缓冲区
        //所有点的颜色数据_存储到数组，然后传递给GPU缓冲区，shader与计算着色器中使用
        public static PixelColorInfoToShader[] pixelColorInfoToShaderArray;
        public static ComputeBuffer pixelColorInfoToShaderBufffer;
        private static void LoadAllPixelColorInfo()
        {
            #region 加载路径下的所有PixelColorInfo.json文件，并存储到PixelColorInfoDic与List
            string rootPath = AssetManager.ExcelToolSaveJsonPath;
            DirectoryInfo root = new DirectoryInfo(rootPath);
            int pixelCount = 0;
            foreach (var fileInfo in root.GetFiles())
            {
                if (fileInfo.Extension != ".json") continue;
                string[] strArray = fileInfo.Name.Split('^');
                if (strArray.Length <= 1 || strArray[0] != "PixelColorInfo") continue;
                IOTool.LoadText(fileInfo.FullName, out string infoText);
                Log.Print(fileInfo.FullName, Color.green);
                //加载到的像素数组
                var InfoArray = JsonTool.ToObject<PixelColorInfo[]>(infoText);
                for (int i = 0; i < InfoArray.Length; i++)
                    if (pixelColorInfoDic.ContainsKey(InfoArray[i].colorName) == false)
                    {
                        InfoArray[i].index = pixelCount++;
                        pixelColorInfoDic.Add(InfoArray[i].colorName, InfoArray[i]);
                        pixelColorInfoList.Add(InfoArray[i]);
                    }
            }
            #endregion

            #region 将allPixelColorInfo转换为allPixelColorInfoToShader并提交到GPU
            pixelColorInfoToShaderArray = new PixelColorInfoToShader[pixelCount + 10];//加几个备用
            for (int i = 0; i < pixelColorInfoList.Count; i++)
            {
                var infoToShader = new PixelColorInfoToShader(pixelColorInfoList[i]);
                pixelColorInfoToShaderArray[i] = infoToShader;
            }
            unsafe
            {
                pixelColorInfoToShaderBufffer = new ComputeBuffer(pixelColorInfoToShaderArray.Length, sizeof(PixelColorInfoToShader));
                pixelColorInfoToShaderBufffer.SetData(pixelColorInfoToShaderArray);
            }
            #endregion
        }
        //所有点的颜色数据_顺序索引
        private static List<PixelColorInfo> pixelColorInfoList = new List<PixelColorInfo>();
        //点颜色数据_字典索引
        private static Dictionary<string, PixelColorInfo> pixelColorInfoDic = new Dictionary<string, PixelColorInfo>();
        public static PixelColorInfo GetPixelColorInfo(string colorName)
        {
            if (pixelColorInfoDic.TryGetValue(colorName, out PixelColorInfo value)) return value;
            else Debug.Log($"没有像素颜色名称为{colorName}");
            return null;
        }
        public static PixelColorInfo GetPixelColorInfo(int id)
        {
            if (id >= pixelColorInfoList.Count) return null;
            else return pixelColorInfoList[id];
        }
        #endregion

        #region 将块数据传递到GPU
        public static void SetBlock(Block block)
        {
            Vector2Int minLightBufferBlockPos = CameraCenterBlockPos - LightResultBufferBlockSize / 2;
            Vector2Int minBlockBufferPos = minLightBufferBlockPos - EachBlockReceiveLightSize / 2;
            Vector2Int maxBlockBufferPos = minBlockBufferPos + LightResultBufferBlockSize - new Vector2Int(1, 1) + EachBlockReceiveLightSize - new Vector2Int(1, 1);

            if (block.BlockPos.x < minBlockBufferPos.x || block.BlockPos.x > maxBlockBufferPos.x
                && block.BlockPos.y < minBlockBufferPos.y || block.BlockPos.y > maxBlockBufferPos.y)
                return;

            Vector2Int localBlockBufferPos = block.BlockPos - minBlockBufferPos;
            int index = localBlockBufferPos.x + localBlockBufferPos.y * (EachBlockReceiveLightSize.x - 1 + LightResultBufferBlockSize.x);
            if (index < 0 || index >= BlockBufferLength) return;

            blockShareMaterialManager.SetBufferData(index, block.textureData.PixelInfoToShaderArray);
            //Debug.Log($"块坐标{block.BlockPos}  传递块数据  块缓存索引{index}");
        }
        public static void SetBackgroundBlock(BackgroundBlock background)
        {
            Vector2Int minLightBufferBlockPos = CameraCenterBlockPos - LightResultBufferBlockSize / 2;
            Vector2Int maxLightBufferBlockPos = minLightBufferBlockPos + LightResultBufferBlockSize - new Vector2Int(1, 1);

            if (background.BlockPos.x < minLightBufferBlockPos.x || background.BlockPos.x > maxLightBufferBlockPos.x
                && background.BlockPos.y < minLightBufferBlockPos.y || background.BlockPos.y > maxLightBufferBlockPos.y)
                return;

            Vector2Int blockPos = background.BlockPos - minLightBufferBlockPos;
            int index = blockPos.y * LightResultBufferBlockSize.x + blockPos.x;

            if (index < 0 || index >= LightResultBufferLength) return;

            backgroundShareMaterialManager.SetBufferData(index, background.textureData.PixelInfoToShaderArray);
            //Debug.Log($"背景坐标{background.BlockPos}  传递背景数据   背景缓存索引{index}");
        }
        #endregion

        public static void UpdateBind()
        {
            blockShareMaterialManager.ClearLastBind();
            backgroundShareMaterialManager.ClearLastBind();

            blockShareMaterialManager.UpdateBind();
            backgroundShareMaterialManager.UpdateBind();
            computeShaderManager.UpdateBind();
        }
        /// <summary>
        /// 更新相机位置，加载更新位置后的区块，延长视野内区块的卸载倒计时
        /// </summary>
        public static void Update()
        {
            CameraCenterBlockPos = Block.WorldToBlock(Camera.main.transform.position);
            for (int y = MinBlockBufferPos.y; y <= MaxBlockBufferPos.y; y++)
                for (int x = MinBlockBufferPos.x; x <= MaxBlockBufferPos.x; x++)
                {
                    var scene = SceneManager.Inst.NowScene;
                    if (scene == null) return;
                    if (scene.ActiveBlockBase.Contains(new Vector2Int(x, y)) == false)
                        scene.ThreadLoadOrCreateBlock(new Vector2Int(x, y));
                    else
                        scene.GetBlock(new Vector2Int(x, y)).ResetUnLoadCountdown();
                }
        }

        public static void LastUpdate()
        {
            if (Monitor.TryEnter(DrawApplyQueue))
            {
                try
                {
                    while (DrawApplyQueue.Count > 0)
                    {
                        BlockBase blockBase = DrawApplyQueue.Dequeue();
                        //blockBase.spriteRenderer.sprite.texture.Apply();
                        switch (blockBase)
                        {
                            case Block block: SetBlock(block); break;
                            case BackgroundBlock background: SetBackgroundBlock(background); break;
                        }
                    }
                }
                finally { Monitor.Exit(DrawApplyQueue); }
            }

            if (CameraCenterBlockPos != LastCameraCenterBlockPos)
            {
                UpdateBind();
                return;
            }
            computeShaderManager.Update();
        }

        /// <summary>
        /// 纹理绘制完成需要提交的队列，渲染线程添加，主线程取出（纹理只是提供一种选择，但最终显示光照遮挡效果不使用纹理贴图）
        /// </summary>
        public static readonly Queue<BlockBase> DrawApplyQueue = new Queue<BlockBase>();
        /// <summary>
        /// 加入&锁住 提交绘制引用队列(新数据提交到显卡)
        /// </summary>
        public static void En_Lock_DrawApplyQueue(BlockBase block)
        {
            lock (DrawApplyQueue)
                if (DrawApplyQueue.Contains(block) == false)
                    DrawApplyQueue.Enqueue(block);
        }


        public static void Dispose()
        {
        }
    }
}