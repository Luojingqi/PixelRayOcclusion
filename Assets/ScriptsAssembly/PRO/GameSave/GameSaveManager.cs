using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace PRO
{
    public class GameSaveManager : MonoBehaviour
    {
        public static GameSaveManager Inst { get; private set; }

        private GameSaveCatalog nowGameSave;
        /// <summary>
        /// 当前活动的游戏存档
        /// </summary>
        public GameSaveCatalog NowGameSave { get => nowGameSave; }
        /// <summary>
        /// 存档文件的加载与保存路径
        /// </summary>
        public static string SavePath;

        private void Awake()
        {
            Inst = this;
            SavePath = @$"{Application.streamingAssetsPath}\GameSaveFiles\";
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                CreateGameSaveFile("testSave");
                // nowGameSave = LoadAllSaveInfo()[0];
            }
        }
        /// <summary>
        /// 从SavePath中加载所有的存档文件目录
        /// </summary>
        /// <returns></returns>
        public List<GameSaveCatalog> LoadAllSaveCatalog()
        {
            List<GameSaveCatalog> ret = new List<GameSaveCatalog>();
            DirectoryInfo root = new DirectoryInfo(SavePath);
            DirectoryInfo[] GameSaveDirectoryArray = root.GetDirectories();
            foreach (DirectoryInfo info in GameSaveDirectoryArray)
            {
                //加载每个存档的目录文件
                GameSaveCatalog gameSaveInfo = GameSaveCatalog.LoadGameSaveInfo(info);
                if (gameSaveInfo != null)
                {
                    gameSaveInfo.directoryInfo = info;
                    ret.Add(gameSaveInfo);
                }
            }
            return ret;
        }

        public void SwitchSave(GameSaveCatalog catalog)
        {
            nowGameSave = catalog;
        }

        /// <summary>
        /// 创建一个空存档文件，如果有，覆盖
        /// </summary>
        /// <param name="saveName"></param>
        /// <returns></returns>
        public GameSaveCatalog CreateGameSaveFile(string saveName)
        {
            if (Directory.Exists(SavePath + saveName))
            {
                Directory.Delete(SavePath + saveName, true);
            }
            GameSaveCatalog gameSave = GameSaveCatalog.CreatFile(saveName);


            return gameSave;
        }

    }
}
