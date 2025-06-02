using PRO.Tool;

namespace PRO.TurnBased
{
    /// <summary>
    /// �غ�״̬�����غ���ϵͳ�ﵥ���غϣ�����һϵ�в���
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
