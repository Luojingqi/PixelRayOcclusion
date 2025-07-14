using Google.FlatBuffers;
using PRO.Skill;
using PRO.Tool;
using System;
using System.Collections.Generic;

namespace PRO.TurnBased
{
    public class TurnState1_Operate : IFSMState<TurnStateEnum>
    {
        public FSMManager<TurnStateEnum> FSM { get => fsm; set => fsm = (TurnFSM)value; }
        private TurnFSM fsm;
        public TurnStateEnum EnumName => TurnStateEnum.operate;

        public void Enter() { NowOperate_T1 = null; }

        public void Exit() { }

        public OperateFSMBase NowOperate_T1 { get; private set; }

        public List<OperateFSMBase> NowOperateList_T2 = new List<OperateFSMBase>();
        public void Update()
        {
            foreach (var operate in fsm.Agent.AllCanUseOperate.Values)
            {
                //更新所有操作的t0状态
                operate.T0.Update();
                //只记录最后一个进入t1状态的操作，在此之前进入的会回退到t0
                if (operate.NowState.EnumName == OperateStateEnum.t1)
                {
                    if (NowOperate_T1 != null && NowOperate_T1 != operate)
                    {
                        NowOperate_T1.SwitchState(OperateStateEnum.t0);
                    }
                    NowOperate_T1 = operate;
                }
            }
            //有一个t1状态，更新
            if (NowOperate_T1 != null)
            {
                if (NowOperate_T1.NowState.EnumName == OperateStateEnum.t1)
                    NowOperate_T1.Update();
                if (NowOperate_T1.NowState.EnumName == OperateStateEnum.t2)
                    NowOperateList_T2.Add(NowOperate_T1);
                if (NowOperate_T1.NowState.EnumName != OperateStateEnum.t1)
                    NowOperate_T1 = null;
            }
            for (int i = NowOperateList_T2.Count - 1; i >= 0; i--)
            {
                var operate = NowOperateList_T2[i];
                operate.Update();
                if (operate.NowState.EnumName != OperateStateEnum.t2)
                    NowOperateList_T2.RemoveAt(i);
            }
        }

        public Offset<Flat.TurnState1_OperateData> ToDisk(FlatBufferBuilder builder)
        {
            Span<int> nowOperateListT2OffsetArray = stackalloc int[NowOperateList_T2.Count];
            for (int i = 0; i < NowOperateList_T2.Count; i++)
            {
                var operate = NowOperateList_T2[i];
                nowOperateListT2OffsetArray[i] = operate.ToDisk(builder).Value;
            }
            var nowOperateListT2Offset = builder.CreateVector_Offset(nowOperateListT2OffsetArray);
            Flat.TurnState1_OperateData.StartTurnState1_OperateData(builder);
            Flat.TurnState1_OperateData.AddNowOperateListT2(builder, nowOperateListT2Offset);
            return Flat.TurnState1_OperateData.EndTurnState1_OperateData(builder);
        }
        public void ToRAM(Flat.TurnState1_OperateData diskData)
        {
            for (int i = diskData.NowOperateListT2Length - 1; i >= 0; i--)
            {
                var operateDiskData = diskData.NowOperateListT2(i).Value;
                var operate = fsm.Agent.AllCanUseOperate[operateDiskData.Guid];
                operate.ToRAM(operateDiskData);
                NowOperateList_T2.Add(operate);
            }
        }
    }
}
