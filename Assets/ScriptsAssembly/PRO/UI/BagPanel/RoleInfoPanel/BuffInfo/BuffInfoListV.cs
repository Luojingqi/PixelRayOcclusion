using PRO;
using UnityEngine;

namespace PRO
{
    public class BuffInfoListV : UIChildViewBase
    {
        public BuffInfoC buffInfo { get; private set; }
        public BuffInfoPanelC buffInfoPanel { get; private set; }

        public override void Init(UIChildControllerBase controller)
        {
            base.Init(controller);
            buffInfoPanel = transform.Find("BuffInfoPanel").GetComponent<BuffInfoPanelC>();
            buffInfo = transform.Find("BuffInfo").GetComponent<BuffInfoC>();
        }
    }
}
