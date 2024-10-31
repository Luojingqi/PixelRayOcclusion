using Cysharp.Threading.Tasks;
using PRO.DataStructure;
using PRO.Disk.Scene;
using PRO.Tool;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
namespace PRO
{

    public class SceneManager : MonoBehaviour
    {

        public static SceneManager Inst;

        public void Awake() { Inst = this; }

        private SceneEntity nowScene;
        public SceneEntity NowScene { get => nowScene; }
        private Dictionary<string, SceneEntity> scenes = new Dictionary<string, SceneEntity>();
        public Block GetBlock(Vector2Int blockPos, string sceneName ) => scenes[sceneName].GetBlock(blockPos);
        public BackgroundBlock GetBackground(Vector2Int blockPos, string sceneName) => scenes[sceneName].GetBackground(blockPos);
        public SceneEntity GetScene(string sceneName) => scenes[sceneName];

        public async void SwitchScene(string toSceneName)
        {
            // if (toSceneName == nowScene.name) return;
            //SceneEntity toSceen = new SceneEntity("123");
            //DrawThread.InitScene(toSceen);
            //await UniTask.Delay(2500);
            //Log.Print("切换场景");
            //nowScene = toSceen;
            //BlockMaterial.UpdateBind();
        }


        public void Start()
        {
            DontDestroyOnLoad(this);
            BlockMaterial.Init();
            PoolNode = new GameObject("PoolNode").transform;

            GreedyCollider.InitBoxCollider2DPool();
            Block.InitBlockPool();
            BackgroundBlock.InitBackgroundPool();
            Pixel.LoadPixelTypeInfo();
            //加载所有的存档目录
            var catalogList = GameSaveManager.Inst.LoadAllSaveCatalog();
            catalogList.ForEach(item => Log.Print(item.name));
            //选择第一个存档
            GameSaveManager.Inst.SwitchSave(catalogList[0]);
            GameSaveCatalog nowSave = GameSaveManager.Inst.NowGameSave;
            //选择存档的第一个场景
            SceneCatalog sceneCatalog = nowSave.sceneCatalogDic[nowSave.sceneNameList[0]];
            //转换为实体数据
            SceneEntity scene = new SceneEntity(sceneCatalog);
            scenes.Add(scene.sceneInfo.name, scene);
            nowScene = scene;
            //填充
            DrawThread.Init(() =>
           {
               BlockMaterial.FirstBind();
           });
            //scenes.Add("main", new SceneEntity(""));
            //nowScene = scenes["main"];

        }

        public Transform PoolNode;
        #region 弃用
        //[NonSerialized]
        ///// <summary>
        ///// 需要更新的区块
        ///// </summary>
        //public List<Block> BlockUpdateList = new List<Block>();
        ///// <summary>
        ///// 添加需要更新的区块
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

        string n = "光源2";
        public void Update()
        {

            Vector3 m = Input.mousePosition;
            m.z = 1;
            m = Camera.main.ScreenToWorldPoint(m);
            Vector2Int blockPos = Block.WorldToBlock(m);
            Vector2Int gloabPos = Block.WorldToGloab(m);
            Vector2Byte pixelPos = Block.WorldToPixel(m);
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ThreadPool.QueueUserWorkItem((obj) =>
                {
                    StarManager.Inst.CreateStar(new(10, 10), 20, 20, "透光墙", 10);
                    StarManager.Inst.CreateStar(new(10, 10), 21, 20, "透光墙", 10);
                    StarManager.Inst.CreateStar(new(15, 15), 10, 10, "透光墙", 10);
                    StarManager.Inst.CreateStar(new(15, 15), 30, 10, "不透光墙", 10);
                    StarManager.Inst.CreateStar(new(15, 15), 31, 10, "不透光墙", 10);
                    StarManager.Inst.CreateStar(new(15, 15), 33, 10, "不透光墙", 10);
                    //for (int i = 0; i <= 0; i++)
                    //{
                    //    for (int j = 0; j <= 0; j++)
                    //    {
                    //        var block = SceneManager.Inst.BlockCrossList[i][j];
                    //        var colliderDataList = GreedyCollider.CreateColliderDataList(block, new(0, 0), new(Block.Size.x - 1, Block.Size.y - 1));
                    //        lock (SceneManager.Inst.mainThreadEventLock)
                    //            SceneManager.Inst.mainThreadEvent += () => { GreedyCollider.CreateColliderAction(block, colliderDataList); };
                    //    }
                    //}
                });
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                SwitchScene("123");
            }
            #region 光源
            if (Input.GetKeyDown(KeyCode.Alpha1))
                n = "光源0";
            if (Input.GetKeyDown(KeyCode.Alpha2))
                n = "光源1";
            if (Input.GetKeyDown(KeyCode.Alpha3))
                n = "光源2";
            if (Input.GetKeyDown(KeyCode.Alpha4))
                n = "光源3";
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Block block = NowScene.GetBlock(blockPos);
                block.SetPixel(Pixel.TakeOut("光源", n, pixelPos));
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

        #region 任务队列与线程锁
        /// <summary>
        /// 绘制图形任务队列，主线程添加，渲染线程取出
        /// </summary>
        public readonly Queue<DrawGraphTaskData> DrawGraphTaskQueue = new Queue<DrawGraphTaskData>();

        /// <summary>
        /// 添加一个绘制图形的任务
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
        /// 主线程的事件锁
        /// </summary>
        public readonly object mainThreadEventLock = new object();
        /// <summary>
        /// 主线程事件，添加事件需要先锁定主线程锁mainThreadEventLock
        /// </summary>
        public event Action mainThreadEvent;
        #endregion
    }


}