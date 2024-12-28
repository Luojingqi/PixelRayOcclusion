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
        /// ab���ļ�
        /// </summary>
        public AssetBundle ab { get; private set; }
        /// <summary>
        /// ��������������
        /// </summary>
        private Dictionary<string, UnityEngine.Object> dataDic = new Dictionary<string, UnityEngine.Object>();

        /// <summary>
        /// ��������н���һ����Դ���첽
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
                    Log.Print($"����������Դ�� ab = {ab.name}   asset = {analysisPath}", new Color32(255, 0, 0, 0));
                }
                else
                {
                    Log.Print($"����������Դ�� ab = {ab.name}   asset = {analysisPath}", new Color32(146, 208, 80, 0));
                    dataDic.Add(analysisPath, data);
                }
            }
            return data as T;
        }
        /// <summary>
        /// ��������н���һ����Դ
        /// </summary>
        /// <returns></returns>
        public T Analysis<T>(string analysisPath) where T : UnityEngine.Object
        {
            if (dataDic.TryGetValue(analysisPath, out UnityEngine.Object data) == false)
            {
                data = ab.LoadAsset<T>(analysisPath);
                if (data == null)
                {
                    Log.Print($"����������Դ�� ab = {ab.name}   asset = {analysisPath}", new Color32(255, 0, 0, 0));
                }
                else
                {
                    Log.Print($"����������Դ�� ab = {ab.name}   asset = {analysisPath}", new Color32(146, 208, 80, 0));
                    dataDic.Add(analysisPath, data);
                }
            }
            return data as T;
        }
        /// <summary>
        /// �����ü�����
        /// ��������������ʱ++
        /// ���䱻���õİ�ж��ʱ--
        /// </summary>
        public int CitedBy { get; set; }

        /// <summary>
        /// ж�ش�ab��������������������Դ��
        /// </summary>
        public async UniTask Unload()
        {
            dataDic.Clear();
            await ab.UnloadAsync(true);
        }
    }
}
