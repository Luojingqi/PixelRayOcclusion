using Google.FlatBuffers;
using PRO.Skill;
using PRO.Tool;
using PRO.TurnBased;

namespace PRO.AI
{
    public partial class MCTS
    {
        internal class Node : NodeBase
        {
            private static ObjectPool<Node> pool = new ObjectPool<Node>();
            public static Node TakeOut() => pool.TakeOut();
            public override void PutIn()
            {
                operate = null;
                FlatBufferBuilder.PutIn(builder);
                builder = null;
                pool.PutIn(this);
            }

            public OperateFSMBase operate;
            public FlatBufferBuilder builder;

            public override void 执行()
            {
                if (operate.T0.TrySwitchStateToT1(OperateFSMBase.Operator.AI) == false) return;
                operate.T1.节点执行(builder, OperateFSMBase.Operator.AI);
                operate.Agent.ForEachBuffApplyEffect(BuffTriggerType.技能释放前, operate.context, -1);
                operate.SwitchState(OperateStateEnum.t2);
                operate.T2.Enter(builder);
                operate.Agent.Turn.State_Operate.NowOperateList_T2.Add(operate);
                TimeManager.Inst.ScriptUpdate(0);
            }
        }
    }
}
