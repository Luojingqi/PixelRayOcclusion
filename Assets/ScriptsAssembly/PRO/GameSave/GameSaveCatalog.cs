using Newtonsoft.Json;
using PRO.Disk.Scene;
using PRO.Tool.Serialize.IO;
using PRO.Tool.Serialize.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PRO
{
    /// <summary>
    /// 单个游戏存档的目录类，通过此对象 加载场景的目录对象
    /// </summary>
    public class GameSaveCatalog
    {
        /// <summary>
        /// 存档名
        /// </summary>
        public string name;
        /// <summary>
        /// 上次保存时间
        /// </summary>
        public DateTime saveTime;
        /// <summary>
        /// 存档包含的场景名
        /// </summary>
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
        /// 创建一个空存档目录，包括文件夹等
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static GameSaveCatalog CreatFile(string name)
        {
            if (Directory.Exists(@$"{Application.streamingAssetsPath}\GameSaveFiles\{name}"))
            {
                Directory.Delete(@$"{Application.streamingAssetsPath}\GameSaveFiles\{name}", true);
            }
            GameSaveCatalog gameSaveCatalog = new GameSaveCatalog();
            gameSaveCatalog.name = name;
            gameSaveCatalog.saveTime = DateTime.Now;

            //创建存档的根目录
            DirectoryInfo root = Directory.CreateDirectory(@$"{Application.streamingAssetsPath}\GameSaveFiles\{name}");
            gameSaveCatalog.directoryInfo = root;
            //创建一个场景
            SceneCatalog sceneCatalog = SceneCatalog.CreateFile("mainScene", gameSaveCatalog);
            gameSaveCatalog.sceneCatalogDic.Add("mainScene", sceneCatalog);
            IOTool.SaveText(@$"{root.FullName}\GameSaveCatalog.json", JsonTool.ToJson(gameSaveCatalog));
            return gameSaveCatalog;
        }

        /// <summary>
        /// 加载一个存档的目录文件
        /// </summary>
        /// <param name="saveDirectoryInfo">存档文件夹的目录</param>
        /// <returns></returns>
        public static GameSaveCatalog LoadGameSaveInfo(DirectoryInfo saveDirectoryInfo)
        {
            if (IOTool.LoadText(@$"{saveDirectoryInfo.FullName}\GameSaveCatalog.json", out string GameSaveInfoText))
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
                            gameSaveCatalog.sceneCatalogDic.Add(sceneName, sceneCatalog);
                        }
                    }
                    return gameSaveCatalog;
                }
            }
            return null;
        }

        public static GameSaveCatalog LoadGameSaveInfo(string name) => LoadGameSaveInfo(new DirectoryInfo(@$"{Application.streamingAssetsPath}\GameSaveFiles\{name}"));


        /// <summary>
        /// 从SavePath中加载所有的存档文件目录
        /// </summary>
        /// <returns></returns>
        public static List<GameSaveCatalog> LoadAllSaveCatalog()
        {
            List<GameSaveCatalog> ret = new List<GameSaveCatalog>();
            DirectoryInfo root = new DirectoryInfo(@$"{Application.streamingAssetsPath}\GameSaveFiles\");
            DirectoryInfo[] GameSaveDirectoryArray = root.GetDirectories();
            foreach (DirectoryInfo info in GameSaveDirectoryArray)
            {
                //加载每个存档的目录文件
                GameSaveCatalog gameSaveInfo = LoadGameSaveInfo(info);
                if (gameSaveInfo != null)
                {
                    gameSaveInfo.directoryInfo = info;
                    ret.Add(gameSaveInfo);
                }
            }
            return ret;
        }
    }
}
