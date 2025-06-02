using PRO.Tool;

namespace PRO.TurnBased
{
    /// <summary>
    /// 回合状态机，回合制系统里单个回合，包含一系列操作
    /// </summary>
    public class TurnFSM : FSMManager<TurnStateEnum>
    {
        public RoundFSM RoundFSM { get; private set; }
        public Role Agent { get; set; }
        public int Index { get; set; }
        public TurnState1_Operate State_Operate { get; private set; }
        public TurnFSM()
        {
            AddState(new TurnState0_Start());
            State_Operate = new TurnState1_Operate();
            AddState(State_Operate);
            AddState(new TurnState2_End());
        }

        public void Init(RoundFSM roundFSM, Role role, int index)
        {
            RoundFSM = roundFSM;
            Agent = role;
            Index = index;
            //foreach (var operate in Agent.AllCanUseOperate)
            //{
            //    operate.Turn = this;
            //}
        }

    }
}
