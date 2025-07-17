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
            BackgroundBlock.InitPool();
            Texture2DPool.InitPool();
        }
        public void TimeStart()
        {
            BuildingBase.InitBuildingType();
            //GameSaveManager.Inst.CreateGameSaveFile("testSave");
            //加载所有的存档目录
            var catalogList = GameSaveCatalog.LoadAllSaveCatalog();
            catalogList.ForEach(item => Log.Print(item.name));
            GameSaveCatalog nowSave = catalogList[0];
            //选择存档的第一个场景
            SceneCatalog sceneCatalog = nowSave.sceneCatalogDic[nowSave.sceneNameList[0]];
            //转换为实体数据
            SceneEntity scene = new SceneEntity(sceneCatalog);
            scenes.Add(nowSave.sceneNameList[0], scene);
            nowScene = scene;

            D(scene);


            source = FreelyLightSource.New(NowScene, BlockMaterial.GetPixelColorInfo("鼠标光源0").color, 20);
        }
        private async UniTask D(SceneEntity scene)
        {
            var countDown = scene.LoadAll();
            await UniTask.WaitUntil(() => countDown.IsSet);
            //foreach (var round in scene.ActiveRound.Values)
            //{
            //    GamePlayMain.Inst.Round = round;
            //    break;
            //}
        }

        public FreelyLightSource source;
        public Transform PoolNode;

        public void TimeUpdate()
        {
            MousePoint.Update();
            BlockMaterial.Update();
            if (source != null) source.GloabPos = MousePoint.globalPos;
            NowScene.TimeUpdate();
        }
        public void TimeLateUpdate()
        {
            BlockMaterial.LastUpdate();
        }
    }
}