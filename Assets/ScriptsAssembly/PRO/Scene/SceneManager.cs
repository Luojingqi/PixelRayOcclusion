using Cysharp.Threading.Tasks;
using PRO.DataStructure;
using PRO.Disk;
using PRO.Disk.Scene;
using PRO.Renderer;
using PRO.SkillEditor;
using PRO.Tool;
using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using static PRO.BlockMaterial;
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
            // scene.sceneCatalog.buildingTypeDic.ForEach(kv => Debug.Log(kv.value.ToString()));
            //��ʼ������
            DrawThread.Init(nowScene);
            BlockMaterial.FirstBind();
            source = FreelyLightSource.New(NowScene, BlockMaterial.GetPixelColorInfo("����Դ0").color, 20);
            List<Block> list0 = new List<Block>();
            List<BackgroundBlock> list1 = new List<BackgroundBlock>();
            //for (int i = 0; i < 1000; i++)
            //{
            //    list0.Add(Block.TakeOut());
            //    list1.Add(BackgroundBlock.TakeOut());
            //}
            //list0.ForEach(b => Block.PutIn(b));
            //list1.ForEach(b => BackgroundBlock.PutIn(b));
        }
        public FreelyLightSource source;
        public Transform PoolNode;
        float time0 = 0;
        float time1 = 0;
        float time2 = 0;
        int l = 3;

        public static float updatetime = 0.2f;

        public void Update()
        {
            MousePoint.Update();
            if (source != null) source.GloabPos = MousePoint.globalPos;
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

            if (Monitor.TryEnter(mainThreadUpdateEventLock_UnClear))
            {
                try
                {
                    mainThreadUpdateEvent_UnClear?.Invoke();
                }
                finally
                {
                    Monitor.Exit(mainThreadUpdateEventLock_UnClear);
                }
            }
            if (Monitor.TryEnter(mainThreadUpdateEventLock_Clear))
            {
                try
                {
                    mainThreadUpdateEvent_Clear?.Invoke();
                    mainThreadUpdateEvent_Clear = null;
                }
                finally
                {
                    Monitor.Exit(mainThreadUpdateEventLock_Clear);
                }
            }
        }
        private void LateUpdate()
        {
            if (Monitor.TryEnter(BlockMaterial.DrawApplyQueue))
            {
                try
                {
                    while (BlockMaterial.DrawApplyQueue.Count > 0)
                    {
                        BlockBase blockBase = BlockMaterial.DrawApplyQueue.Dequeue();
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
                    Monitor.Exit(BlockMaterial.DrawApplyQueue);
                }
            }
            BlockMaterial.Update();
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

        #region ���̸߳����¼�_UnClear
        /// <summary>
        /// ���̵߳ĸ����¼���
        /// </summary>
        private readonly object mainThreadUpdateEventLock_UnClear = new object();
        /// <summary>
        /// ���̸߳����¼�
        /// </summary>
        private event Action mainThreadUpdateEvent_UnClear;
        /// <summary>
        /// �¼���ӵ����̸߳����¼�
        /// </summary>
        /// <param name="action"></param>
        public void AddMainThreadEvent_UnClear_Lock(Action action)
        {
            lock (mainThreadUpdateEventLock_UnClear)
            {
                mainThreadUpdateEvent_UnClear += action;
            }
        }
        #endregion

        #region ���̸߳����¼�_Clear
        /// <summary>
        /// ���̵߳ĸ����¼���
        /// </summary>
        private readonly object mainThreadUpdateEventLock_Clear = new object();
        /// <summary>
        /// ���̸߳����¼�
        /// </summary>
        private event Action mainThreadUpdateEvent_Clear;
        /// <summary>
        /// �¼���ӵ����̸߳����¼�
        /// </summary>
        /// <param name="action"></param>
        public void AddMainThreadEvent_Clear_Lock(Action action)
        {
            lock (mainThreadUpdateEventLock_Clear)
            {
                mainThreadUpdateEvent_Clear += action;
            }
        }
        private ObjectPoolArbitrary<AutoResetEvent> resetEventPool = new ObjectPoolArbitrary<AutoResetEvent>(() => new AutoResetEvent(false));
        /// <summary>
        /// �¼���ӵ����̸߳����¼������ȴ�ִ����ϣ���ֹ���̵߳���
        /// </summary>
        /// <param name="action"></param>
        public void AddMainThreadEvent_Clear_WaitInvoke_Lock(Action action)
        {
            var manual = resetEventPool.TakeOut();
            lock (mainThreadUpdateEventLock_Clear)
            {
                mainThreadUpdateEvent_Clear += action;
                mainThreadUpdateEvent_Clear += () => manual.Set();
            }
            manual.WaitOne();
            resetEventPool.PutIn(manual);
        }
        #endregion

        #endregion
    }
}