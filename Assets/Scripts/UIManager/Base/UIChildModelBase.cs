using UnityEngine;

namespace PRO
{
    public class UIChildModelBase
    {
        protected Transform transform;
        public virtual void Init(Transform transform)
        {
            this.transform = transform;
        }
    }
}
