using UnityEngine;
namespace PRO
{
    public abstract class UIChildControllerBase : MonoBehaviour
    {
        public abstract UIChildViewBase View { get; }
        public abstract UIChildModelBase Model { get; }

        public virtual void Init()
        {
            View.Init(transform);
            Model.Init(transform);
        }
    }
}