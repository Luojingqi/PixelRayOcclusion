using PRO.Tool;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace PRO
{
    public class LogPanelM
    {
        public class OneLog : IGameObjectPool_NoMono
        {
            public Transform transform { get; set; }
            public GameObject gameObject { get; set; }

            public TMP_Text value;
            public void Init()
            {
                value = transform.GetComponent<TMP_Text>();
                value.text = null;
            }

            public static GameObjectPool_NoMono<OneLog> pool;
            public static Queue<OneLog> oneLogQueue = new Queue<OneLog>();

            public static void InitPool(GameObject prefab, Transform panelPutIn)
            {
                var parent = prefab.transform.parent;
                pool = new GameObjectPool_NoMono<OneLog>(prefab, panelPutIn);
                pool.PutInEvent += t =>
                {
                    t.value.text = null;
                };
                pool.TakeOutEvent += t =>
                {
                    oneLogQueue.Enqueue(t);
                    t.transform.SetParent(parent);
                };
            }
        }
    }
}
