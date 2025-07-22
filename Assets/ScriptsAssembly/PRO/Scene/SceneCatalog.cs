using Newtonsoft.Json;
using PRO.Tool.Serialize.IO;
using PRO.Tool.Serialize.Json;
using System.IO;
using UnityEngine;

namespace PRO.Disk.Scene
{
    /// <summary>
    /// 单个场景的目录类，通过此对象加载场景的实体对象
    /// </summary>
    public class SceneCatalog
    {
        /// <summary>
        /// 场景文件夹的目录/{GameSave}/Scene/{SceneName}
        /// </summary>
        [JsonIgnore]
        public DirectoryInfo directoryInfo;
        private SceneCatalog() { }

        public string name;

        public Vector2 cameraPos;

        public void Save()
        {
            IOTool.SaveText(@$"{directoryInfo}\SceneCatalog.json", JsonTool.ToJson(this));
        }


        /// <summary>
        /// 为当前存档创建一个新的场景目录
        /// </summary>
        /// <param name="name">场景名</param>
        /// <param name="sceneRoot">场景的根目录</param>
        /// <returns></returns>
        public static SceneCatalog CreateFile(string name, GameSaveCatalog saveInfo)
        {
            SceneCatalog info = new SceneCatalog();
            //创建场景根文件夹
            DirectoryInfo sceneDirectory = Directory.CreateDirectory(@$"{saveInfo.directoryInfo}\Scene\{name}");
            //创建块文件夹
            Directory.CreateDirectory(@$"{sceneDirectory}\Block");
            //创建建筑文件夹
            Directory.CreateDirectory(@$"{sceneDirectory}\Building");
            //创建角色文件夹
            Directory.CreateDirectory(@$"{sceneDirectory}\Role");
            //创建战斗文件夹
            Directory.CreateDirectory(@$"{sceneDirectory}\Round");

            

            saveInfo.sceneNameList.Add(name);
            info.directoryInfo = sceneDirectory;
            info.name = name;
            info.Save();
            return info;
        }

        /// <summary>
        /// 加载一个场景的目录文件
        /// </summary>
        /// <param name="saveDirectoryInfo">目录文件所在的文件夹</param>
        /// <returns></returns>
        public static SceneCatalog LoadSceneInfo(DirectoryInfo saveDirectoryInfo)
        {
            if (IOTool.LoadText(@$"{saveDirectoryInfo}\SceneCatalog.json", out string sceneInfoText))
            {
                SceneCatalog sceneCatalog = JsonTool.ToObject<SceneCatalog>(sceneInfoText);
                if (sceneCatalog != null)
                {
                    sceneCatalog.directoryInfo = saveDirectoryInfo;
                    return sceneCatalog;
                }
            }
            return null;
        }
    }
}
