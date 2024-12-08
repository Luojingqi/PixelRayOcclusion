using Cysharp.Threading.Tasks;
using Newtonsoft.Json.Linq;
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
                    JsonTool.LoadText(excelToolPath + "path.json", out string pathText);
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
        /// <param name="bundlePath">相对路径或名字</param>
        /// <returns></returns>
        private static async UniTask<AssetBundleEntity> LoadBundle(string bundlePath, bool CitedBy = false)
        {
            if (MainManifest == null)
            {
                SaveABPath = Application.streamingAssetsPath + $@"\AB\";
                AssetBundle assetBundleMainManifest = await AssetBundle.LoadFromFileAsync(SaveABPath + "AssetBundle");
                MainManifest = assetBundleMainManifest.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                Debug.Log("加载AssetBundleManifest");
            }

            string[] strings = bundlePath.Split('\\', '/');
            string name = strings[strings.Length - 1];
            //获取包名
            if (bundleDic.ContainsKey(name) == false)
            {
                string[] dependencies = MainManifest.GetAllDependencies(name);
                for (int i = 0; i < dependencies.Length; i++)
                {
                    //递归加载依赖
                    await LoadBundle(dependencies[i], true);
                }
                //记录并返回
                AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(SaveABPath + bundlePath);
                AssetBundle ab = await request.ToUniTask();
                Log.Print($"加载ab资源包： ab = {bundlePath}", ab != null ? new Color32(146, 208, 80, 0) : new Color32(255, 0, 0, 0));
                AssetBundleEntity bundle = new AssetBundleEntity(ab);
                if (CitedBy)
                    bundle.CitedBy++;
                bundleDic.Add(name, bundle);
                return bundle;
            }
            else
            {
                return bundleDic[name];
            }
        }


        /// <summary>
        /// 加载一个包中的资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bundlePath">包路径</param>
        /// <param name="analysisPath">资源路径</param>
        /// <returns></returns>
        public static async UniTask<T> LoadAsync_A<T>(string bundlePath, string analysisPath) where T : UnityEngine.Object
        {
            analysisPath = "Assets/" + analysisPath;
            switch (typeof(T).Name)
            {
                case nameof(GameObject): analysisPath += ".prefab"; break;
                case nameof(Material): analysisPath += ".mat"; break;
            }
            AssetBundleEntity bundle = await LoadBundle(bundlePath);
            T asset = await bundle.AnalysisAsync<T>(analysisPath);
            Log.Print($"解析包内资源： ab = {bundlePath}   asset = {analysisPath}", asset != null ? new Color32(146, 208, 80, 0) : new Color32(255, 0, 0, 0));
            CheckUnload();
            return asset;
        }

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
        /// 检查所有资源包是否可以销毁
        /// </summary>
        private static void CheckUnload()
        {
            canUnload.Clear();
            foreach (var keyValue in bundleDic)
            {
                if (keyValue.Value.CheckUnload())
                {
                    Log.Print("卸载ab包" + keyValue.Key, new Color32(255, 0, 0, 0));
                    string[] CitedByBundle = MainManifest.GetAllDependencies(keyValue.Key);
                    foreach (string name in CitedByBundle)
                        if (bundleDic.TryGetValue(name, out AssetBundleEntity bundle))
                            bundle.CitedBy--;
                    canUnload.Add(keyValue.Key);
                }
            }

            foreach (var key in canUnload)
            {
                bundleDic[key].Unload();
                bundleDic.Remove(key);
            }
        }
        private static List<string> canUnload = new List<string>();
    }
}