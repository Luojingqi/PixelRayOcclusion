using PRO;
using TMPro;
namespace PRO
{
    public class InjuryEstimationPanelV : UIChildViewBase
    {
        public TMP_Text Info_命中率 { get; private set; }
        public TMP_Text Info_暴击率 { get; private set; }

        public override void Init(UIChildControllerBase controller)
        {
            base.Init(controller);
            Info_命中率 = transform.Find("命中率").GetComponent<TMP_Text>();
            Info_暴击率 = transform.Find("暴击率").GetComponent<TMP_Text>();
        }
    }
}