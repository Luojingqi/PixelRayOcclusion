using UnityEngine;
namespace PRO
{
    public abstract class UIChildControllerBase : MonoScriptBase
    {
        public abstract UIChildViewBase View { get; }

        public virtual void Init()
        {
            View.Init(this);
        }
    }
}