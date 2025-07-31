using Google.FlatBuffers;
using PRO.Tool;

namespace PRO.TurnBased
{
    /// <summary>
    /// 回合状态机，回合制系统里单个回合，包含一系列操作
    /// </summary>
    public class TurnFSM : FSMManager<TurnStateEnum>
    {
        public RoundFSM RoundFSM { get; private set; }
        public Role Agent { get; private set; }
        public int Index { get; private set; }
        public TurnState1_Operate State_Operate { get; private set; }
        public TurnFSM(RoundFSM roundFSM, Role role, int index)
        {
            AddState(new TurnState0_Start());
            State_Operate = new TurnState1_Operate();
            AddState(State_Operate);
            AddState(new TurnState2_End());

            RoundFSM = roundFSM;
            Agent = role;
            Index = index;
            Agent.Turn = this;
            SetState(TurnStateEnum.end);
        }

        public Offset<Flat.TurnFSMData> ToDisk(FlatBufferBuilder builder)
        {
            var roleGuidOffset = builder.CreateString(Agent.GUID);
            var state_operate_offset = NowState.EnumName == TurnStateEnum.operate ? State_Operate.ToDisk(builder) : new();
            Flat.TurnFSMData.StartTurnFSMData(builder);
            Flat.TurnFSMData.AddNowState(builder, (int)NowState.EnumName);
            Flat.TurnFSMData.AddRoleGuid(builder, roleGuidOffset);
            Flat.TurnFSMData.AddIndex(builder, Index);
            Flat.TurnFSMData.AddStateOperate(builder, state_operate_offset);
            return Flat.TurnFSMData.EndTurnFSMData(builder);
        }
        public void ToRAM(Flat.TurnFSMData diskData)
        {
            SetState((TurnStateEnum)diskData.NowState);
            if (NowState.EnumName == TurnStateEnum.operate)
                State_Operate.ToRAM(diskData.StateOperate.Value);
        }
    }
}
