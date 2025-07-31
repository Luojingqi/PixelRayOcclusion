using PRO.Tool;

namespace PRO.TurnBased
{
    public class RoundState2_Initiative : IFSMState<RoundStateEnum>
    {
        public FSMManager<RoundStateEnum> FSM { get => fsm; set => fsm = (RoundFSM)value; }
        private RoundFSM fsm;
        public RoundStateEnum EnumName => RoundStateEnum.initiative;

        public void Enter()
        {
            int i = 0;
            var state3 = fsm.GetState<RoundState3_Turn>();
            foreach (var role in fsm.RoleHash)
            {
                //此处应当先按照先攻排序，然后按顺序生成回合
                TurnFSM turn = new TurnFSM(fsm, role, i++);
                state3.TurnFSMList.Add(turn);
            }
            if (GamePlayMain.Inst.Round == fsm)
                GamePlayMain.Inst.Round = fsm;
            fsm.SwitchState(RoundStateEnum.turn);
        }

        public void Exit()
        {

        }

        public void Update()
        {

        }
    }
}
