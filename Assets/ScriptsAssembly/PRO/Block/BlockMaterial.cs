using PRO.Data;
using PRO.Renderer;
using PRO.Tool;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
namespace PRO
{
    public static class BlockMaterial
    {
        /// <summary>
        /// 相机中心所在的块坐标
        /// </summary>
        public static Vector2Int CameraCenterBlockPos { get; private set; }
        /// <summary>
        /// 光缓存的大小，(宽与高，最小为1,1)，只有在此范围内的才会被渲染线程提交给gpu
        /// </summary>
        public static Vector2Int LightBufferBlockSize { get; private set; }
        /// <summary>
        /// 每个区块接收多大范围内的光照,最小1,1,更改后需要去Shader中同步更改Block缓冲区数量
        /// </summary>
        public static Vector2Int EachBlockReceiveLightSize { get; private set; }
        public static int BlockBufferLength { get; private set; }
        public static int LightBufferLength { get; private set; }
        private static PROconfig proConfig;
        #region 公共材质

        public static BlockShareMaterialManager blockShareMaterialManager = new BlockShareMaterialManager();
        public static BackgroundShareMaterialManager backgroundShareMaterialManager = new BackgroundShareMaterialManager();
        public static ComputeShaderManager computeShaderManager = new ComputeShaderManager();

        #endregion

        public static void Init()
        {
            if (!JsonTool.LoadText(Application.streamingAssetsPath + @"\Json\PROconfig.json", out string proConfigText))
            {
                Debug.Log("PROconfig.json加载失败，BlockMaterial无法初始化");
                return;
            }
            proConfig = JsonTool.ToObject<PROconfig>(proConfigText);
            LightBufferBlockSize = proConfig.LightBufferBlockSize;
            EachBlockReceiveLightSize = proConfig.EachBlockReceiveLightSize;
            BlockBufferLength = (EachBlockReceiveLightSize.x - 1 + LightBufferBlockSize.x) * (EachBlockReceiveLightSize.y - 1 + LightBufferBlockSize.y);
            LightBufferLength = LightBufferBlockSize.x * LightBufferBlockSize.y;

            LoadAllPixelColorInfo();
            LoadAllLightSourceInfo();

            blockShareMaterialManager.Init();
            backgroundShareMaterialManager.Init();
            computeShaderManager.Init();
        }

