using Cysharp.Threading.Tasks;
using PRO.DataStructure;
using PRO.Disk.Scene;
using PRO.Tool;
using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
namespace PRO
{

    public class SceneManager : MonoBehaviour
    {

        public static SceneManager Inst;


        private SceneEntity nowScene;
        public SceneEntity NowScene { get => nowScene; }
        private Dictionary<string, SceneEntity> scenes = new Dictionary<string, SceneEntity>();
        public Block GetBlock(Vector2Int blockPos, string sceneName) => scenes[sceneName].GetBlock(blockPos);
        public BackgroundBlock GetBackground(Vector2Int blockPos, string sceneName) => scenes[sceneName].GetBackground(blockPos);
        public SceneEntity GetScene(string sceneName) => scenes[sceneName];

        public async void SwitchScene(string toSceneName)
        {
            // if (toSceneName == nowScene.name) return;
            //SceneEntity toSceen = new SceneEntity("123");
            //DrawThread.InitScene(toSceen);
            //await UniTask.Delay(2500);
            //Log.Print("�л�����");
            //nowScene = toSceen;
            //BlockMaterial.UpdateBind();
        }

        public GameObject g;
        public void Awake()
        {
            Inst = this;
            DontDestroyOnLoad(this);
            BlockMaterial.Init();
            PoolNode = new GameObject("PoolNode").transform;

            GreedyCollider.InitBoxCollider2DPool();
            Block.InitPool();
            BackgroundBlock.InitPool();
            Pixel.LoadPixelTypeInfo();
            // GameSaveManager.Inst.CreateGameSaveFile("testSave");
            //�������еĴ浵Ŀ¼
            var catalogList = GameSaveManager.Inst.LoadAllSaveCatalog();
            catalogList.ForEach(item => Log.Print(item.name));
            //ѡ���һ���浵
            GameSaveManager.Inst.SwitchSave(catalogList[0]);
            GameSaveCatalog nowSave = GameSaveManager.Inst.NowGameSave;
            //ѡ��浵�ĵ�һ������
            SceneCatalog sceneCatalog = nowSave.sceneCatalogDic[nowSave.sceneNameList[0]];
            //ת��Ϊʵ������
            SceneEntity scene = new SceneEntity(sceneCatalog);
            scenes.Add(nowSave.sceneNameList[0], scene);
            nowScene = scene;
            // scene.sceneCatalog.buildingTypeDic.ForEach(kv => Debug.Log(kv.Value.ToString()));
            //���
            DrawThread.Init(() =>
           {
               BlockMaterial.FirstBind();
               Block block = NowScene.GetBlock(new Vector2Int(0, 0));
               block.SetPixel(Pixel.TakeOut("��Դ", "��Դ3", new Vector2Byte()));
               block.DrawPixelAsync();
           });

        }

        public Transform PoolNode;
        #region ����
        //[NonSerialized]
        ///// <summary>
        ///// ��Ҫ���µ�����
        ///// </summary>
        //public List<Block> BlockUpdateList = new List<Block>();
        ///// <summary>
        ///// �����Ҫ���µ�����
        ///// </summary>
        //public void AddUpdateBlock(Block block)
        //{
        //    int l = 0;
        //    int r = BlockUpdateList.Count - 1;
        //    int b = 0;
        //    while (l <= r)
        //    {
        //        b = (l + r) / 2;
        //        if (block.BlockPos.y < BlockUpdateList[b].BlockPos.y)
        //            r = b - 1;
        //        else if (block.BlockPos.y > BlockUpdateList[b].BlockPos.y)
        //            l = b + 1;
        //        else break;
        //    }
        //    BlockUpdateList.Add(block);
        //    if (block.BlockPos.y > BlockUpdateList[b].BlockPos.y) b++;


