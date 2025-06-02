using PRO;
using UnityEngine;
namespace PRO
{
    public class GameMainUIV : UIViewBase
    {
        /// <summary>
        /// 顶部回合制面板
        /// </summary>
        public RoundPanelC RoundPanel { get; private set; }
        /// <summary>
        /// 底部背包
        /// </summary>
        public BottomBagC BottomBag { get; private set; }
        /// <summary>
        /// 角色信息面板
        /// </summary>
        public RoleInfoPanelC RoleInfoPanel { get; private set; }
        /// <summary>
        /// 伤害估算预制体
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
