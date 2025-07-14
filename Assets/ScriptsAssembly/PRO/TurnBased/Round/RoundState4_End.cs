using PRO.Tool;

namespace PRO.TurnBased
{
    public class RoundState4_End : IFSMState<RoundStateEnum>
    {
        public FSMManager<RoundStateEnum> FSM { get => fsm; set => fsm = (RoundFSM)value; }
        private RoundFSM fsm;
        public RoundStateEnum EnumName => RoundStateEnum.end;

        public void Enter()
        {

        }

        public void Update()
        {

        }

        public void Exit()
        {

        }
    }
}
