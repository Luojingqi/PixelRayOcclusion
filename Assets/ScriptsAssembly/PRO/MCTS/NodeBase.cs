using Google.FlatBuffers;
using PRO.DataStructure;
using PRO.Tool;
using PRO.TurnBased;
using UnityEngine;

namespace PRO.AI
{
    public partial class MCTS
    {
        internal abstract class NodeBase
        {
            public virtual void PutIn()
            {
                mcts = null;
                parent = null;
                foreach (var node in chiles)
                    node.PutIn();
                chiles.Clear();
                for (int i = 0; i < Effects.Length; i++)
                    Effects[i] = default;
                已扩展 = false;
                turnTimeNum = 0;
            }

            private bool 结束检查()
            {
                if (this is Node node && node.operate is Skill.Skill_0_0)
                {
                    turnTimeNum = 0;
                }

                if (mcts.round.State3_Turn.NowRoundNum - mcts.startingData.roundNum > 3)
                {
                    //更新了若干回合
                    {
                        var effect_自己 = new Effect();
                        var guid = mcts.startingData.roleGuid;
                        var role = mcts.round.Scene.GetRole(guid);
                        var startingInfo = mcts.startingData.roleGuidInfoDic[guid];
                        if (role == null)
                        {
                            //我死了
                            effect_自己.血量 -= startingInfo.血量.Value * 20;
                        }
                        else
                        {
                            effect_自己.血量 += role.Info.血量.Value - startingInfo.血量.Value;
                        }
                        AddEffect(EffectAgent.自己, effect_自己);
                    }
                    {
                        var effect_敌军 = new Effect();
                        foreach (var kv in mcts.startingData.roleGuidInfoDic)
                        {
                            string guid = kv.Key;
                            var startingInfo = kv.Value;
                            var role = mcts.round.Scene.GetRole(guid);
                            if (role == null)
                            {
                                //杀死了敌人
                                effect_敌军.血量 -= startingInfo.血量.Value * 10;
                            }
                            else
                            {
                                effect_敌军.血量 += role.Info.血量.Value - startingInfo.血量.Value;
                            }
                        }
                        AddEffect(EffectAgent.敌军, effect_敌军);
                    }

                    return true;
                }


                if (turnTimeNum > maxTurnTimeNum)
                    return false;

                return false;
            }

            public void 访问()
            {
                try
                {
                    执行();
                    if (结束检查()) return;
                    if (访问次数 == 0)
                    {
                        //第一次访问此节点，随机模拟
                        扩展();
                        if (chiles.Count > 0)
                        {
                            var nextNode = chiles[Random.Range(0, chiles.Count)];
                            nextNode.访问();
                            for (int i = 0; i < chiles.Count; i++)
                                chiles[i].PutIn();
                            chiles.Clear();
                        }
                    }
                    else
                    {
                        if (已扩展 == false)
                        {
                            扩展();
                            已扩展 = true;
                        }
                        if (chiles.Count == 0)
                            return;
                        var nextNode = chiles.Dequeue();
                        nextNode.访问();
                        chiles.Enqueue(nextNode, -nextNode.Get_UCB());
                    }
                }
                finally
                {
                    访问次数 += 1;
                }
            }

            public void 扩展()
            {
                foreach (var operate in mcts.round.State3_Turn.NowTurn.Agent.AllCanUseOperate.Values)
                {
                    if (operate.NowState.EnumName != OperateStateEnum.t0 ||
                        operate.T0.CheckUp() == false)
                        continue;
                    ReusableList<FlatBufferBuilder> reusableList = new ReusableList<FlatBufferBuilder>(10);
                    operate.T1.节点扩展(ref reusableList);
                    for (int i = 0; i < reusableList.Count; i++)
                    {
                        var node = Node.TakeOut();
                        node.parent = this;
                        node.mcts = mcts;
                        node.operate = operate;
                        node.builder = reusableList[i];
                        node.turnTimeNum = turnTimeNum;
                        chiles.Enqueue(node, float.MinValue);
                    }
                }
                for (int i = maxTurnTimeNum - turnTimeNum; i > 0; i--)
                {
                    var node = TimeNode.TakeOut();
                    node.parent = this;
                    node.mcts = mcts;
                    node.timeNum = i;
                    node.turnTimeNum = turnTimeNum;
                    chiles.Enqueue(node, float.MinValue);
                }
            }
            public abstract void 执行();

            /// <summary>
            /// 每个回合最大的等待时间/每个为0.5s
            /// </summary>
            private static int maxTurnTimeNum = 12;
            /// <summary>
            /// 一个回合已经持续的时间
            /// </summary>
            public int turnTimeNum;


            private bool 已扩展 = false;

            public NodeBase parent;
            public PriorityQueue<NodeBase> chiles = new PriorityQueue<NodeBase>();

            protected MCTS mcts;

            public int 访问次数;


            public static float c = 1;
            public float Get_UCB()
            {
                float 探索系数 = c * Mathf.Sqrt(Mathf.Log(parent.访问次数) / 访问次数);
                float 经验系数 = 0;
                经验系数 += Effects[0].血量;
                经验系数 += Effects[1].血量;
                经验系数 -= Effects[2].血量;
                return 经验系数 + 探索系数;
            }



            public Effect[] Effects = new Effect[(int)EffectAgent.end];
            public void AddEffect(EffectAgent agent, Effect addEffect)
            {
                var byEffect = Effects[(int)agent];
                byEffect.血量 += addEffect.血量;
                Effects[(int)agent] = byEffect;
                parent?.AddEffect(agent, addEffect);
            }
            public struct Effect
            {
                public int 血量;
            }
            public enum EffectAgent
            {
                自己,
                友军,
                敌军,
                end,
            }
        }
    }
}
