using UnityEditor;
using UnityEngine;

public class BuildAssetBundle
{
    private static string BuildPath = Application.dataPath + @"\StreamingAssets\AssetBundle";
    // private static string DownloadURL = "http://127.0.0.1:8080/AssetBundle";
    [MenuItem("PRO/打ab包")]
    public static void Build()
    {
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
    //            CRCInfo info = new CRCInfo();
    //            info.Name = fileInfo.Name.Substring(0, fileInfo.Name.LastIndexOf('.'));
    //            info.SavePath = fileInfo.DirectoryName.Substring(BuildPath.Length, fileInfo.DirectoryName.Length - BuildPath.Length);
    //            info.DownloadURL = DownloadURL + info.SavePath  + '/'+ info.Name;
    //            using (StreamReader sr = File.OpenText(fullPath))
    //            {
    //                //数据保存
    //                sr.ReadLine();
    //                //读第二行
    //                string str = sr.ReadLine();
    //                string head = "CRC: ";
    //                info.CRC = Convert.ToUInt32(str.Substring(head.Length, str.Length - head.Length));
    //                sr.Close();
    //            }
    //            CRCInfos.Add(info);
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

