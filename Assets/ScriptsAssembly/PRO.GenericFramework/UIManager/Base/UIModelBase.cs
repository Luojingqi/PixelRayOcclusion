using UnityEngine;
namespace PRO
{
    public class UIModelBase
    {
        public Transform transform { get; set; }


        public virtual void Init(Transform transform)
        {
            this.transform = transform;
        }
    }
}