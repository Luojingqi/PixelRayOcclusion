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
            //Log.Print("�л�����");
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
            Pixel.���� = Pixel.TakeOut("����", 0, new());
            GreedyCollider.InitBoxCollider2DPool();
            Block.InitPool();
            BackgroundBlock.InitPool();
            Texture2DPool.InitPool();
            //GameSaveManager.Inst.CreateGameSaveFile("testSave");
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
                //  FreelyLightSource.New(BlockMaterial.GetPixelColorInfo("����Դ0").color, 50).GloabPos = new Vector2Int();
                source = FreelyLightSource.New(BlockMaterial.GetPixelColorInfo("����Դ0").color, 20);


            });

        }
        public FreelyLightSource source;
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

        public static float updatetime = 0.2f;

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
                source.GloabPos = MousePoint.globalPos;
            // if (Input.GetKey(KeyCode.Space))
            time0 += Time.deltaTime;
            while (time0 > updatetime)
            {
                time0 -= updatetime;
                for (int y = -l; y <= l; y++)
                    for (int x = -l; x <= l; x++)
                    {
                        NowScene.GetBlock(new(x, y)).RandomUpdatePixelList((int)(updatetime * 1000));
                        NowScene.GetBackground(new(x, y)).RandomUpdatePixelList((int)(updatetime * 1000));
                    }
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