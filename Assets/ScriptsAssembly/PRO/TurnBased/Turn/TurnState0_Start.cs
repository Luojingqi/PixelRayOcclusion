using PRO.Tool;

namespace PRO.TurnBased
{
    public class TurnState0_Start : IFSMState<TurnStateEnum>
    {
        public FSMManager<TurnStateEnum> FSM { get => fsm; set => fsm = (TurnFSM)value; }
        private TurnFSM fsm;

        public TurnStateEnum EnumName => TurnStateEnum.start;

        public void Enter()
        {
            CombatContext context = CombatContext.TakeOut();
            context.SetAgent(fsm.Agent, fsm.RoundFSM, fsm);
            context.LogBuilder.Append($"回合开始：");
            fsm.Agent.ForEachBuffApplyEffect(BuffTriggerType.回合开始时, context, -1);
            context.Calculate_最终结算();
            LogPanelC.Inst.AddLog(context, true);
            CombatContext.PutIn(context);
            fsm.SwitchState(TurnStateEnum.operate);
        }

        public void Exit() { }

        public void Update() { }
    }
}
