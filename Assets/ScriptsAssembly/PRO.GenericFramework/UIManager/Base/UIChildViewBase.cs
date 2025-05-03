using UnityEngine;

namespace PRO
{
    public class UIChildViewBase
    {
        public RectTransform rectTransform { get; private set; }
        protected Transform transform;
        protected MonoScriptBase mono;
        public virtual void Init(MonoScriptBase mono)
        {
            this.mono = mono;
            transform = mono.transform;
            rectTransform = transform.GetComponent<RectTransform>();
        }
    }
}
