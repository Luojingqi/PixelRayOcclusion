using Google.FlatBuffers;
using PRO.Tool;
using System;

namespace PRO.AI
{
    public partial class MCTS
    {
        internal class TimeNode : NodeBase
        {
            private static ObjectPool<TimeNode> pool = new ObjectPool<TimeNode>();
            public static TimeNode TakeOut() => pool.TakeOut();

            public override void PutIn()
            {
                base.PutIn();
                timeNum = 0;
                pool.PutIn(this);
            }

            public int timeNum;

            public override void 执行()
            {
                turnTimeNum += timeNum;
                float time = timeNum * 0.4f;
                while (time > 0)
                {
                    time -= TimeManager.physicsDeltaTime;
                    TimeManager.Inst.ScriptUpdate(TimeManager.physicsDeltaTime);
                }
            }

            public override (Flat.NodeBase, Offset<int>) ToDisk(FlatBufferBuilder builder)
            {
                Flat.TimeNode.StartTimeNode(builder);
                Flat.TimeNode.AddTurnTimeNum(builder, turnTimeNum);
                Flat.TimeNode.AddTurnTime(builder, timeNum);
                return (Flat.NodeBase.TimeNode, new(Flat.TimeNode.EndTimeNode(builder).Value));
            }

            public static TimeNode ToRAM(Flat.TimeNode diskData, SceneEntity scene)
            {
                var node = TakeOut();
                node.turnTimeNum = diskData.TurnTimeNum;
                node.timeNum = diskData.TurnTime;
                return node;
            }
        }
    }
}
