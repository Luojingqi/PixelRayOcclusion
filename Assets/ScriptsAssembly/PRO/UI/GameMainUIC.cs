using PRO.TurnBased;
using PRO;
using System;
using System.Collections.Generic;
namespace PRO
{
    public class GameMainUIC : UIControllerBase, ITime_Awake, ITime_Update
    {
        public static GameMainUIC Inst { get; private set; }
        public override UIViewBase View => view;
        private GameMainUIV view = new GameMainUIV();

        public override void Init(string uiName)
        {
            Inst = this;
            base.Init(uiName);
            AddChildUI(view.RoundPanel);
            view.RoundPanel.gameObject.SetActive(false);
            AddChildUI(view.BottomBag);
            AddChildUI(view.RoleInfoPanel);
            InjuryEstimationPanelC.InitPool(view.InjuryEstimationPrefab);
        }

        public void TimeAwake()
        {
            Init("");
        }

        public void SetTurn(List<TurnFSM> list, int nowTurn)
        {
            view.RoundPanel.SetTurn(list, nowTurn);
            SetRole(list[nowTurn].Agent);
        }

        public Role NowShowRole { get; private set; }
        /// <summary>
        /// 设置当前展示哪位角色的信息
        /// 一般用于选择对应角色后切换显示他的信息
        /// </summary>
        /// <param name="role"></param>
        public void SetRole(Role role)
        {
            if (NowShowRole != null)
            {
                NowShowRole.Info.ClearAction();
            }

            view.BottomBag.SetRole(role);
            view.RoleInfoPanel.SetRole(role);
            NowShowRole = role;
        }
        /// <summary>
        /// 只更新当前展示的角色的信息ui
        /// 一般用于buff更新后调用
        /// </summary>
        public void UpdateNowShowRoleBuff()
        {
            if (NowShowRole == null) return;
            view.RoleInfoPanel.BuffInfoListC.SetRole(NowShowRole);
        }
        /// <summary>
        /// 战斗开始时打开回合制ui
        /// </summary>
        public void OpenTurnUI()
        {
            view.RoundPanel.gameObject.SetActive(true);
        }
        /// <summary>
        /// 战斗结束后关闭回合制ui
        /// </summary>
        public void CloseTurnUI()
        {
            view.RoundPanel.Clear();
            view.RoundPanel.gameObject.SetActive(false);
        }

        public event Action UpdateAction;
        public override void TimeUpdate()
        {
            base.TimeUpdate();
            UpdateAction?.Invoke();
        }
    }
}