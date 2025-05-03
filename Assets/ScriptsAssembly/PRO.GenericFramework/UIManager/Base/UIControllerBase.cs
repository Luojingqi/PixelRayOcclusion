using System;
namespace PRO
{
    public abstract class UIControllerBase : MonoScriptBase
    {

        public abstract UIViewBase View { get; }
        public string UIName { get; private set; }



        //public LanguageManager.LanguageEnum NowUseLanguage { get; set; }




        /// <summary>
        /// Ui初始化值调用一次
        /// </summary>
        public virtual void Init(string uiName)
        {
            View.Init(transform);
            this.UIName = uiName;
        }

        public event Action OpenEvent;
        /// <summary>
        /// 每次打开UI调用
        /// </summary>
        public virtual void Open()
        {
            gameObject.SetActive(true);
            if (IsPause == true) UnPause();
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

        private bool IsPause = false;
        /// <summary>
        /// 禁用当前UI所有交互操作
        /// </summary>
        public virtual void Pause()
        {
            IsPause = true;
            View.canvasGroup.blocksRaycasts = false;
            View.canvasGroup.alpha = 0.3f;
        }
        /// <summary>
        /// 解除禁用
        /// </summary>
        public virtual void UnPause()
        {
            IsPause = false;
            View.canvasGroup.blocksRaycasts = true;
            View.canvasGroup.alpha = 1f;
        }

    }
}