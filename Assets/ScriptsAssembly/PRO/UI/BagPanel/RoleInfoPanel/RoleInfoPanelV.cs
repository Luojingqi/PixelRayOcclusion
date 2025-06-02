using PRO;
using UnityEngine.UI;
using static PRO.RoleInfoPanelM;
namespace PRO
{
    public class RoleInfoPanelV : UIChildViewBase
    {
        public Image Icon { get; private set; }
        public 行动点Panel 行动点Panel { get; private set; }
        public Info Info_血 { get; private set; }
        public Info Info_护甲 { get; private set; }
        public Info Info_闪避 { get; private set; }
        public Info Info_命中 { get; private set; }
        public Info Info_暴击 { get; private set; }

        public BuffInfoListC BuffInfoListC { get; private set; }

        public override void Init(UIChildControllerBase controller)
        {
            base.Init(controller);
            Icon = transform.Find("Icon").GetComponent<Image>();
            行动点Panel = new 行动点Panel(transform.Find("行动点Panel"));
            Info_血 = new Info(transform.Find("Info-血"));
            Info_护甲 = new Info(transform.Find("Info-护甲"));
            Info_闪避 = new Info(transform.Find("Info-闪避"));
            Info_命中 = new Info(transform.Find("Info-命中"));
            Info_暴击 = new Info(transform.Find("Info-暴击"));
            BuffInfoListC = transform.Find("BuffList").GetComponent<BuffInfoListC>();
        }
    }
}