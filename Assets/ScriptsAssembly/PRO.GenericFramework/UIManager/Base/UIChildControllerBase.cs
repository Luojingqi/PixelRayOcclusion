using System.Collections.Generic;
using UnityEngine;
namespace PRO
{
    public abstract class UIChildControllerBase : MonoBehaviour
    {
        public abstract UIChildViewBase View { get; }

        public virtual void Init()
        {
            View.Init(this);
        }

        public List<ITime_Update> ChildUITimeUpdateList = new List<ITime_Update>();
        protected void AddChildUI(UIChildControllerBase childUI)
        {
            childUI.Init();
            if (childUI is ITime_Update i)
            {
                ChildUITimeUpdateList.Add(i);
            }
        }
        public virtual void TImeUpdate()
        {
            foreach (var childUI in ChildUITimeUpdateList)
                childUI.TimeUpdate();
        }
    }
}