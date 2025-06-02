using PRO;
using UnityEngine.UI;
using static PRO.RoleInfoPanelM;
namespace PRO
{
    public class RoleInfoPanelV : UIChildViewBase
    {
        public Image Icon { get; private set; }
        public �ж���Panel �ж���Panel { get; private set; }
        public Info Info_Ѫ { get; private set; }
        public Info Info_���� { get; private set; }
        public Info Info_���� { get; private set; }
        public Info Info_���� { get; private set; }
        public Info Info_���� { get; private set; }

        public BuffInfoListC BuffInfoListC { get; private set; }

        public override void Init(UIChildControllerBase controller)
        {
            base.Init(controller);
            Icon = transform.Find("Icon").GetComponent<Image>();
            �ж���Panel = new �ж���Panel(transform.Find("�ж���Panel"));
            Info_Ѫ = new Info(transform.Find("Info-Ѫ"));
            Info_���� = new Info(transform.Find("Info-����"));
            Info_���� = new Info(transform.Find("Info-����"));
            Info_���� = new Info(transform.Find("Info-����"));
            Info_���� = new Info(transform.Find("Info-����"));
            BuffInfoListC = transform.Find("BuffList").GetComponent<BuffInfoListC>();
        }
    }
}