using System;
using System.Collections.Generic;
using UnityEngine;
namespace PRO.Tool
{
    public class GameObjectPool<T> : ObjectPoolBase<GameObject>
    {
        private GameObject holdPrefab;
        public readonly Transform toolParent;
        public Dictionary<GameObject, T> goToTDic = new Dictionary<GameObject, T>();
        public GameObjectPool(GameObject prefab, Transform parent)
        {
            holdPrefab = prefab;
            toolParent = parent;

            base.CreateEvent += CreateAction;
            base.PutInEvent += PutInAction;
            base.TakeOutEvent += TakeOutAction;
            Type type = typeof(T);
            if (type.IsClass)
                canGet = type.IsSubclassOf(typeof(Component));
            else if (type.IsInterface)
                canGet = true;
            else
                canGet = false;
        }
        /// <summary>
        /// 是否可以直接在unity中Get组件
        /// </summary>
        bool canGet;
        protected override GameObject NewObject()
        {
            GameObject go = null;
            if (holdPrefab != null)
                go = GameObject.Instantiate(holdPrefab);
            if (canGet)
                goToTDic.Add(go, go.GetComponent<T>());
            else
                goToTDic.Add(go, (T)Activator.CreateInstance(typeof(T)));
            return go;
        }

        public T TakeOutT()
        {
            GameObject go = base.TakeOut();
            return goToTDic[go];
        }

        public override void Clear()
        {
            base.Clear();
            goToTDic.Clear();
        }

        public event Action<GameObject, T> CreateEventT;
        public event Action<GameObject, T> PutInEventT;
        public event Action<GameObject, T> TakeOutEventT;

        private void CreateAction(GameObject go)
        {
            go.transform.SetParent(toolParent);
            CreateEventT?.Invoke(go, goToTDic[go]);
        }

        private void PutInAction(GameObject go)
        {
            go.transform.SetParent(toolParent);
            go.SetActive(false);
            PutInEventT?.Invoke(go, goToTDic[go]);
        }

        private void TakeOutAction(GameObject go)
        {
            go.SetActive(true);
            TakeOutEventT?.Invoke(go, goToTDic[go]);
        }
    }
}