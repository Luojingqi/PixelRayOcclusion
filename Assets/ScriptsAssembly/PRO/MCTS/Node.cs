using Google.FlatBuffers;
using PRO.Skill;
using PRO.Tool;
using PRO.TurnBased;
using System;

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

            public override (Flat.NodeBase, Offset<int>) ToDisk(FlatBufferBuilder builder)
            {
                var roleOffset = builder.CreateString(operate.Agent.GUID);
                var operateOffset = builder.CreateString(operate.GUID);
                var builderOffset = builder.CreateVector_Builder(builder);
                Flat.Node.StartNode(builder);
                Flat.Node.AddTurnTimeNum(builder, turnTimeNum);
                Flat.Node.AddRole(builder, roleOffset);
                Flat.Node.AddOperate(builder, operateOffset);
                Flat.Node.AddBuilder(builder, builderOffset);
                return (Flat.NodeBase.Node, new(Flat.Node.EndNode(builder).Value));
            }

            public static Node ToRAM(Flat.Node diskData, SceneEntity scene)
            {
                var node = TakeOut();
                node.turnTimeNum = diskData.TurnTimeNum;
                node.operate = scene.GetRole(diskData.Role).AllCanUseOperate[diskData.Operate];
                node.builder = FlatBufferBuilder.TakeOut(diskData.BuilderLength);
                Span<byte> builderByteSpan = node.builder.DataBuffer.ToSpan(0, node.builder.DataBuffer.Length);
                for (int i = builderByteSpan.Length - 1; i >= 0; i--)
                    builderByteSpan[builderByteSpan.Length - i - 1] = diskData.Builder(i);
                return node;
            }
        }
    }
}
