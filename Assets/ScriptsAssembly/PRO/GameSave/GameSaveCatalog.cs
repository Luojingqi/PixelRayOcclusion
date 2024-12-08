using PRO.Disk.Scene;
using PRO.Tool;
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace PRO
{
    /// <summary>
    /// 单个游戏存档的目录类，通过此对象 加载场景的目录对象
    /// </summary>
    public class GameSaveCatalog
    {
        public string name;
        public DateTime saveTime;
        public List<string> sceneNameList = new List<string>();
        /// <summary>
        /// 路径/{GameSave}
        /// </summary>
        [JsonIgnore]
        public DirectoryInfo directoryInfo;
        /// <summary>
        /// key场景名 value场景目录
        /// </summary>
        [JsonIgnore]
        public Dictionary<string, SceneCatalog> sceneCatalogDic = new Dictionary<string, SceneCatalog>();

        private GameSaveCatalog() { }



        /// <summary>
        /// 创建一个存档目录，包括文件夹等
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static GameSaveCatalog CreatFile(string name)
        {
            GameSaveCatalog gameSaveCatalog = new GameSaveCatalog();
            gameSaveCatalog.name = name;
            gameSaveCatalog.saveTime = DateTime.Now;

            //创建存档的根目录
            DirectoryInfo root = Directory.CreateDirectory(GameSaveManager.SavePath + name);
            gameSaveCatalog.directoryInfo = root;
            //创建场景的根目录
            DirectoryInfo sceneRoot = Directory.CreateDirectory(@$"{root.FullName}\Scene");
            //创建一个场景
            SceneCatalog sceneCatalog = SceneCatalog.CreateFile("mainScene", gameSaveCatalog, sceneRoot);
            gameSaveCatalog.sceneCatalogDic.Add(sceneCatalog.name, sceneCatalog);
            JsonTool.StoreText(@$"{root.FullName}\GameSaveCatalog.json", JsonTool.ToJson(gameSaveCatalog));
            return gameSaveCatalog;
        }

        /// <summary>
        /// 加载一个存档的目录文件
        /// </summary>
        /// <param name="saveDirectoryInfo">存档文件夹的目录</param>
        /// <returns></returns>
        public static GameSaveCatalog LoadGameSaveInfo(DirectoryInfo saveDirectoryInfo)
        {
            if (JsonTool.LoadText(@$"{saveDirectoryInfo.FullName}\GameSaveCatalog.json", out string GameSaveInfoText))
            {
                //加载存档的目录文件
                GameSaveCatalog gameSaveCatalog = JsonTool.ToObject<GameSaveCatalog>(GameSaveInfoText);
                if (gameSaveCatalog != null)
                {
                    gameSaveCatalog.directoryInfo = saveDirectoryInfo;
                    foreach (var sceneName in gameSaveCatalog.sceneNameList)
                    {
                        SceneCatalog sceneCatalog = SceneCatalog.LoadSceneInfo(new DirectoryInfo(@$"{saveDirectoryInfo.FullName}\Scene\{sceneName}"));
                        if (sceneCatalog != null)
                        {
                            gameSaveCatalog.sceneCatalogDic.Add(sceneCatalog.name, sceneCatalog);
                        }
                    }
                    return gameSaveCatalog;
                }
            }
            return null;
        }
    }
}
