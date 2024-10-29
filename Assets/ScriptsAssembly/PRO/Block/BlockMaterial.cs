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
        /// ����������ڵĿ�����
        /// </summary>
        public static Vector2Int CameraCenterBlockPos { get; private set; }
        /// <summary>
        /// �⻺��Ĵ�С��(����ߣ���СΪ1,1)��ֻ���ڴ˷�Χ�ڵĲŻᱻ��Ⱦ�߳��ύ��gpu
        /// </summary>
        public static Vector2Int LightBufferBlockSize { get; private set; }
        /// <summary>
        /// ÿ��������ն��Χ�ڵĹ���,��С1,1,���ĺ���ҪȥShader��ͬ������Block����������
        /// </summary>
        public static Vector2Int EachBlockReceiveLightSize { get; private set; }
        public static int BlockBufferLength { get; private set; }
        public static int LightBufferLength { get; private set; }
        private static PROconfig proConfig;
        #region ��������

        public static BlockShareMaterialManager blockShareMaterialManager = new BlockShareMaterialManager();
        public static BackgroundShareMaterialManager backgroundShareMaterialManager = new BackgroundShareMaterialManager();
        public static ComputeShaderManager computeShaderManager = new ComputeShaderManager();

        #endregion

        public static void Init()
        {
            if (!JsonTool.LoadText(Application.streamingAssetsPath + @"\Json\PROconfig.json", out string proConfigText))
            {
                Debug.Log("PROconfig.json����ʧ�ܣ�BlockMaterial�޷���ʼ��");
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

        #region ���ص����ɫ���Եļ����뻺����
        //���е����ɫ����_�洢�����飬Ȼ�󴫵ݸ�GPU��������shader�������ɫ����ʹ��
        public static PixelColorInfoToShader[] pixelColorInfoToShaderArray;
        public static ComputeBuffer pixelColorInfoToShaderBufffer;
        private static void LoadAllPixelColorInfo()
        {
            #region ����·���µ�����PixelColorInfo.json�ļ������洢��PixelColorInfoDic��List
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
                //���ص�����������
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

            #region ��allPixelColorInfoת��ΪallPixelColorInfoToShader���ύ��GPU
            pixelColorInfoToShaderArray = new PixelColorInfoToShader[pixelCount + 10];//�Ӽ�������
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
        //���е����ɫ����_˳������
        private static List<PixelColorInfo> pixelColorInfoList = new List<PixelColorInfo>();
        //����ɫ����_�ֵ�����
        private static Dictionary<string, PixelColorInfo> pixelColorInfoDic = new Dictionary<string, PixelColorInfo>();
        public static PixelColorInfo GetPixelColorInfo(string pixelName)
        {
            if (pixelColorInfoDic.TryGetValue(pixelName, out PixelColorInfo value)) return value;
            else Debug.Log($"û��������ɫ����Ϊ{pixelName}");
            return null;
        }
        public static PixelColorInfo GetPixelColorInfo(int id)
        {
            if (id >= pixelColorInfoList.Count) return null;
            else return pixelColorInfoList[id];
        }
        #endregion

        #region ��Դ��Ϣ
        private static void LoadAllLightSourceInfo()
        {
            #region ����·���µ�����LightSourceInfo.json�ļ������洢��LightSourceInfoDic��List
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
                //���ص�����������
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
        ///��Դ��Ϣ��˳������
        private static List<LightSourceInfo> lightSourceInfoList = new List<LightSourceInfo>();
        //��Դ��Ϣ���ֵ�����
        private static Dictionary<string, LightSourceInfo> lightSourceInfoDic = new Dictionary<string, LightSourceInfo>();
        public static LightSourceInfo GetLightSourceInfo(string lightSourceName)
        {
            if (lightSourceInfoDic.TryGetValue(lightSourceName, out LightSourceInfo value)) return value;
            else if (lightSourceName.StartsWith(PixelColorInfoToShader.sign_SL) == false) Debug.Log($"û�й�Դ����Ϊ{lightSourceName}");
            return null;
        }
        public static LightSourceInfo GetLightSourceInfo(int id)
        {
            if (id >= lightSourceInfoList.Count) return null;
            else return lightSourceInfoList[id];
        }
        public static int LightSourceInfoListCount => lightSourceInfoList.Count;
        #endregion
        #region �������ݴ��ݵ�GPU
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
            //Debug.Log($"������{block.BlockPos}  ���ݿ�����  �黺������{Info}");
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
            //Debug.Log($"��������{background.BlockPos}  ���ݱ�������   ������������{Info}");
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
        /// ������������Ҫ�ύ�Ķ��У���Ⱦ�߳���ӣ����߳�ȡ��������ֻ���ṩһ��ѡ�񣬵�������ʾRO��ʹ������
        /// </summary>
        public static readonly Queue<BlockBase> DrawApplyQueue = new Queue<BlockBase>();
        /// <summary>
        /// ����&��ס �ύ�������ö���(�������ύ���Կ�)
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