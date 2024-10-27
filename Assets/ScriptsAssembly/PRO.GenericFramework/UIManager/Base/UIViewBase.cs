using UnityEngine;
using UnityEngine.UI;
namespace PRO
{
    public class UIViewBase
    {
        public Transform transform { get; private set; }
        public RectTransform rectTransform { get; private set; }

        public CanvasGroup canvasGroup { get; private set; }
        public Canvas canvas { get; private set; }
        public CanvasScaler canvasScaler { get; private set; }

        public virtual void Init(Transform transform)
        {
            this.transform = transform;
            rectTransform = transform.GetComponent<RectTransform>();
            canvasGroup = transform.GetComponent<CanvasGroup>();
            canvas = transform.GetComponent<Canvas>();
            canvasScaler = transform.GetComponent<CanvasScaler>();
        }
    }
}