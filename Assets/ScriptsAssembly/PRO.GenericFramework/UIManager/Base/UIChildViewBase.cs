using UnityEngine;

namespace PRO
{
    public class UIChildViewBase
    {
        protected Transform transform;
        public virtual void Init(Transform transform)
        {
            this.transform = transform;
        }
    }
}
