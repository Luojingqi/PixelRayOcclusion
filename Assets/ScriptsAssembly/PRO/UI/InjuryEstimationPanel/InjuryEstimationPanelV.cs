using PRO;
using TMPro;
namespace PRO
{
    public class InjuryEstimationPanelV : UIChildViewBase
    {
        public TMP_Text Info_������ { get; private set; }
        public TMP_Text Info_������ { get; private set; }

        public override void Init(UIChildControllerBase controller)
        {
            base.Init(controller);
            Info_������ = transform.Find("������").GetComponent<TMP_Text>();
            Info_������ = transform.Find("������").GetComponent<TMP_Text>();
        }
    }
}