using Cysharp.Threading.Tasks;
using PRO.Disk.Scene;
using PRO.Tool;
using PROTool;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
namespace PRO
{

    public class SceneManager : MonoScriptBase, ITime_Awake, ITime_Start, ITime_Update, ITime_LateUpdate
    {

        public static SceneManager Inst;


        private SceneEntity nowScene;
        [ShowInInspector]
        public SceneEntity NowScene { get => nowScene; }
        [ShowInInspector]
        private Dictionary<string, SceneEntity> scenes = new Dictionary<string, SceneEntity>();

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
            DontDestroyOnLoad(PoolNode);
            BlockMaterial.Init();
            Pixel.LoadPixelTypeInfo();
            GreedyCollider.InitBoxCollider2DPool();
            Block.InitPool();
            BackgroundBlock.InitPrefab();
            Texture2DPool.InitPool();
        }
        public void TimeStart()
        {
            BuildingBase.InitBuildingType();
            //GameSaveManager.Inst.CreateGameSaveFile("testSave");
            //�������еĴ浵Ŀ¼
            var catalogList = GameSaveCatalog.LoadAllSaveCatalog();
            catalogList.ForEach(item => Log.Print(item.name));
            GameSaveCatalog nowSave = catalogList[0];
            //ѡ��浵�ĵ�һ������
            SceneCatalog sceneCatalog = nowSave.sceneCatalogDic[nowSave.sceneNameList[0]];
            //ת��Ϊʵ������
            SceneEntity scene = new SceneEntity(sceneCatalog);
            scenes.Add(nowSave.sceneNameList[0], scene);
            nowScene = scene;

            scene.LoadAll();

#if !PRO_MCTS
            source = FreelyLightSource.New(NowScene, Pixel.GetPixelColorInfo("����Դ0").color, 20);
#endif
        }

        public FreelyLightSource source;
        public Transform PoolNode;

        public void TimeUpdate()
        {
#if !PRO_MCTS
            MousePoint.Update();
            BlockMaterial.Update();
            if (source != null) source.GloabPos = MousePoint.globalPos;
#endif
            NowScene?.TimeUpdate();
        }
        public void TimeLateUpdate()
        {
#if !PRO_MCTS
            BlockMaterial.LastUpdate();
#endif
        }
    }
}