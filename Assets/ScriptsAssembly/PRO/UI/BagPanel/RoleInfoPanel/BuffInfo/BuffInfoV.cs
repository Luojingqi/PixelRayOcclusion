using PRO;
using TMPro;
using UnityEngine;

namespace PRO
{
    public class BuffInfoV : UIChildViewBase
    {
        public TMP_Text NumText { get; private set; }

        public override void Init(UIChildControllerBase controller)
        {
            base.Init(controller);
            NumText = transform.Find("NumText").GetComponent<TMP_Text>();
            NumText.text = null;
        }
    }
}
