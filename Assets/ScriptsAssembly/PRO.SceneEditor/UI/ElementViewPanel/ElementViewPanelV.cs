using UnityEngine;

namespace PRO.SceneEditor
{
    internal class ElementViewPanelV : UIChildViewBase
    {
        public Transform Content { get; private set; }
        public ElementC Element { get; private set; }
        
        public override void Init(Transform transform)
        {
            base.Init(transform);


            Content = transform.Find("Viewport/Content");
            Element = transform.Find("Viewport/Content/Element").GetComponent<ElementC>();
        }
    }
}
