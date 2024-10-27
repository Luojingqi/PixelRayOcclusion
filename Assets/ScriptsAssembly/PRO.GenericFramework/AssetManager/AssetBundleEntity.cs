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
        /// ab���ļ�
        /// </summary>
        public AssetBundle ab { get; private set; }
        /// <summary>
        /// �������������ݵ�������
        /// </summary>
        private Dictionary<string, WeakReference> counter = new Dictionary<string, WeakReference>();

        /// <summary>
        /// ��������н���һ����Դ���첽
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
                //���ذ����洢������
                AssetBundleRequest request = ab.LoadAssetAsync<T>(name);
                T asset = await request.ToUniTask() as T;
                WeakReference cite = new WeakReference(asset);
                counter.Add(name, cite);
                return asset;
            }
        }

        /// <summary>
        /// ���ab��ʹ��������Ƿ��������
        /// </summary>
        /// <returns></returns>
        public bool CheckUnload()
        {
            bool canUnload = true;

            foreach (WeakReference value in counter.Values)
            {
                if (value.Target == null)
                {
                    //���ص���Դ������
                    //�����κβ����������е���Դ��������canUnload��δtrue
                }
                else
                {
                    //��Դ����
                    canUnload = false;
                }
            }
            if (CitedBy == 0 && canUnload)
                canUnload = true;
            else
                canUnload = false;
            //���ص�������Դ�Ѿ���CG�ұ���Դ�������ü���Ϊ0
            return canUnload;
        }

        /// <summary>
        /// �����ü�����
        /// ��������������ʱ++
        /// ���䱻���õİ�ж��ʱ--
        /// </summary>
        public int CitedBy { get; set; }

        public void Unload()
        {
            ab.Unload(true);
        }
    }
}
