using System;
using UnityEngine;
namespace PRO
{
    public abstract class UIControllerBase : MonoBehaviour
    {

        public abstract UIViewBase View { get; }
        public abstract UIModelBase Model { get; }

        public string UIName;

        //public LanguageManager.LanguageEnum NowUseLanguage { get; set; }




        /// <summary>
        /// Ui初始化值调用一次
        /// </summary>
        public virtual void Init()
        {
            View.Init(transform);
            Model.Init(transform);
        }

        public event Action OpenEvent;
        /// <summary>
        /// 每次打开UI调用
        /// </summary>
        public virtual void Open()
        {
            gameObject.SetActive(true);
            if (IsPause == false)
            {
                IsPause = false;
                View.canvasGroup.blocksRaycasts = true;
                View.canvasGroup.alpha = 1f;
            }
            OpenEvent?.Invoke();
        }
        public event Action CloseEvent;
        /// <summary>
        /// 销毁/关闭UI调用
        /// </summary>
        public virtual void Close()
        {
            gameObject.SetActive(false);
            CloseEvent?.Invoke();
        }
        public event Action HideEvent;
        public virtual void Hide()
        {
            gameObject.SetActive(false);
            HideEvent?.Invoke();
        }

        /// <summary>
        /// 暂停标记
        /// </summary>
        public bool IsPause { get; private set; }
        /// <summary>
        /// 禁用当前UI所有交互操作
        /// </summary>
        public virtual void Pause()
        {
            IsPause = true;
            View.canvasGroup.blocksRaycasts = false;
            View.canvasGroup.alpha = 0.3f;
        }

    }
}