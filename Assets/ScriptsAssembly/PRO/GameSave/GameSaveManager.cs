using PRO.Tool;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace PRO
{
    public class GameSaveManager : MonoBehaviour
    {
        public static GameSaveManager Inst { get; private set; }

        private GameSaveCatalog nowGameSave;
        public GameSaveCatalog NowGameSave { get => nowGameSave; }

        public static string SavePath;

        private void Awake()
        {
            Inst = this;
            SavePath = @$"{Application.streamingAssetsPath}\GameSaveFiles\";
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                CreateGameSaveFile("testSave");
                // nowGameSave = LoadAllSaveInfo()[0];
            }
        }

        public List<GameSaveCatalog> LoadAllSaveCatalog()
        {
            List<GameSaveCatalog> ret = new List<GameSaveCatalog>();
            DirectoryInfo root = new DirectoryInfo(SavePath);
            DirectoryInfo[] GameSaveDirectoryArray = root.GetDirectories();
            foreach (DirectoryInfo info in GameSaveDirectoryArray)
            {
                //����ÿ���浵��Ŀ¼�ļ�
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
        /// ����һ���浵�ļ�������У�����
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
