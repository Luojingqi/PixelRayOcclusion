using UnityEngine;

namespace PRO.SceneEditor
{
    public class ElementViewPanelV : UIChildViewBase
    {
        public Transform Content { get; private set; }
        public ElementC Element { get; private set; }

        public override void Init(MonoScriptBase mono)
        {
            base.Init(mono);
            Content = transform.Find("Viewport/Content");
            Element = transform.Find("Viewport/Content/Element").GetComponent<ElementC>();
        }
    }
}
