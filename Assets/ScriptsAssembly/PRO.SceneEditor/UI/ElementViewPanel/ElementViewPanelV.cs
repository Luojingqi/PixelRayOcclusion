using UnityEngine;

namespace PRO.SceneEditor
{
    public class ElementViewPanelV : UIChildViewBase
    {
        public Transform Content { get; private set; }
        public ElementC Element { get; private set; }

        public override void Init(UIChildControllerBase controller)
        {
            base.Init(controller);
            Content = transform.Find("Viewport/Content");
            Element = transform.Find("Viewport/Content/Element").GetComponent<ElementC>();
        }
    }
}
