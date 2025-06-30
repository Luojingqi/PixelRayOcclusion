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
        public GameObject OneLogPrefab { get; private set; }
        public ScrollRect ScrollRect { get; private set; }

        public override void Init(UIControllerBase controller)
        {
            base.Init(controller);

            CloseButton = transform.Find("TopInfo/CloseButton").GetComponent<Button>();
            ClearButton = transform.Find("TopInfo/ClearButton").GetComponent<Button>();
            TimeButton = transform.Find("TopInfo/TimeButton").GetComponent<Button>();
            InputField = transform.Find("Content/InputField").GetComponent<TMP_InputField>();
            OneLogPrefab = transform.Find("Content/LogPanel/Viewport/Content/OneLog").gameObject;
            ScrollRect = transform.Find("Content/LogPanel").GetComponent<ScrollRect>();
        }

        public class OneLog : IGameObjectPool_NoMono
        {
            public Transform transform { get; set; }
            public GameObject gameObject { get; set; }
            public TMP_Text value;
            public string time;
            public string content;

            public void Init()
            {
                value = transform.GetComponent<TMP_Text>();
            }

            public static GameObjectPool_NoMono<OneLog> pool;
            public static Queue<OneLog> queue = new Queue<OneLog>();


            public static void InitPool(GameObject prefab, ConsolePanelC panel)
            {
                var content = prefab.transform.parent;
                pool = new GameObjectPool_NoMono<OneLog>(prefab, panel.transform);
                pool.CreateEvent += t =>
                {
                    t.Init();
                };
                pool.PutInEvent += t =>
                {
                    t.value.text = null;
                    t.time = null;
                    t.content = null;
                };
                pool.TakeOutEvent += t =>
                {
                    queue.Enqueue(t);
                    t.transform.parent = content;
                };
            }
        }
    }
}
