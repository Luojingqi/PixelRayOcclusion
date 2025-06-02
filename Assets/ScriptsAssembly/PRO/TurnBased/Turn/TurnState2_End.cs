using PRO.Tool;

namespace PRO.TurnBased
{
    public class TurnState2_End : IFSMState<TurnStateEnum>
    {
        public FSMManager<TurnStateEnum> FSM { get => fsm; set => fsm = (TurnFSM)value; }
        private TurnFSM fsm;

        public TurnStateEnum EnumName => TurnStateEnum.end;

        public void Enter()
        {
            CombatContext context = CombatContext.TakeOut();
            context.SetAgent(fsm.Agent, fsm.RoundFSM, fsm);
            context.LogBuilder.Append($"回合结束：");
            fsm.Agent.ForEachBuffApplyEffect(BuffTriggerType.回合结束时, context, -1);
            context.Calculate_最终结算();
            LogPanelC.Inst.AddLog(context, true);
            CombatContext.PutIn(context);
        }

        public void Exit()
        {

        }

        public void Update()
        {

        }
    }
}
