using System;
using System.Collections.Generic;
using UnityEngine;

namespace PRO.Tool
{
    public class MonoObjectPool<T> : ObjectPoolBase<T> where T : MonoScriptBase
    {
        private MonoScriptBase holdPrefab;
        public readonly Transform toolParent;
        public Dictionary<GameObject, T> goToTDic = new Dictionary<GameObject, T>();
        public MonoObjectPool(T prefab, Transform parent)
        {
            holdPrefab = prefab;
            toolParent = parent;
        }

        protected override T NewObject()
        {
            GameObject go = GameObject.Instantiate(holdPrefab.gameObject);
            go.transform.parent = toolParent;
            return go.GetComponent<T>();
        }

        public T TakeOutT()
        {
            T t = base.TakeOut();
            t.SetActive(true);
            return t;
        }

        public override void PutIn(T item)
        {
            base.PutIn(item);
            item.SetActive(false);
        }
    }
}
