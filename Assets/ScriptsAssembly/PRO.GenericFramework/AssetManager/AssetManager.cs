using Cysharp.Threading.Tasks;
using Newtonsoft.Json.Linq;
using PRO.Tool.Serialize.IO;
using PRO.Tool.Serialize.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace PRO.Tool
{
    public static class AssetManager
    {
        private static string SaveABPath;
        private static AssetBundleManifest MainManifest;
        private static string excelToolSaveJsonPath;
        public static string ExcelToolSaveJsonPath
        {
            get
            {
                if (excelToolSaveJsonPath != null) return excelToolSaveJsonPath;
                else
                {
                    string excelToolPath = Application.streamingAssetsPath + @"\Excel\ExcelTool\";
                    IOTool.LoadText(excelToolPath + "path.json", out string pathText);
                    excelToolSaveJsonPath = excelToolPath + (string)JsonTool.ToObject<JObject>(pathText)["jsonPath"];
                    return excelToolSaveJsonPath;
                }
            }
        }

        /// <summary>
        /// 记录当前已经加载了的包
        /// </summary>
        private static Dictionary<string, AssetBundleEntity> bundleDic = new Dictionary<string, AssetBundleEntity>();
        /// <summary>
        /// 已经加载了的resource资源
        /// </summary>
        private static Dictionary<string, WeakReference> resourceDic = new Dictionary<string, WeakReference>();

        /// <summary>
        /// 加载一个ab包以及它的依赖
        /// </summary>
        /// <param name="bundlePath">包名</param>
        /// <returns></returns>
        private static async UniTask<AssetBundleEntity> LoadBundleAsync(string bundlePath, bool CitedBy = false)
        {
            if (MainManifest == null)
            {
                SaveABPath = Application.streamingAssetsPath + $@"\AssetBundle\";
                AssetBundle assetBundleMainManifest = await AssetBundle.LoadFromFileAsync(SaveABPath + "AssetBundle");
                MainManifest = assetBundleMainManifest.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                Debug.Log("加载AssetBundleManifest");
            }

            //获取包名
            if (bundleDic.ContainsKey(bundlePath) == false)
            {
                //获取准备加载包的依赖包名
                string[] dependencies = MainManifest.GetAllDependencies(bundlePath);
                for (int i = 0; i < dependencies.Length; i++)
                {
                    //递归加载依赖
                    await LoadBundleAsync(dependencies[i], true);
                }
                //记录并返回
                AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(SaveABPath + bundlePath);
                AssetBundle ab = await request.ToUniTask();
                Log.Print($"加载ab资源包： ab = {bundlePath}", ab != null ? new Color32(146, 208, 80, 0) : new Color32(255, 0, 0, 0));
                AssetBundleEntity bundle = new AssetBundleEntity(ab);
                if (CitedBy)
                    bundle.CitedBy++;
                bundleDic.Add(bundlePath, bundle);
                return bundle;
            }
            else
            {
                return bundleDic[bundlePath];
            }
        }

        /// <summary>
        /// 加载一个ab包以及它的依赖
        /// </summary>
        /// <param name="bundlePath">包名</param>
        /// <returns></returns>
        private static AssetBundleEntity LoadBundle(string bundlePath, bool CitedBy = false)
        {
            if (MainManifest == null)
            {
                SaveABPath = Application.streamingAssetsPath + $@"\AssetBundle\";
                AssetBundle assetBundleMainManifest = AssetBundle.LoadFromFile(SaveABPath + "AssetBundle");
                MainManifest = assetBundleMainManifest.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                Debug.Log("加载AssetBundleManifest");
            }
            if (bundleDic.ContainsKey(bundlePath) == false)
            {
                //获取准备加载包的依赖包名
                string[] dependencies = MainManifest.GetAllDependencies(bundlePath);
                for (int i = 0; i < dependencies.Length; i++)
                {
                    //递归加载依赖
                    LoadBundle(dependencies[i], true);
                }
                //记录并返回
                AssetBundle ab = AssetBundle.LoadFromFile(SaveABPath + bundlePath);
                Log.Print($"加载ab资源包： ab = {bundlePath}", ab != null ? new Color32(146, 208, 80, 0) : new Color32(255, 0, 0, 0));
                AssetBundleEntity bundle = new AssetBundleEntity(ab);
                if (CitedBy)
                    bundle.CitedBy++;
                bundleDic.Add(bundlePath, bundle);
                return bundle;
            }
            else
            {
                return bundleDic[bundlePath];
            }
        }

        /// <summary>
        /// 加载一个包中的资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bundleName">包名，需要包含后缀</param>
        /// <param name="analysisPath">资源路径</param>
        /// <returns></returns>
        public static async UniTask<T> LoadAsync_A<T>(string bundleName, string analysisPath) where T : UnityEngine.Object
        {
            analysisPath = @"Assets/" + analysisPath;
            switch (typeof(T).Name)
            {
                case nameof(GameObject): analysisPath += ".prefab"; break;
                case nameof(Material): analysisPath += ".mat"; break;
            }
            AssetBundleEntity bundle = await LoadBundleAsync(bundleName);
            T asset = await bundle.AnalysisAsync<T>(analysisPath);
            return asset;
        }
        /// <summary>
        /// 加载一个包中的资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bundleName">包名，需要包含后缀</param>
        /// <param name="analysisPath">资源路径</param>
        /// <returns></returns>
        public static T Load_A<T>(string bundleName, string analysisPath) where T : UnityEngine.Object
        {
            analysisPath = @"Assets/" + analysisPath;
            switch (typeof(T).Name)
            {
                case nameof(GameObject): analysisPath += ".prefab"; break;
                case nameof(Material): analysisPath += ".mat"; break;
            }
            AssetBundleEntity bundle = LoadBundle(bundleName);
            T asset = bundle.Analysis<T>(analysisPath);
            return asset;
        }
        /// <summary>
        /// 通过Resource加载资源 (不建议)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resourcePath"></param>
        /// <returns></returns>
        public static async UniTask<T> LoadAsync_R<T>(string resourcePath) where T : UnityEngine.Object
        {
            if (resourceDic.TryGetValue(resourcePath, out WeakReference weak))
            {
                if (weak.Target == null)
                {
                    resourceDic.Remove(resourcePath);
                    return await LoadAsync_R<T>(resourcePath);
                }
                return weak.Target as T;
            }
            else
            {
                ResourceRequest request = Resources.LoadAsync<T>(resourcePath);
                T asset = await request.ToUniTask() as T;
                Log.Print($"加载Resource资源： path = {resourcePath}", asset != null ? new Color32(146, 208, 80, 0) : new Color32(255, 0, 0, 0));
                WeakReference cite = new WeakReference(asset);
                resourceDic.Add(resourcePath, cite);
                return asset;
            }
        }
        /// <summary>
        /// 尝试卸载一个包（如果此包没有被其他包引用）（卸载包包括解析的资源）
        /// </summary>
        /// <param name="bundleName"></param>
        /// <returns></returns>
        public static async UniTask<bool> TryUnload_A(string bundleName)
        {
            if (bundleDic.TryGetValue(bundleName, out AssetBundleEntity bundle))
            {
                if (bundle.CitedBy == 0)
                {
                    bundleDic.Remove(bundleName);
                    await bundle.Unload();
                    Log.Print("卸载ab包" + bundleName, Color.yellow);
                    //获取当前包引用的包
                    string[] CitedByBundleNameArray = MainManifest.GetAllDependencies(bundleName);
                    foreach (string name in CitedByBundleNameArray)
                        if (bundleDic.TryGetValue(name, out AssetBundleEntity citedByBundle))
                            citedByBundle.CitedBy--;
                    return true;
                }
            }
            return false;
        }
    }
}