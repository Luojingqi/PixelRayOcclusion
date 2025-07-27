using Cysharp.Threading.Tasks;
using PRO.Disk.Scene;
using PRO.Tool;
using PROTool;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
namespace PRO
{

    public class SceneManager : MonoScriptBase, ITime_Awake, ITime_Start, ITime_Update, ITime_LateUpdate
    {

        public static SceneManager Inst;


        private SceneEntity nowScene;
        [ShowInInspector]
        public SceneEntity NowScene => nowScene;
        [ShowInInspector]
        private Dictionary<string, SceneEntity> scenes = new Dictionary<string, SceneEntity>();

        public SceneEntity GetScene(string sceneName) => scenes[sceneName];

        public void SwitchScene(SceneEntity scene)
        {
            BlockMaterial.DrawApplyQueue.Clear();
            if (nowScene != null)
            {
                nowScene.gameObject.SetActive(false);
                nowScene.sceneCatalog.cameraPos = Camera.main.transform.position;
                nowScene.sceneCatalog.mainRound = GamePlayMain.Inst.Round?.GUID;
                BlockMaterial.ClearBind(nowScene);
            }

            if (scene != null)
            {
                scene.gameObject.SetActive(true);
                Camera.main.transform.position = scene.sceneCatalog.cameraPos;
                if (scene.sceneCatalog.mainRound != null)
                    GamePlayMain.Inst.Round = scene.ActiveRound[scene.sceneCatalog.mainRound];
                BlockMaterial.Bind(scene);
            }

            nowScene = scene;
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
            SceneEntity.InitPool();
        }
        public void TimeStart()
        {
            BuildingBase.InitBuildingType();
#if PRO_MCTS_SERVER
            //GameSaveManager.Inst.CreateGameSaveFile("testSave");
            //加载所有的存档目录
            var catalogList = GameSaveCatalog.LoadAllSaveCatalog();
            catalogList.ForEach(item => Log.Print(item.name));
            GameSaveCatalog nowSave = catalogList.Find((info) => info.name == "testSave");            
            SceneCatalog sceneCatalog = nowSave.sceneCatalogDic[nowSave.sceneNameList[0]];//选择存档的第一个场景
            //转换为实体数据
            SceneEntity scene = SceneEntity.TakeOut(sceneCatalog);
            scenes.Add(nowSave.sceneNameList[0], scene);

            var countEvent = scene.LoadAll(sceneCatalog);
            ThreadPool.QueueUserWorkItem((obj) =>
            {
                countEvent.Wait();
                TimeManager.Inst.AddToQueue_MainThreadUpdate_Clear(() =>
                {
                    SwitchScene(scene);
#if PRO_RENDER
                    source = FreelyLightSource.New(nowScene, Pixel.GetPixelColorInfo("鼠标光源0").color, 20);
#endif
                });
            });
#endif
        }

        public FreelyLightSource source;
        public Transform PoolNode;

        public void TimeUpdate()
        {
#if PRO_RENDER
            MousePoint.Update(nowScene);
            BlockMaterial.Update(nowScene);
            if (source != null) source.GloabPos = MousePoint.globalPos;
#endif
            nowScene?.TimeUpdate();
        }
        public void TimeLateUpdate()
        {
#if PRO_RENDER
            BlockMaterial.LastUpdate(nowScene);
#endif
        }
    }
}