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
            PoolNode = new GameObject("PoolNode").transform;
            BlockMaterial.Init();
            Pixel.LoadPixelTypeInfo();
            Pixel.空气 = Pixel.TakeOut("空气", 0, new());
            GreedyCollider.InitBoxCollider2DPool();
            Block.InitPool();
            BackgroundBlock.InitPool();
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
                //  FreelyLightSource.New(BlockMaterial.GetPixelColorInfo("鼠标光源0").color, 50).GloabPos = new Vector2Int();
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
        //        if (block.blockPos.y < BlockUpdateList[b].blockPos.y)
        //            r = b - 1;
        //        else if (block.blockPos.y > BlockUpdateList[b].blockPos.y)
        //            l = b + 1;
        //        else break;
        //    }
        //    BlockUpdateList.Add(block);
        //    if (block.blockPos.y > BlockUpdateList[b].blockPos.y) b++;


        //    for (int i = b; i < BlockUpdateList.Count - 1; i++)
        //    {
        //        Block temp = BlockUpdateList[i];
        //        BlockUpdateList[i] = BlockUpdateList[BlockUpdateList.Count - 1];
        //        BlockUpdateList[BlockUpdateList.Count - 1] = temp;
        //    }
        //}
        #endregion
        float time0 = 0;
        float time1 = 0;
        float time2 = 0;
        int n = 2;
        int l = 3;
        int k = 20;


        public async void Update()
        {
            #region 
            //Vector2Int a = new Vector2Int(0, -4);
            //Debug.Log(Block.GlobalToWorld(a) + "|" + Block.WorldToGlobal(Block.GlobalToWorld(a)));
            ////if (Input.GetKeyDown(KeyCode.Mouse0))
            ////{
            ////    var hit = Physics2D.Raycast(m, Vector2.zero, 0, 1 << 9);
            ////    if (hit.collider != null)
            ////    {
            ////        // Debug.Log(hit.transform.name + "|" + hit.collider.name);
            ////        hit.transform.GetComponent<BuildingBase>().CreateSelectionBox(Color.red);
            ////    }
            ////}
            #endregion
            if (source != null)
                source.GloabPos = MousePoint.gloabPos;
            // if (Input.GetKey(KeyCode.Space))
            time0 += Time.deltaTime;
            while (time0 > 0.02f)
            {
                time0 -= 0.02f;
                for (int y = -l; y <= l; y++)
                    for (int x = -l; x <= l; x++)
                        NowScene.GetBlock(new(x, y)).RandomUpdatePixelList();
            }
            time1 += Time.deltaTime;
            while (time1 > 0.25f)
            {
                time1 -= 0.25f;
                for (int y = -l; y <= l; y++)
                    for (int x = -l; x <= l; x++)
                        NowScene.GetBlock(new(x, y)).UpdateFluid1();
            }
            time2 += Time.deltaTime;
            while (time2 > 0.125f)
            {
                time2 -= 0.125f;
                for (int y = l; y >= -l; y--)
                    for (int x = -l; x <= l; x++)
                        NowScene.GetBlock(new(x, y)).UpdateFluid2();
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
                n = 0;
            if (Input.GetKeyDown(KeyCode.Alpha2))
                n = 1;
            if (Input.GetKeyDown(KeyCode.Alpha3))
                n = 2;
            if (Input.GetKeyDown(KeyCode.Alpha4))
                n = 3;
            if (Input.GetKeyDown(KeyCode.Alpha5))
                n = 4;
            if (Input.GetKeyDown(KeyCode.Alpha6))
                n = 5;
            if (Input.GetKeyDown(KeyCode.Alpha7))
                n = 6;
            if (Input.GetKeyDown(KeyCode.Alpha8))
                n = 7;
            if (Input.GetKeyDown(KeyCode.Alpha9))
                n = 8;
            if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.Mouse0))
            {
                //MousePoint.block.SetPixel(Pixel.TakeOut("火焰", n, MousePoint.pixelPos));
                MousePoint.block.SetPixel(Pixel.TakeOut("光源", n, MousePoint.pixelPos));
                MousePoint.block.DrawPixelAsync();
            }
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Mouse0))
            {
                for (int x = -1; x <= 1; x++)
                    for (int y = -1; y <= 1; y++)
                    {
                        var pixel = Pixel.TakeOut("水蒸气", 0, MousePoint.pixelPos + new Vector2Byte(x, y));
                        if (pixel != null)
                            MousePoint.block.SetPixel(pixel);
                    }

                MousePoint.block.DrawPixelAsync();
            }
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Mouse1))
            {
                for (int x = -1; x <= 1; x++)
                    for (int y = -1; y <= 1; y++)
                    {
                        var pixel = Pixel.TakeOut("油", 0, MousePoint.pixelPos + new Vector2Byte(x, y));
                        if (pixel != null)
                            MousePoint.block.SetPixel(pixel);
                    }
                MousePoint.block.DrawPixelAsync();
            }
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Mouse3))
            {
                for (int x = -1; x <= 1; x++)
                    for (int y = -1; y <= 1; y++)
                    {
                        var pixel = Pixel.TakeOut("沙子", 0, MousePoint.pixelPos + new Vector2Byte(0, 0));
                        if (pixel != null)
                            MousePoint.block.SetPixel(pixel);
                    }
                MousePoint.block.DrawPixelAsync();
            }
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Mouse4))
            {
                for (int x = -1; x <= 1; x++)
                    for (int y = -1; y <= 1; y++)
                    {
                        var pixel = Pixel.TakeOut("砂砾", 0, MousePoint.pixelPos + new Vector2Byte(x, y));
                        if (pixel != null)
                            MousePoint.block.SetPixel(pixel);
                    }
                MousePoint.block.DrawPixelAsync();
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