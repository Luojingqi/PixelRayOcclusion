using Google.FlatBuffers;
using PRO.Tool;
using System;
using System.Collections.Generic;

namespace PRO.TurnBased
{
    public class RoundState3_Turn : IFSMState<RoundStateEnum>
    {
        public FSMManager<RoundStateEnum> FSM { get => fsm; set => fsm = (RoundFSM)value; }
        private RoundFSM fsm;
        public RoundStateEnum EnumName => RoundStateEnum.turn;

        /// <summary>
        /// 回合顺序
        /// </summary>
        public List<TurnFSM> TurnFSMList = new List<TurnFSM>();

       
        public void Enter()
        {
            TurnFSMList[0].SwitchState(TurnStateEnum.start);
        }

        public void Exit()
        {
            foreach (var turn in TurnFSMList)
            {
                CombatContext context = CombatContext.TakeOut();
                context.SetAgent(turn.Agent);
                context.LogBuilder.Append($"战斗结束：");
                turn.Agent.ForEachBuffApplyEffect(BuffTriggerType.战斗结束时, context, -1);
                context.Calculate_最终结算();
                LogPanelC.Inst.AddLog(context, true);
                CombatContext.PutIn(context);
            }
        }
        /// <summary>
        /// 当前进行的轮次
        /// </summary>
        public int NowRoundNum => nowRoundNum;
        private int nowRoundNum;
        /// <summary>
        /// 当前正在执行谁的回合
        /// </summary>
        private int nowTurnIndex = 0;
        public TurnFSM NowTurn => TurnFSMList[nowTurnIndex];
        public void Update()
        {
            TurnFSM turn = TurnFSMList[nowTurnIndex];
            if (turn.NowState.EnumName == TurnStateEnum.end)
            {
                turn.Agent.Info.ClearAction();
                //上一位的回合结束
                //判断是否一个轮次结束了
                if (++nowTurnIndex >= TurnFSMList.Count)
                    NextRound();
                turn = TurnFSMList[nowTurnIndex];
                turn.SwitchState(TurnStateEnum.start);
                if (nowRoundNum > 0)
                    turn.Agent.Info.行动点.Value += turn.Agent.Info.行动点初始.Value;
                if (GamePlayMain.Inst.Round == fsm)
                    GamePlayMain.Inst.Round = fsm;
            }
            turn.Update();
        }

        private void NextRound()
        {
            foreach (var turn in TurnFSMList)
            {
                CombatContext context = CombatContext.TakeOut();
                context.SetAgent(turn.Agent);
                context.LogBuilder.Append($"轮次结束：");
                turn.Agent.ForEachBuffApplyEffect(BuffTriggerType.轮次结束时, context, -1);
                context.Calculate_最终结算();
                LogPanelC.Inst.AddLog(context, true);
                CombatContext.PutIn(context);
            }
            nowTurnIndex = 0;
            nowRoundNum++;
            foreach (var turn in TurnFSMList)
            {
                CombatContext context = CombatContext.TakeOut();
                context.SetAgent(turn.Agent);
                context.LogBuilder.Append($"轮次开始：");
                turn.Agent.ForEachBuffApplyEffect(BuffTriggerType.轮次开始时, context, -1);
                context.Calculate_最终结算();
                LogPanelC.Inst.AddLog(context, true);
                CombatContext.PutIn(context);
            }
        }

        public Offset<Flat.RoundState3_TurnData> ToDisk(FlatBufferBuilder builder)
        {
            Span<int> turnFSMListOffsetArray = stackalloc int[TurnFSMList.Count];
            for (int i = 0; i < turnFSMListOffsetArray.Length; i++)
                turnFSMListOffsetArray[i] = TurnFSMList[i].ToDisk(builder).Value;
            var turnFSMListOffset = builder.CreateVector_Offset(turnFSMListOffsetArray);
            Flat.RoundState3_TurnData.StartRoundState3_TurnData(builder);
            Flat.RoundState3_TurnData.AddNowRoundNum(builder, nowRoundNum);
            Flat.RoundState3_TurnData.AddNowTurnIndex(builder, nowTurnIndex);
            Flat.RoundState3_TurnData.AddTurnFsmList(builder, turnFSMListOffset);
            return Flat.RoundState3_TurnData.EndRoundState3_TurnData(builder);
        }
        public void ToRAM(Flat.RoundState3_TurnData diskData)
        {
            nowRoundNum = diskData.NowRoundNum;
            nowTurnIndex = diskData.NowTurnIndex;
            for (int i = diskData.TurnFsmListLength - 1; i >= 0; i--)
            {
                var turnDiskData = diskData.TurnFsmList(i).Value;
                var role = fsm.Scene.GetRole(turnDiskData.RoleGuid);
                var turn = new TurnFSM(fsm, role, turnDiskData.Index);
                turn.ToRAM(turnDiskData);
                TurnFSMList.Add(turn);
            }
        }
    }
}
