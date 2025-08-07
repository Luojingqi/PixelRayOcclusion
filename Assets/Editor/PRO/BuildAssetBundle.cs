using PRO;
using PRO.SkillEditor;
using System.IO;
using UnityEditor;
using UnityEngine;

public class BuildAssetBundle
{
    private static string BuildPath = Application.dataPath + @"\StreamingAssets\AssetBundle";
    // private static string DownloadURL = "http://127.0.0.1:8080/AssetBundle";
    [MenuItem("PRO/打ab包")]
    public static void Build()
    {
        string directoryPath = $@"Assets\{AssetManagerEX.SkillDirectoryPath}\";

        string[] guids = AssetDatabase.FindAssets($"t:{typeof(SkillVisual_Disk).Name}");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var asset = AssetDatabase.LoadAssetAtPath<SkillVisual_Disk>(path);
            asset.loadPath = path.Substring(@"Assets\".Length, path.Length - @"Assets\.asset".Length);
            EditorUtility.SetDirty(asset);
        }
        AssetDatabase.SaveAssets();



        if (!Application.isPlaying)
        {
            AssetDatabase.SaveAssets();

            DirectoryInfo directoryInfo = new DirectoryInfo(BuildPath);
            foreach (var file in directoryInfo.GetFiles())
                file.Delete();
            AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(BuildPath, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneWindows64);
        }
        #region 生成ab包的CRC，暂时弃用
        //[MenuItem("Tools/AssetBundle/UpdataCRC")]
        //public static void UpdataCRC()
        //{
        //    List<CRCInfo> CRCInfos = new List<CRCInfo>();
        //    List<string> AllfilePath = GetAllFilePath(BuildPath);
        //    foreach (string fullPath in AllfilePath)
        //    {
        //        int index = fullPath.LastIndexOf('\\');
        //        FileInfo fileInfo = new FileInfo(fullPath);
        //        if (fileInfo.Extension == ".manifest")
        //        {
        //            Debug.Log(fullPath);
        //            CRCInfo typeInfo = new CRCInfo();
        //            typeInfo.Name = fileInfo.Name.Substring(0, fileInfo.Name.LastIndexOf('.'));
        //            typeInfo.SavePath = fileInfo.DirectoryName.Substring(BuildPath.Length, fileInfo.DirectoryName.Length - BuildPath.Length);
        //            typeInfo.DownloadURL = DownloadURL + typeInfo.SavePath  + '/'+ typeInfo.Name;
        //            using (StreamReader sr = File.OpenText(fullPath))
        //            {
        //                //数据保存
        //                sr.ReadLine();
        //                //读第二行
        //                string str = sr.ReadLine();
        //                string head = "CRC: ";
        //                typeInfo.CRC = Convert.ToUInt32(str.Substring(head.Length, str.Length - head.Length));
        //                sr.Close();
        //            }
        //            CRCInfos.Add(typeInfo);
        //        }
        //    }
        //    JsonTool.StoreText(BuildPath + "/CRC.Json", JsonTool.ObjectToJson(CRCInfos));
        //}

        ///// <summary>
        ///// 获取一个文件路径下所有文件路径
        ///// </summary>
        ///// <param name="path"></param>
        ///// <returns></returns>
        //private static List<string> GetAllFilePath(string path)
        //{
        //    //绑定到指定的文件夹目录
        //    DirectoryInfo dir = new DirectoryInfo(path);

        //    //检索表示当前目录的文件和子目录
        //    FileSystemInfo[] fsinfos = dir.GetFileSystemInfos();
        //    List<string> findPath = new List<string>();
        //    //遍历检索的文件和子目录
        //    foreach (FileSystemInfo fsinfo in fsinfos)
        //    {
        //        //判断是否为文件夹　　
        //        if (fsinfo is DirectoryInfo)
        //        {
        //            //递归调用
        //            findPath.AddRange(GetAllFilePath(fsinfo.FullName));
        //        }
        //        else
        //        {
        //            //将得到的文件全路径放入到集合中
        //            findPath.Add(fsinfo.FullName);
        //        }
        //    }
        //    return findPath;
        //}

        //[MenuItem("Tools/Unload")]
        //public static void Unload()
        //{
        //    EditorUtility.UnloadUnusedAssetsImmediate();
        //}
        #endregion
    }
}


