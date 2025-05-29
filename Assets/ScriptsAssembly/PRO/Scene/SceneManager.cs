using PRO.Disk.Scene;
using PRO.Tool;
using PROTool;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
namespace PRO
{

    public class SceneManager : MonoScriptBase, ITime_Awake, ITime_Update, ITime_LateUpdate
    {

        public static SceneManager Inst;


        private SceneEntity nowScene;
        public SceneEntity NowScene { get => nowScene; }

        private Dictionary<string, SceneEntity> scenes = new Dictionary<string, SceneEntity>();
        public Block GetBlock(Vector2Int blockPos, string sceneName) => scenes[sceneName].GetBlock(blockPos);
        public BackgroundBlock GetBackground(Vector2Int blockPos, string sceneName) => scenes[sceneName].GetBackground(blockPos);
        public SceneEntity GetScene(string sceneName) => scenes[sceneName];

        public void SwitchScene(string toSceneName)
        {
            // if (toSceneName == nowScene.name) return;
            //SceneEntity toSceen = new SceneEntity("123");
            //DrawThread.InitScene(toSceen);
            //await UniTask.Delay(2500);
            //Log.Print("�л�����");
            //nowScene = toSceen;
            //BlockMaterial.UpdateBind();
        }
        public void TimeAwake()
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

            foreach (var buildingType in ReflectionTool.GetDerivedClasses(typeof(BuildingBase)))
                SceneEntity.BuildingTypeDic.Add(buildingType.Name, buildingType);
            //ת��Ϊʵ������
            SceneEntity scene = new SceneEntity(sceneCatalog);
            scenes.Add(nowSave.sceneNameList[0], scene);
            nowScene = scene;
            //��ʼ������
            new Thread(() => DrawThread.LoopDraw()).Start();
            BlockMaterial.FirstBind();
            source = FreelyLightSource.New(NowScene, BlockMaterial.GetPixelColorInfo("����Դ0").color, 20);
            List<Block> list0 = new List<Block>();
            List<BackgroundBlock> list1 = new List<BackgroundBlock>();
        }

        public FreelyLightSource source;
        public Transform PoolNode;
        float time���� = 0;
        float timeFluid1 = 0;
        float timeFluid2 = 0;
        float timeFluid3 = 0;
        float UnLoadBlockCountdownTime = 0;
        List<Vector2Int> unLoadBlock = new List<Vector2Int>(20);
        int l = 3;

        public void TimeUpdate()
        {
            MousePoint.Update();

            if (source != null) source.GloabPos = MousePoint.globalPos;
            Vector2Int minLightBufferBlockPos = BlockMaterial.CameraCenterBlockPos - BlockMaterial.LightResultBufferBlockSize / 2;
            Vector2Int maxLightBufferBlockPos = minLightBufferBlockPos + BlockMaterial.LightResultBufferBlockSize;
            time���� += TimeManager.deltaTime;
            timeFluid1 += TimeManager.deltaTime;
            timeFluid2 += TimeManager.deltaTime;
            timeFluid3 += TimeManager.deltaTime;
            while (time���� > BlockMaterial.proConfig.UpdateTime����)
            {
                time���� -= BlockMaterial.proConfig.UpdateTime����;
                int updateTime = (int)(BlockMaterial.proConfig.UpdateTime���� * 1000);
                for (int y = minLightBufferBlockPos.y; y <= maxLightBufferBlockPos.y; y++)
                    for (int x = minLightBufferBlockPos.x; x <= maxLightBufferBlockPos.x; x++)
                    {
                        NowScene.GetBlock(new(x, y)).Update_����ȼ��(updateTime);
                        NowScene.GetBackground(new(x, y)).Update_����ȼ��(updateTime);
                    }
            }
            while (timeFluid1 > BlockMaterial.proConfig.UpdateTimeFluid1)
            {
                timeFluid1 -= BlockMaterial.proConfig.UpdateTimeFluid1;
                for (int y = minLightBufferBlockPos.y; y <= maxLightBufferBlockPos.y; y++)
                    for (int x = minLightBufferBlockPos.x; x <= maxLightBufferBlockPos.x; x++)
                    {
                        NowScene.GetBlock(new(x, y)).UpdateFluid1();
                    }
            }
            while (timeFluid2 > BlockMaterial.proConfig.UpdateTimeFluid2)
            {
                timeFluid2 -= BlockMaterial.proConfig.UpdateTimeFluid2;
                for (int y = minLightBufferBlockPos.y; y <= maxLightBufferBlockPos.y; y++)
                    for (int x = minLightBufferBlockPos.x; x <= maxLightBufferBlockPos.x; x++)
                    {
                        NowScene.GetBlock(new(x, y)).UpdateFluid2();
                    }
            }
            while (timeFluid3 > BlockMaterial.proConfig.UpdateTimeFluid3)
            {
                timeFluid3 -= BlockMaterial.proConfig.UpdateTimeFluid3;
                for (int y = minLightBufferBlockPos.y; y <= maxLightBufferBlockPos.y; y++)
                    for (int x = minLightBufferBlockPos.x; x <= maxLightBufferBlockPos.x; x++)
                    {
                        NowScene.GetBlock(new(x, y)).UpdateFluid3();
                    }
            }
            UnLoadBlockCountdownTime += TimeManager.deltaTime;
            while (UnLoadBlockCountdownTime > BlockMaterial.proConfig.AutoUnLoadBlockCountdownTime)
            {
                UnLoadBlockCountdownTime -= BlockMaterial.proConfig.AutoUnLoadBlockCountdownTime;
                foreach (var scene in scenes.Values)
                {
                    foreach (var blockPos in scene.BlockBaseInRAM)
                    {
                        var block = scene.GetBlock(blockPos);
                        block.UnLoadCountdown -= BlockMaterial.proConfig.AutoUnLoadBlockCountdownTime;
                        if (block.UnLoadCountdown <= 0)
                        {
                            unLoadBlock.Add(blockPos);
                        }
                    }
                    var array = unLoadBlock.ToArray();
                    ThreadPool.QueueUserWorkItem((obj) =>
                    {
                        foreach (var blockPos in array)
                        {
                            scene.SaveBlockData(blockPos, true);
                            scene.UnloadBlockData(blockPos);
                        }
                    });
                    unLoadBlock.Clear();
                }
            }
        }
        public void TimeLateUpdate()
        {
            BlockMaterial.Update();
        }
    }
}