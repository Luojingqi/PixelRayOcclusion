using UnityEngine;
namespace PRO
{
    public abstract class UIChildControllerBase : MonoBehaviour
    {
        public abstract UIChildViewBase View { get; }

        public virtual void Init()
        {
            View.Init(transform);
        }
    }
}