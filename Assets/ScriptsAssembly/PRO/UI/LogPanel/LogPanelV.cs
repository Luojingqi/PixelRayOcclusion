using PRO;
using UnityEngine;
using UnityEngine.UI;
using static PRO.LogPanelM;
namespace PRO
{
    public class LogPanelV : UIViewBase
    {
        public ScrollRect ScrollRect { get; private set; }
        public Transform Content { get; private set; }
        public Transform LogPrefab { get; private set; }
        public override void Init(UIControllerBase controller)
        {
            base.Init(controller);
            ScrollRect = transform.GetComponent<ScrollRect>();
            Content = transform.Find("Viewport/Content");
            LogPrefab = Content.Find("OneLog");
            LogPrefab.gameObject.SetActive(false);
        }
    }
}
