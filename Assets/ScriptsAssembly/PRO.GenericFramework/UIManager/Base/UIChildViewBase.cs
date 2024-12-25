using UnityEngine;

namespace PRO
{
    public class UIChildViewBase
    {
        public RectTransform rectTransform {  get; private set; }
        protected Transform transform;
        public virtual void Init(Transform transform)
        {
            this.transform = transform;
            rectTransform = transform.GetComponent<RectTransform>();
        }
    }
}
