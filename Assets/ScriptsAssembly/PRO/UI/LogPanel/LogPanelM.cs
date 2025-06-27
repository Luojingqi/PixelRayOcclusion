using PRO.Tool;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace PRO
{
    public class LogPanelM
    {
        public class OneLog : MonoBehaviour
        {
            public Transform transform;
            public TMP_Text value;
            public void Init(Transform transform)
            {
                this.transform = transform;
                value = transform.GetComponent<TMP_Text>();
            }

            public static GameObjectPool<OneLog> pool;
            public static Queue<OneLog> oneLogQueue = new Queue<OneLog>();
            public static void InitPool(Transform prefab, Transform panelPutIn)
            {
                //pool = new GameObjectPool<OneLog>(prefab, panelPutIn);
                //pool.CreateEventT += (g, t) =>
                //{
                //    t.Init(g.transform);
                //    t.value.text = null;
                //};
                //pool.PutInEvent +=  t =>
                //{
                //    t.value.text = null;
                //};
                //pool.TakeOutEventT += (g, t) =>
                //{
                //    oneLogQueue.Enqueue(t);
                //    t.transform.parent = prefab.parent;
                //};
            }
        }
    }
}
