using System.Collections.Generic;
using System;
using UnityEngine;
using Cysharp.Threading.Tasks;

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
        /// 解析出来的数据的弱引用
        /// </summary>
        private Dictionary<string, WeakReference> counter = new Dictionary<string, WeakReference>();

        /// <summary>
        /// 从这个包中解析一个资源，异步
        /// </summary>
        /// <returns></returns>
        public async UniTask<T> AnalysisAsync<T>(string name) where T : UnityEngine.Object
        {
            if (counter.TryGetValue(name, out WeakReference weak))
            {
                if (weak.Target == null)
                {
                    counter.Remove(name);
                    return await AnalysisAsync<T>(name);
                }
                return weak.Target as T;
            }
            else
            {
                //加载包并存储弱引用
                AssetBundleRequest request = ab.LoadAssetAsync<T>(name);
                T asset = await request.ToUniTask() as T;
                WeakReference cite = new WeakReference(asset);
                counter.Add(name, cite);
                return asset;
            }
        }

        /// <summary>
        /// 检查ab包使用情况，是否可以销毁
        /// </summary>
        /// <returns></returns>
        public bool CheckUnload()
        {
            bool canUnload = true;

            foreach (WeakReference value in counter.Values)
            {
                if (value.Target == null)
                {
                    //加载的资源不存在
                    //不做任何操作，当所有的资源不存在则canUnload仍未true
                }
                else
                {
                    //资源存在
                    canUnload = false;
                }
            }
            if (CitedBy == 0 && canUnload)
                canUnload = true;
            else
                canUnload = false;
            //加载的所有资源已经被CG且本资源包被引用计数为0
            return canUnload;
        }

        /// <summary>
        /// 被引用计数器
        /// 当被其他包引用时++
        /// 当其被引用的包卸载时--
        /// </summary>
        public int CitedBy { get; set; }

        public void Unload()
        {
            ab.Unload(true);
        }
    }
}