        #region 像素点的颜色属性的加载与缓冲区
        //所有点的颜色数据_存储到数组，然后传递给GPU缓冲区，shader与计算着色器中使用
        public static PixelColorInfoToShader[] pixelColorInfoToShaderArray;
        public static ComputeBuffer pixelColorInfoToShaderBufffer;
        private static void LoadAllPixelColorInfo()
        {
            #region 加载路径下的所有PixelColorInfo.json文件，并存储到PixelColorInfoDic与List
            string rootPath = Application.streamingAssetsPath + @"\Json";
            DirectoryInfo root = new DirectoryInfo(rootPath);
            int pixelCount = 0;
            foreach (var fileInfo in root.GetFiles())
            {
                if (fileInfo.Extension != ".json") continue;
                string[] strArray = fileInfo.Name.Split('^');
                if (strArray.Length <= 1 || strArray[0] != "PixelColorInfo") continue;
                JsonTool.LoadText(fileInfo.FullName, out string infoText);
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
        public static PixelColorInfo GetPixelColorInfo(string pixelName)
        {
            if (pixelColorInfoDic.TryGetValue(pixelName, out PixelColorInfo value)) return value;
            else Debug.Log($"没有像素颜色名称为{pixelName}");
            return null;
        }
        public static PixelColorInfo GetPixelColorInfo(int id)
        {
            if (id >= pixelColorInfoList.Count) return null;
            else return pixelColorInfoList[id];
        }
        #endregion

        #region 光源信息
        private static void LoadAllLightSourceInfo()
        {
            #region 加载路径下的所有LightSourceInfo.json文件，并存储到LightSourceInfoDic与List
            string rootPath = Application.streamingAssetsPath + @"\Json";
            DirectoryInfo root = new DirectoryInfo(rootPath);
            foreach (var fileInfo in root.GetFiles())
            {
                if (fileInfo.Extension != ".json") continue;
                string[] strArray = fileInfo.Name.Split('^');
                if (strArray.Length <= 1 || strArray[0] != "LightSourceInfo") continue;
                JsonTool.LoadText(fileInfo.FullName, out string infoText);
                Log.Print(fileInfo.FullName, Color.green);
                int lightSourceCount = 0;
                //加载到的像素数组
                var InfoArray = JsonTool.ToObject<LightSourceInfo[]>(infoText);
                for (int i = 0; i < InfoArray.Length; i++)
                    if (lightSourceInfoDic.ContainsKey(InfoArray[i].name) == false)
                    {
                        InfoArray[i].index = lightSourceCount++;
                        lightSourceInfoDic.Add(InfoArray[i].name, InfoArray[i]);
                        lightSourceInfoList.Add(InfoArray[i]);
                    }
            }
            #endregion
        }
        ///光源信息的顺序索引
        private static List<LightSourceInfo> lightSourceInfoList = new List<LightSourceInfo>();
        //光源信息的字典索引
        private static Dictionary<string, LightSourceInfo> lightSourceInfoDic = new Dictionary<string, LightSourceInfo>();
        public static LightSourceInfo GetLightSourceInfo(string lightSourceName)
        {
            if (lightSourceInfoDic.TryGetValue(lightSourceName, out LightSourceInfo value)) return value;
            else if (lightSourceName.StartsWith(PixelColorInfoToShader.sign_SL) == false) Debug.Log($"没有光源名称为{lightSourceName}");
            return null;
        }
        public static LightSourceInfo GetLightSourceInfo(int id)
        {
            if (id >= lightSourceInfoList.Count) return null;
            else return lightSourceInfoList[id];
        }
        public static int LightSourceInfoListCount => lightSourceInfoList.Count;
        #endregion
        #region 将块数据传递到GPU
        public static void SetBlock(Block block)
        {
            Vector2Int minLightBufferBlockPos = CameraCenterBlockPos - LightBufferBlockSize / 2;
            Vector2Int minBlockBufferPos = minLightBufferBlockPos - EachBlockReceiveLightSize / 2;
            Vector2Int maxBlockBufferPos = minBlockBufferPos + LightBufferBlockSize - new Vector2Int(1, 1) + EachBlockReceiveLightSize - new Vector2Int(1, 1);

            if (block.BlockPos.x < minBlockBufferPos.x || block.BlockPos.x > maxBlockBufferPos.x
                && block.BlockPos.y < minBlockBufferPos.y || block.BlockPos.y > maxBlockBufferPos.y)
                return;

            Vector2Int localBlockBufferPos = block.BlockPos - minBlockBufferPos;
            int Info = localBlockBufferPos.x + localBlockBufferPos.y * (EachBlockReceiveLightSize.x - 1 + LightBufferBlockSize.x);
            if (Info < 0 || Info >= BlockBufferLength) return;

            blockShareMaterialManager.SetBufferData(Info, block.textureData.PixelIDToShader);
            //Debug.Log($"块坐标{block.BlockPos}  传递块数据  块缓存索引{Info}");
        }
        public static void SetBackgroundBlock(BackgroundBlock background)
        {
            Vector2Int minLightBufferBlockPos = CameraCenterBlockPos - LightBufferBlockSize / 2;
            Vector2Int maxLightBufferBlockPos = minLightBufferBlockPos + LightBufferBlockSize - new Vector2Int(1, 1);

            if (background.BlockPos.x < minLightBufferBlockPos.x || background.BlockPos.x > maxLightBufferBlockPos.x
                && background.BlockPos.y < minLightBufferBlockPos.y || background.BlockPos.y > maxLightBufferBlockPos.y)
                return;

            Vector2Int blockPos = background.BlockPos - minLightBufferBlockPos;
            int Info = blockPos.y * LightBufferBlockSize.x + blockPos.x;

            if (Info < 0 || Info >= LightBufferLength) return;

            backgroundShareMaterialManager.SetBufferData(Info, background.textureData.PixelIDToShader);
            //Debug.Log($"背景坐标{background.BlockPos}  传递背景数据   背景缓存索引{Info}");
        }
        #endregion

        public static void FirstBind()
        {
            blockShareMaterialManager.FirstBind();
            backgroundShareMaterialManager.FirstBind();
            computeShaderManager.FirstBind();

            Update();
        }
        public static void UpdateBind()
        {
            CameraCenterBlockPos = Block.WorldToBlock(Camera.main.transform.position);
            blockShareMaterialManager.UpdateBind();
            backgroundShareMaterialManager.UpdateBind();
            computeShaderManager.UpdateBind();
        }


        public static void Update()
        {
            Vector2Int nowCameraPos = Block.WorldToBlock(Camera.main.transform.position);
            if (nowCameraPos != CameraCenterBlockPos) UpdateBind();
            computeShaderManager.Update();


            if (Monitor.TryEnter(DrawApplyQueue))
            {
                try
                {
                    while (DrawApplyQueue.Count > 0)
                    {
                        BlockBase blockBase = DrawApplyQueue.Dequeue();
                        blockBase.spriteRenderer.sprite.texture.Apply();
                        switch (blockBase)
                        {
                            case Block block: BlockMaterial.SetBlock(block); break;
                            case BackgroundBlock background: BlockMaterial.SetBackgroundBlock(background); break;
                        }
                    }
                }
                finally
                {
                    Monitor.Exit(DrawApplyQueue);
                }
            }
        }

        /// <summary>
        /// 纹理绘制完成需要提交的队列，渲染线程添加，主线程取出（纹理只是提供一种选择，但最终显示RO不使用纹理）
        /// </summary>
        public static readonly Queue<BlockBase> DrawApplyQueue = new Queue<BlockBase>();
        /// <summary>
        /// 加入&锁住 提交绘制引用队列(新数据提交到显卡)
        /// </summary>
        public static void En_Lock_DrawApplyQueue(BlockBase block)
        {
            lock (DrawApplyQueue)
                DrawApplyQueue.Enqueue(block);
        }


        public static void Dispose()
        {
        }
    }
}