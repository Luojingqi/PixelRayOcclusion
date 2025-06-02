using PRO;
using UnityEngine;
namespace PRO
{
    public class GameMainUIV : UIViewBase
    {
        /// <summary>
        /// �����غ������
        /// </summary>
        public RoundPanelC RoundPanel { get; private set; }
        /// <summary>
        /// �ײ�����
        /// </summary>
        public BottomBagC BottomBag { get; private set; }
        /// <summary>
        /// ��ɫ��Ϣ���
        /// </summary>
        public RoleInfoPanelC RoleInfoPanel { get; private set; }
        /// <summary>
        /// �˺�����Ԥ����
        /// </summary>
        public InjuryEstimationPanelC InjuryEstimationPrefab { get; private set; }

        public override void Init(UIControllerBase controller)
        {
            base.Init(controller);
            RoundPanel = transform.Find("RoundPanel").GetComponent<RoundPanelC>();
            BottomBag = transform.Find("BottomBag").GetComponent<BottomBagC>();
            RoleInfoPanel = transform.Find("RoleInfoPanel").GetComponent<RoleInfoPanelC>();
            InjuryEstimationPrefab = transform.Find("InjuryEstimationPanel").GetComponent<InjuryEstimationPanelC>();
            InjuryEstimationPrefab.gameObject.SetActive(false);
        }

    }
}
