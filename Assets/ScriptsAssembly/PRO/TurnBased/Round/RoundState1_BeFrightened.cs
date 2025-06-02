using PRO.Tool;

namespace PRO.TurnBased
{
    public class RoundState1_BeFrightened : IFSMState<RoundStateEnum>
    {
        public FSMManager<RoundStateEnum> FSM { get => fsm; set => fsm = (RoundFSM)value; }
        private RoundFSM fsm;
        public RoundStateEnum EnumName => RoundStateEnum.beFrightened;

        public void Enter()
        {
            fsm.SwitchState(RoundStateEnum.initiative);
        }

        public void Exit()
        {

        }

        public void Update()
        {

        }
    }
}
