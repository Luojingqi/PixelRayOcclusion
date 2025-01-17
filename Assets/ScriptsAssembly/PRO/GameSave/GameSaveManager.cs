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
        /// ��ǰ�����Ϸ�浵
        /// </summary>
        public GameSaveCatalog NowGameSave { get => nowGameSave; }
        /// <summary>
        /// �浵�ļ��ļ����뱣��·��
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
        /// ��SavePath�м������еĴ浵�ļ�Ŀ¼
        /// </summary>
        /// <returns></returns>
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
        /// ����һ���մ浵�ļ�������У�����
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