        //    for (int i = b; i < BlockUpdateList.Count - 1; i++)
        //    {
        //        Block temp = BlockUpdateList[i];
        //        BlockUpdateList[i] = BlockUpdateList[BlockUpdateList.Count - 1];
        //        BlockUpdateList[BlockUpdateList.Count - 1] = temp;
        //    }
        //}
        #endregion
        float time = 0;
        string n = "��Դ2";
        public void Update()
        {

            Vector3 m = Input.mousePosition;
            m.z = 1;
            m = Camera.main.ScreenToWorldPoint(m);
            Vector2Int blockPos = Block.WorldToBlock(m);
            Vector2Int gloabPos = Block.WorldToGlobal(m);
            Vector2Byte pixelPos = Block.WorldToPixel(m);


            time += Time.deltaTime;
            while (time > 0.1f)
            {
                time -= 0.1f;
                for (int y = -2; y <= 2; y++)
                    for (int x = -2; x <= 2; x++)
                        NowScene.GetBlock(new(x, y)).UpdateFluid1();
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                Span<int> aaa = stackalloc[] { 123 };
                // SwitchScene("123");
                Log.Print("����");
                for (int x = -1; x <= 1; x++)
                    for (int y = -1; y <= 1; y++)
                        nowScene.SaveBlockData(new(x, y));
            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                Log.Print("����");
                for (int x = -1; x <= 1; x++)
                    for (int y = -1; y <= 1; y++)
                    {
                        Block.PutIn(nowScene.GetBlock(new(x, y)));
                        nowScene.LoadBlockData(new(x, y));
                    }
                BlockMaterial.UpdateBind();

            }
            #region ��Դ
            if (Input.GetKeyDown(KeyCode.Alpha1))
                n = "��Դ0";
            if (Input.GetKeyDown(KeyCode.Alpha2))
                n = "��Դ1";
            if (Input.GetKeyDown(KeyCode.Alpha3))
                n = "��Դ2";
            if (Input.GetKeyDown(KeyCode.Alpha4))
                n = "��Դ4";
            if (Input.GetKeyDown(KeyCode.Alpha5))
                n = "��Դ5";
            if (Input.GetKeyDown(KeyCode.Alpha6))
                n = "��Դ6";
            if (Input.GetKeyDown(KeyCode.Alpha7))
                n = "��Դ7";
            if (Input.GetKeyDown(KeyCode.Alpha8))
                n = "��Դ8";
            if (Input.GetKeyDown(KeyCode.Alpha9))
                n = "��Դ9";
            if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.Mouse0))
            {
                Block block = NowScene.GetBlock(blockPos);
                block.SetPixel(Pixel.TakeOut("��Դ", n, pixelPos));
                block.DrawPixelAsync();
            }
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Mouse0))
            {
                Block block = NowScene.GetBlock(blockPos);
                for (int x = -1; x <= 1; x++)
                    for (int y = -1; y <= 1; y++)
                    {
                        var pixel = Pixel.TakeOut("ˮ", 0, pixelPos + new Vector2Byte(x, y));
                        if (pixel != null)
                            block.SetPixel(pixel);
                    }

                block.DrawPixelAsync();
            }
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Mouse1))
            {
                Block block = NowScene.GetBlock(blockPos);
                for (int x = -1; x <= 1; x++)
                    for (int y = -1; y <= 1; y++)
                    {
                        var pixel = Pixel.TakeOut("��", 0, pixelPos + new Vector2Byte(x, y));
                        if (pixel != null)
                            block.SetPixel(pixel);
                    }
                block.DrawPixelAsync();
            }
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Mouse3))
            {
                Block block = NowScene.GetBlock(blockPos);
                for (int x = -1; x <= 1; x++)
                    for (int y = -1; y <= 1; y++)
                    {
                        var pixel = Pixel.TakeOut("ɳ��", 0, pixelPos + new Vector2Byte(0, 0));
                        if (pixel != null)
                            block.SetPixel(pixel);
                    }
                block.DrawPixelAsync();
            }
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Mouse4))
            {
                Block block = NowScene.GetBlock(blockPos);
                for (int x = -1; x <= 1; x++)
                    for (int y = -1; y <= 1; y++)
                    {
                        var pixel = Pixel.TakeOut("ɰ��", 0, pixelPos + new Vector2Byte(x, y));
                        if (pixel != null)
                            block.SetPixel(pixel);
                    }
                block.DrawPixelAsync();
            }
            #endregion
            BlockMaterial.Update();
            if (Monitor.TryEnter(mainThreadEventLock))
            {
                try
                {
                    mainThreadEvent?.Invoke();
                    mainThreadEvent = null;
                }
                finally
                {
                    Monitor.Exit(mainThreadEventLock);
                }
            }



        }

        #region ����������߳���
        /// <summary>
        /// ����ͼ��������У����߳���ӣ���Ⱦ�߳�ȡ��
        /// </summary>
        public readonly Queue<DrawGraphTaskData> DrawGraphTaskQueue = new Queue<DrawGraphTaskData>();

        /// <summary>
        /// ���һ������ͼ�ε�����
        /// </summary>
        /// <param name="drawTask"></param>
        public async void EnqueueDrawGraphTaskAsync(DrawGraphTaskData drawTask)
        {
            while (true)
            {
                bool queueLock = false;
                try
                {
                    queueLock = Monitor.TryEnter(DrawGraphTaskQueue);

                    if (queueLock)
                    {
                        DrawGraphTaskQueue.Enqueue(drawTask);
                        break;
                    }
                    else
                    {
                        await UniTask.Yield();
                    }
                }
                finally
                {
                    if (queueLock)
                        Monitor.Exit(DrawGraphTaskQueue);
                }
            }
        }
        /// <summary>
        /// ���̵߳��¼���
        /// </summary>
        public readonly object mainThreadEventLock = new object();
        /// <summary>
        /// ���߳��¼�������¼���Ҫ���������߳���mainThreadEventLock
        /// </summary>
        public event Action mainThreadEvent;
        #endregion
    }


}