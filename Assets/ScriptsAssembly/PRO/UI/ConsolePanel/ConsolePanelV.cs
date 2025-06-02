using PRO;
using PRO.Tool;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PRO.Console
{
    internal class ConsolePanelV : UIViewBase
    {
        public Button CloseButton { get; private set; }
        public Button ClearButton { get; private set; }
        public Button TimeButton { get; private set; }
        public TMP_InputField InputField { get; private set; }
        public Transform OneLogPrefab { get; private set; }
        public ScrollRect ScrollRect { get; private set; }

        public override void Init(UIControllerBase controller)
        {
            base.Init(controller);

            CloseButton = transform.Find("TopInfo/CloseButton").GetComponent<Button>();
            ClearButton = transform.Find("TopInfo/ClearButton").GetComponent<Button>();
            TimeButton = transform.Find("TopInfo/TimeButton").GetComponent<Button>();
            InputField = transform.Find("Content/InputField").GetComponent<TMP_InputField>();
            OneLogPrefab = transform.Find("Content/LogPanel/Viewport/Content/OneLog");
            ScrollRect = transform.Find("Content/LogPanel").GetComponent<ScrollRect>();
        }

        public class OneLog
        {
            public Transform transform;
            public TMP_Text value;
            public string time;
            public string content;

            public void Init(Transform transform)
            {
                this.transform = transform;
                this.value = transform.GetComponent<TMP_Text>();
            }

            public static GameObjectPool<OneLog> pool;
            public static Queue<OneLog> queue = new Queue<OneLog>();

            public static void InitPool(GameObject prefab, ConsolePanelC panel)
            {
                pool = new GameObjectPool<OneLog>(prefab, panel.transform);
                pool.CreateEventT += (g, t) =>
                {
                    t.Init(g.transform);
                };
                pool.PutInEventT += (g, t) =>
                {
                    t.value.text = null;
                    t.time = null;
                    t.content = null;
                };
                pool.TakeOutEventT += (g, t) =>
                {
                    queue.Enqueue(t);
                    t.transform.parent = prefab.transform.parent;
                };
            }
        }
    }
}
