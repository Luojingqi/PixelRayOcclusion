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
        /// ����������ڵĿ�����
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
        /// ��һ֡����������ڵĿ�����
        /// </summary>
        public static Vector2Int LastCameraCenterBlockPos { get; private set; }
        /// <summary>
        /// ����Դ�뾶���޸������Ҫ���ñ༭����չPRO>2.����hlsl
        /// ������LightRadiusMax��������ɫ������  ÿ������һ����ͬ�뾶�Ĺ�Դ����
        /// </summary>
        public static readonly int LightRadiusMax = 128;
        /// <summary>
        /// �⻺��Ĵ�С��(����ߣ���СΪ1,1)��ֻ���ڴ˷�Χ�ڵĲŻᱻ��Ⱦ�߳��ύ��gpu
        /// ��������ڵ����鿪ʼ���𣬳����Ĳ��ֲ�����ʾ����Ч��
        /// ���ĺ���Ҫ ���ñ༭����չPRO>2.����hlsl
        /// </summary>
        public static readonly Vector2Int LightResultBufferBlockSize = new Vector2Int(7, 5);
        /// <summary>
        /// ÿ��������ն��Χ�ڵĹ���,��С1,1
        /// Buffer_LightBuffer.hlsl ��StructuredBuffer<int> BlockBuffer0;
        /// ���ĺ���Ҫ ���ñ༭����չPRO>2.����hlsl
        /// </summary>
        public static readonly Vector2Int EachBlockReceiveLightSize = new Vector2Int(5, 5);
        /// <summary>
        /// ÿ֡���µ���������
        /// </summary>
        public static int FrameUpdateBlockNum;

        public static int BlockBufferLength { get; private set; }
        public static int LightResultBufferLength { get; private set; }
        public static PROconfig proConfig;
        /// <summary>
        /// Ϊ�յĲ�������
        /// </summary>
        public static MaterialPropertyBlock NullMaterialPropertyBlock;
        #region ��������

        public static BlockShareMaterialManager blockShareMaterialManager = new BlockShareMaterialManager();
        public static BackgroundShareMaterialManager backgroundShareMaterialManager = new BackgroundShareMaterialManager();
        public static ComputeShaderManager computeShaderManager = new ComputeShaderManager();

        #endregion

        public static void Init()
        {
            CameraCenterBlockPos = Block.WorldToBlock(Camera.main.transform.position);
            if (!IOTool.LoadText(Application.streamingAssetsPath + @"\Json\PROconfig.json", out string proConfigText))
            {
                Debug.Log("PROconfig.json����ʧ�ܣ�BlockMaterial�޷���ʼ��");
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

        #region ���ص����ɫ���Եļ����뻺����
        //���е����ɫ����_�洢�����飬Ȼ�󴫵ݸ�GPU��������shader�������ɫ����ʹ��
        public static PixelColorInfoToShader[] pixelColorInfoToShaderArray;
        public static ComputeBuffer pixelColorInfoToShaderBufffer;
        private static void LoadAllPixelColorInfo()
        {
            #region ����·���µ�����PixelColorInfo.json�ļ������洢��PixelColorInfoDic��List
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
        public static PixelColorInfo GetPixelColorInfo(string colorName)
        {
            if (pixelColorInfoDic.TryGetValue(colorName, out PixelColorInfo value)) return value;
            else Debug.Log($"û��������ɫ����Ϊ{colorName}");
            return null;
        }
        public static PixelColorInfo GetPixelColorInfo(int id)
        {
            if (id >= pixelColorInfoList.Count) return null;
            else return pixelColorInfoList[id];
        }
        #endregion

        #region �������ݴ��ݵ�GPU
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
            //Debug.Log($"������{block.BlockPos}  ���ݿ�����  �黺������{index}");
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
            //Debug.Log($"��������{background.BlockPos}  ���ݱ�������   ������������{index}");
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
        /// �������λ�ã����ظ���λ�ú�����飬�ӳ���Ұ�������ж�ص���ʱ
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
        /// ������������Ҫ�ύ�Ķ��У���Ⱦ�߳���ӣ����߳�ȡ��������ֻ���ṩһ��ѡ�񣬵�������ʾ�����ڵ�Ч����ʹ��������ͼ��
        /// </summary>
        public static readonly Queue<BlockBase> DrawApplyQueue = new Queue<BlockBase>();
        /// <summary>
        /// ����&��ס �ύ�������ö���(�������ύ���Կ�)
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