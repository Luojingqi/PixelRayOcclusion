using UnityEngine;

namespace PRO
{
    public class UIChildViewBase
    {
        public RectTransform rectTransform { get; private set; }
        protected Transform transform;
        public virtual void Init(UIChildControllerBase controller)
        {
            this.transform = controller.transform;
            rectTransform = transform.GetComponent<RectTransform>();
        }
    }
}
