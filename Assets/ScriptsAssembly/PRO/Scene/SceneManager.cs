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
        public SceneEntity NowScene { get => nowScene; set => nowScene = value; }
        [ShowInInspector]
        private Dictionary<string, SceneEntity> scenes = new Dictionary<string, SceneEntity>();

        public SceneEntity GetScene(string sceneName) => scenes[sceneName];

        public void SwitchScene(string toSceneName)
        {
            // if (toSceneName == nowScene.name) return;
            //SceneEntity toSceen = new SceneEntity("123");
            //DrawThread.InitScene(toSceen);
            //await UniTask.Delay(2500);
            //Log.Print("切换场景");
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
#if PRO_MCTS_SERVER
            //GameSaveManager.Inst.CreateGameSaveFile("testSave");
            //加载所有的存档目录
            var catalogList = GameSaveCatalog.LoadAllSaveCatalog();
            catalogList.ForEach(item => Log.Print(item.name));
            GameSaveCatalog nowSave = catalogList.Find((info) => info.name == "testSave");            //选择存档的第一个场景
            SceneCatalog sceneCatalog = nowSave.sceneCatalogDic[nowSave.sceneNameList[0]];
            //转换为实体数据
            SceneEntity scene = SceneEntity.TakeOut(sceneCatalog);
            scenes.Add(nowSave.sceneNameList[0], scene);
            nowScene = scene;

            scene.LoadAll();
#endif

#if PRO_RENDER
            source = FreelyLightSource.New(NowScene, Pixel.GetPixelColorInfo("鼠标光源0").color, 20);
#endif
        }
        public void SwitchScene(SceneCatalog sceneCatalog)
        {
            
        }

        public FreelyLightSource source;
        public Transform PoolNode;

        public void TimeUpdate()
        {
#if PRO_RENDER
            MousePoint.Update();
            BlockMaterial.Update();
            if (source != null) source.GloabPos = MousePoint.globalPos;
#endif
            NowScene?.TimeUpdate();
        }
        public void TimeLateUpdate()
        {
#if PRO_RENDER
            BlockMaterial.LastUpdate();
#endif
        }
    }
}