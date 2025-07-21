using PRO.Tool;

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
                timeNum = 0;
                pool.PutIn(this);
            }

            public int timeNum;

            public override void 执行()
            {
                turnTimeNum += timeNum;
                float time = timeNum * 0.5f;
                while (time > 0)
                {
                    time -= TimeManager.physicsDeltaTime;
                    TimeManager.Inst.ScriptUpdate(TimeManager.physicsDeltaTime);
                }
            }
        }
    }
}
