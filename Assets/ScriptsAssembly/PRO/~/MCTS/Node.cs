using Google.FlatBuffers;
using PRO.Skill;
using PRO.Tool;
using PRO.TurnBased;
using System;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

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
                base.PutIn();
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
                //To(this.builder.ToSpan());
                var roleOffset = builder.CreateString(operate.Agent.GUID);
                var operateOffset = builder.CreateString(operate.GUID);
                var builderOffset = builder.CreateVector_Builder(this.builder);
                //To(this.builder.ToSpan());
                Flat.Node.StartNode(builder);
                Flat.Node.AddTurnTimeNum(builder, turnTimeNum);
                Flat.Node.AddRole(builder, roleOffset);
                Flat.Node.AddOperate(builder, operateOffset);
                Flat.Node.AddBuilder(builder, builderOffset);
                var offset = Flat.Node.EndNode(builder);
                return (Flat.NodeBase.Node, new(offset.Value));
            }

            public static Node ToRAM(Flat.Node diskData, SceneEntity scene)
            {
                var node = TakeOut();
                node.turnTimeNum = diskData.TurnTimeNum;
                node.operate = scene.GetRole(diskData.Role).AllCanUseOperate[diskData.Operate];
                var length = diskData.BuilderLength;
                node.builder = FlatBufferBuilder.TakeOut(length);
                Span<byte> builderByteSpan = node.builder.DataBuffer.ToSpan(0, length);
                for (int i = length - 1; i >= 0; i--)
                    builderByteSpan[length - i - 1] = diskData.Builder(i);
                node.builder.Offset = length;
                //To(node.builder.DataBuffer.ToSpan(0, length));
                return node;
            }

            public static void To(Span<byte> bytes)
            {
                StringBuilder sb = new StringBuilder(bytes.Length);
                for (int i = 0; i < bytes.Length; i++)
                    sb.Append(bytes[i]);
                Debug.Log(sb.ToString());
            }
        }
    }
}
