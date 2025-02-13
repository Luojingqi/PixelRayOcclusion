using Cysharp.Threading.Tasks;
using PRO.DataStructure;
using PRO.Disk;
using PRO.Disk.Scene;
using PRO.SkillEditor;
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
            //Log.Print("切换场景");
            //nowScene = toSceen;
            //BlockMaterial.UpdateBind();
        }
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
            Texture2DPool.InitPool();
            //GameSaveManager.Inst.CreateGameSaveFile("testSave");
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
            scenes.Add(nowSave.sceneNameList[0], scene);
            nowScene = scene;
            // scene.sceneCatalog.buildingTypeDic.ForEach(kv => Debug.Log(kv.Value.ToString()));
            //填充
            DrawThread.Init(() =>
           {
               BlockMaterial.FirstBind();
               FreelyLightSource.New(BlockMaterial.GetPixelColorInfo("鼠标光源0").color, 50).GloabPos = new Vector2Int();
               source = FreelyLightSource.New(BlockMaterial.GetPixelColorInfo("鼠标光源0").color, 20);


           });

        }
        public FreelyLightSource source;
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
        float time = 0;
        string n = "光源2";
        int l = 3;
        int k = 20;


        public async void Update()
        {

            Vector3 m = Input.mousePosition;
            m.z = 1;
            m = Camera.main.ScreenToWorldPoint(m);
            Vector2Int blockPos = Block.WorldToBlock(m);
            Vector2Int gloabPos = Block.WorldToGlobal(m);
            Vector2Byte pixelPos = Block.WorldToPixel(m);

            //if (Input.GetKeyDown(KeyCode.Mouse0))
            //{
            //    var hit = Physics2D.Raycast(m, Vector2.zero, 0, 1 << 9);
            //    if (hit.collider != null)
            //    {
            //        // Debug.Log(hit.transform.name + "|" + hit.collider.name);
            //        hit.transform.GetComponent<BuildingBase>().CreateSelectionBox(Color.red);
            //    }
            //}

            if (source != null)
                source.GloabPos = gloabPos;
            time += Time.deltaTime;
            while (time > 0.1f && Input.GetKey(KeyCode.Space))
            {
                time -= 0.1f;
                source.Radius++;
                k++;
                if (k > BlockMaterial.LightRadiusMax)
                {
                    source.Radius = 1;
                    k = 1;
                }
                for (int y = -l; y <= l; y++)
                    for (int x = -l; x <= l; x++)
                        NowScene.GetBlock(new(x, y)).UpdateFluid1();
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                Log.Print("保存");
                for (int x = -l; x <= l; x++)
                    for (int y = -l; y <= l; y++)
                        nowScene.SaveBlockData(new(x, y));
                foreach (var kv in nowScene.BuildingInRAM)
                    nowScene.SaveBuilding(kv.Key);
                nowScene.sceneCatalog.Save();
            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                Log.Print("加载开始，池内数量" + Pixel.pixelPool.notUsedObject.Count);
                t = Time.realtimeSinceStartup;
                for (int x = -l; x <= l; x++)
                    for (int y = -l; y <= l; y++)
                    {
                        nowScene.LoadBlockData(new(x, y));
                    }
                BlockMaterial.UpdateBind();
                Log.Print("加载完成" + (Time.realtimeSinceStartup - t) + "，池内数量" + Pixel.pixelPool.notUsedObject.Count);
            }
            if (Input.GetKeyDown(KeyCode.K))
            {
                t = Time.realtimeSinceStartup;
                Log.Print("回收开始，池内数量" + Pixel.pixelPool.notUsedObject.Count);
                t = Time.realtimeSinceStartup;
                nowScene.Unload();
                Log.Print("回收完成" + (Time.realtimeSinceStartup - t) + "，池内数量" + Pixel.pixelPool.notUsedObject.Count);
            }
            #region 光源
            if (Input.GetKeyDown(KeyCode.Alpha1))
                n = "光源0";
            if (Input.GetKeyDown(KeyCode.Alpha2))
                n = "光源1";
            if (Input.GetKeyDown(KeyCode.Alpha3))
                n = "光源2";
            if (Input.GetKeyDown(KeyCode.Alpha4))
                n = "光源4";
            if (Input.GetKeyDown(KeyCode.Alpha5))
                n = "光源5";
            if (Input.GetKeyDown(KeyCode.Alpha6))
                n = "光源6";
            if (Input.GetKeyDown(KeyCode.Alpha7))
                n = "光源7";
            if (Input.GetKeyDown(KeyCode.Alpha8))
                n = "光源8";
            if (Input.GetKeyDown(KeyCode.Alpha9))
                n = "光源9";
            if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.Mouse0))
            {
                Block block = NowScene.GetBlock(blockPos);
                block.SetPixel(Pixel.TakeOut("光源", n, pixelPos));
                block.DrawPixelAsync();
            }
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Mouse0))
            {
                Block block = NowScene.GetBlock(blockPos);
                for (int x = -1; x <= 1; x++)
                    for (int y = -1; y <= 1; y++)
                    {
                        var pixel = Pixel.TakeOut("水", 0, pixelPos + new Vector2Byte(x, y));
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
                        var pixel = Pixel.TakeOut("油", 0, pixelPos + new Vector2Byte(x, y));
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
                        var pixel = Pixel.TakeOut("沙子", 0, pixelPos + new Vector2Byte(0, 0));
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
                        var pixel = Pixel.TakeOut("砂砾", 0, pixelPos + new Vector2Byte(x, y));
                        if (pixel != null)
                            block.SetPixel(pixel);
                    }
                block.DrawPixelAsync();
            }
            #endregion

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
        private void LateUpdate()
        {
            BlockMaterial.Update();
        }

        private float t;
        private Vector2Int g;
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