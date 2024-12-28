using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace PRO.Tool
{
    public class AssetBundleEntity
    {
        public AssetBundleEntity(AssetBundle bundle)
        {
            this.ab = bundle;
        }

        /// <summary>
        /// ab包文件
        /// </summary>
        public AssetBundle ab { get; private set; }
        /// <summary>
        /// 解析出来的数据
        /// </summary>
        private Dictionary<string, UnityEngine.Object> dataDic = new Dictionary<string, UnityEngine.Object>();

        /// <summary>
        /// 从这个包中解析一个资源，异步
        /// </summary>
        /// <returns></returns>
        public async UniTask<T> AnalysisAsync<T>(string analysisPath) where T : UnityEngine.Object
        {
            if (dataDic.TryGetValue(analysisPath, out UnityEngine.Object data) == false)
            {
                AssetBundleRequest request = ab.LoadAssetAsync<T>(analysisPath);
                data = await request.ToUniTask();
                if (data == null)
                {
                    Log.Print($"解析包内资源： ab = {ab.name}   asset = {analysisPath}", new Color32(255, 0, 0, 0));
                }
                else
                {
                    Log.Print($"解析包内资源： ab = {ab.name}   asset = {analysisPath}", new Color32(146, 208, 80, 0));
                    dataDic.Add(analysisPath, data);
                }
            }
            return data as T;
        }
        /// <summary>
        /// 从这个包中解析一个资源
        /// </summary>
        /// <returns></returns>
        public T Analysis<T>(string analysisPath) where T : UnityEngine.Object
        {
            if (dataDic.TryGetValue(analysisPath, out UnityEngine.Object data) == false)
            {
                data = ab.LoadAsset<T>(analysisPath);
                if (data == null)
                {
                    Log.Print($"解析包内资源： ab = {ab.name}   asset = {analysisPath}", new Color32(255, 0, 0, 0));
                }
                else
                {
                    Log.Print($"解析包内资源： ab = {ab.name}   asset = {analysisPath}", new Color32(146, 208, 80, 0));
                    dataDic.Add(analysisPath, data);
                }
            }
            return data as T;
        }
        /// <summary>
        /// 被引用计数器
        /// 当被其他包引用时++
        /// 当其被引用的包卸载时--
        /// </summary>
        public int CitedBy { get; set; }

        /// <summary>
        /// 卸载此ab包（包括解析出来的资源）
        /// </summary>
        public async UniTask Unload()
        {
            dataDic.Clear();
            await ab.UnloadAsync(true);
        }
    }
}
