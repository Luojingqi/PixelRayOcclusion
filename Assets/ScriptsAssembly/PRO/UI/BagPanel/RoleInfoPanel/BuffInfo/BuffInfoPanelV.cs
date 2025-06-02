using PRO;
using TMPro;
using UnityEngine;

namespace PRO
{
    public class BuffInfoPanelV : UIChildViewBase
    {
        public TMP_Text Name { get; private set; }
        public TMP_Text Info { get; private set; }

        public override void Init(UIChildControllerBase controller)
        {
            base.Init(controller);
            Name = transform.Find("Name").GetComponent<TMP_Text>();
            Info = transform.Find("Info").GetComponent<TMP_Text>();
        }
    }
}
