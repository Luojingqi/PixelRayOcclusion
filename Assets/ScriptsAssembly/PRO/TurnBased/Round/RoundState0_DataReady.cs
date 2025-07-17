using PRO.Tool;
using UnityEngine;

namespace PRO.TurnBased
{
    public class RoundState0_DataReady : IFSMState<RoundStateEnum>
    {
        public FSMManager<RoundStateEnum> FSM { get => fsm; set => fsm = (RoundFSM)value; }
        private RoundFSM fsm;
        public RoundStateEnum EnumName => RoundStateEnum.dataReady;

        public void Enter() { }

        public void Exit() { }
        public void Update() { }

        public void AddRole(Role role)
        {
            fsm.RoleHash.Add(role);
        }

        public void ReadyOver()
        {
            LogPanelC.Inst.AddLog($"角色准备完毕，角色数量{fsm.RoleHash.Count}", true);

            foreach (var agent in fsm.RoleHash)
            {
                agent.Rig2D.simulated = true;

                CombatContext context = CombatContext.TakeOut();
                context.SetAgent(agent);
                context.LogBuilder.Append("战斗开始：");
                agent.ForEachBuffApplyEffect(BuffTriggerType.战斗开始时, context, -1);
                context.Calculate_最终结算();
                LogPanelC.Inst.AddLog(context, true);
                CombatContext.PutIn(context);

                agent.Info.行动点.Value = agent.Info.行动点初始.Value;
            }

            fsm.SwitchState(RoundStateEnum.beFrightened);
        }


    }
}
