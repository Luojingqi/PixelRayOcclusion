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
        /// ���õ�ǰչʾ��λ��ɫ����Ϣ
        /// һ������ѡ���Ӧ��ɫ���л���ʾ������Ϣ
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
        /// ֻ���µ�ǰչʾ�Ľ�ɫ����Ϣui
        /// һ������buff���º����
        /// </summary>
        public void UpdateNowShowRoleBuff()
        {
            if (NowShowRole == null) return;
            view.RoleInfoPanel.BuffInfoListC.SetRole(NowShowRole);
        }
        /// <summary>
        /// ս����ʼʱ�򿪻غ���ui
        /// </summary>
        public void OpenTurnUI()
        {
            view.RoundPanel.gameObject.SetActive(true);
        }
        /// <summary>
        /// ս��������رջغ���ui
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