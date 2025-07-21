using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace PRO.AI
{
    public partial class MCTS
    {
        internal class MainNode : NodeBase
        {
            public void 开始模拟()
            {
                扩展();
                ThreadPool.QueueUserWorkItem((obj) =>
                {
                    CountdownEvent countDown = null;
                    int max = chiles.Count * 50;
                    for (int i = 0; i < max; i++)
                    {
                        TimeManager.Inst.AddToQueue_MainThreadUpdate_Clear_WaitInvoke(() =>
                        {
                            访问次数 += 1;
                            var nextNode = chiles.Dequeue();
                            nextNode.访问();
                            chiles.Enqueue(nextNode, -nextNode.Get_UCB());
                            Debug.Log($"模拟第{i}次——结束");
                            countDown = mcts.round.Scene.ReLoadAll();
                        });
                        countDown.Wait();
                    }

                    var nextNode = chiles.Dequeue();
                    while (nextNode != null)
                    {
                        if (nextNode is Node node) { Debug.Log("操作" + node.operate.config.Name + "|" + node.访问次数); }
                        else if (nextNode is TimeNode timeNode) { Debug.Log("等待" + timeNode.timeNum + "|" + timeNode.访问次数); }
                        if (nextNode.chiles.Count > 0)
                            nextNode = nextNode.chiles.Dequeue();
                        else
                            nextNode = null;
                    }

                });
            }


            public override void 执行()
            {
                throw new System.NotImplementedException();
            }

            public MainNode(MCTS mcts)
            {
                this.mcts = mcts;
            }
        }
    }
}
