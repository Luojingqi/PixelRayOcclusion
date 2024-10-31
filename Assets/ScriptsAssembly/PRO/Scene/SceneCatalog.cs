using PRO.Tool;
using System.IO;
using Unity.Plastic.Newtonsoft.Json;

namespace PRO.Disk.Scene
{
    /// <summary>
    /// 单个场景的目录类，通过此对象加载场景的实体对象
    /// </summary>
    public class SceneCatalog
    {
        public string name;
        /// <summary>
        /// 场景文件夹的目录/{GameSave}/Scene/{SceneName}
        /// </summary>
        [JsonIgnore]
        public DirectoryInfo directoryInfo;
        private SceneCatalog() { }



        /// <summary>
        /// 为当前存档创建一个新的场景目录
        /// </summary>
        /// <param name="name">场景名</param>
        /// <param name="sceneRoot">场景的根目录</param>
        /// <returns></returns>
        public static SceneCatalog CreateFile(string name, GameSaveCatalog saveInfo, DirectoryInfo sceneRoot)
        {
            SceneCatalog info = new SceneCatalog();
            info.name = name;
            DirectoryInfo sceneDirectory = Directory.CreateDirectory(@$"{sceneRoot.FullName}\{name}");
            Directory.CreateDirectory(@$"{sceneDirectory.FullName}\Block");
            JsonTool.StoreText(@$"{sceneDirectory.FullName}\SceneCatalog.json", JsonTool.ToJson(info));
            saveInfo.sceneNameList.Add(name);
            info.directoryInfo = sceneDirectory;
            return info;
        }

        /// <summary>
        /// 加载一个场景的目录文件
        /// </summary>
        /// <param name="saveDirectoryInfo">目录文件所在的文件夹</param>
        /// <returns></returns>
        public static SceneCatalog LoadSceneInfo(DirectoryInfo saveDirectoryInfo)
        {
            if (JsonTool.LoadText(@$"{saveDirectoryInfo.FullName}\SceneCatalog.json", out string sceneInfoText))
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
