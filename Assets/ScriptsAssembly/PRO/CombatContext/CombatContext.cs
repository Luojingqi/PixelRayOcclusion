using PRO.Buff.Base;
using PRO.TurnBased;
using PRO.Tool;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static PRO.CombatContextEX;
namespace PRO
{
    /// <summary>
    /// 战斗上下文
    /// </summary>
    public class CombatContext
    {
        public Role Agent { get; private set; }
        public RoleInfo AgentInfo { get; private set; } = new RoleInfo();
        public List<StartCombatEffectData> StartCombatEffectDataList = new List<StartCombatEffectData>();
        public 施法方式 施法方式type;


        public StringBuilder LogBuilder = new StringBuilder();
        public RoundFSM Round { get; private set; }
        public TurnFSM Turn { get; private set; }


        public List<CombatContext_ByAgentData> ByAgentDataList = new List<CombatContext_ByAgentData>();
        #region CombatContext对象池
        private static ObjectPool<CombatContext> pool = new ObjectPool<CombatContext>();
        public static CombatContext TakeOut()
        {
            var context = pool.TakeOut();
            return context;
        }
        public static void PutIn(CombatContext context)
        {
            context.Agent = null;
            context.LogBuilder.Clear();
            context.StartCombatEffectDataList.Clear();
            context.ClearByAgentData();
            context.Round = null;
            context.Turn = null;
            pool.PutIn(context);
        }
        #endregion

        public class CombatContext_ByAgentData
        {
            public Role Agent;
            public RoleInfo AgentInfo = new RoleInfo();
            public List<StartCombatEffectData> StartCombatEffectDataList = new List<StartCombatEffectData>();
            public EndCombatEffectData EndCombatEffectData;
            public InjuryEstimationPanelC InjuryEstimation;
            public bool PlayAffectedAnimation = true;
            public StringBuilder LogBuilder = new StringBuilder();

            #region CombatContext_ByAgentData对象池
            private static ObjectPool<CombatContext_ByAgentData> pool = new ObjectPool<CombatContext_ByAgentData>();
            public static CombatContext_ByAgentData TakeOut()
            {
                var agentData = pool.TakeOut();
               agentData.InjuryEstimation = InjuryEstimationPanelC.pool.TakeOutT();
                return agentData;
            }
            public static void PutIn(CombatContext_ByAgentData agentData)
            {
                agentData.Agent.UnSelect();
                agentData.Agent = null;
                agentData.StartCombatEffectDataList.Clear();
                agentData.EndCombatEffectData = new EndCombatEffectData();
                agentData.LogBuilder.Clear();
                InjuryEstimationPanelC.pool.PutIn(agentData.InjuryEstimation.gameObject);
                agentData.InjuryEstimation = null;
                agentData.PlayAffectedAnimation = true;
                pool.PutIn(agentData);
            }
            #endregion
        }



        public void SetAgent(Role role, RoundFSM Round, TurnFSM Turn)
        {
            Agent = role;
            RoleInfo.Clone(role.Info, AgentInfo);
            LogBuilder.Insert(0, $"{Agent.Name}：");
            this.Round = Round;
            this.Turn = Turn;
        }

        public void SetAgent(CombatContext context)
        {
            SetAgent(context.Agent, context.Round, context.Turn);
        }

        public void AddByAgent(Role byRole, bool triggerBuff_选中)
        {
            var byAgentData = CombatContext_ByAgentData.TakeOut();
            ByAgentDataList.Add(byAgentData);
            byAgentData.Agent = byRole;
            RoleInfo.Clone(byRole.Info, byAgentData.AgentInfo);
            byAgentData.LogBuilder.Append($"{byRole.Name}：被攻击。");
            foreach (var data in StartCombatEffectDataList)
                byAgentData.StartCombatEffectDataList.Add(data);
            if (triggerBuff_选中)
            {
                int agentIndex = ByAgentDataList.Count - 1;
                Agent.ForEachBuffApplyEffect(BuffTriggerType.技能选中敌人, this, agentIndex);
                byAgentData.Agent.ForEachBuffApplyEffect(BuffTriggerType.被技能选中, this, agentIndex);
                EndCombatEffectData data = new EndCombatEffectData();
                data.命中率 = AgentInfo.命中率.Value - byAgentData.AgentInfo.闪避率.Value;
                data.暴击率 = AgentInfo.暴击率.Value + Mathf.Max(data.命中率 - 1f, 0);
                byAgentData.EndCombatEffectData = data;
                byAgentData.InjuryEstimation.Open(this, agentIndex);
                byRole.Select();
            }
        }
        #region 选择全部与取消选择全部等
        public void ClearByAgentData()
        {
            foreach (var data in ByAgentDataList)
                CombatContext_ByAgentData.PutIn(data);
            ByAgentDataList.Clear();
        }
        #endregion
        /// <summary>
        /// 添加技能伤害
        /// </summary>
        public void Calculate_战斗技能初始化(施法方式 type, System.Span<StartCombatEffectData> startDatas)
        {
            施法方式type = type;
            foreach (var data in startDatas)
            {
                StartCombatEffectDataList.Add(data);
                LogBuilder.Append($"{data.type}{data.value}，");
            }
            LogBuilder.EndCommaToPeriod();
            LogBuilder.Append('\n');
        }
        /// <summary>
        /// 添加技能伤害
        /// </summary>
        public void Calculate_战斗技能初始化(施法方式 type, List<StartCombatEffectData> startDatas)
        {
            施法方式type = type;
            foreach (var data in startDatas)
            {
                StartCombatEffectDataList.Add(data);
                LogBuilder.Append($"{data.type}{data.value}，");
            }
            LogBuilder.EndCommaToPeriod();
            LogBuilder.Append('\n');
        }
        /// <summary>
        /// 计算前请将攻击人的StartHurtData填充到受击者的StartHurtData中
        /// 然后将每个受击者单独的StartHurtData转换填充到每名受击者的EndHurtData中
        /// </summary>
        /// <param name="index"></param>
        private EndCombatEffectData Calculate_命中与暴击(int index)
        {
            CombatContext_ByAgentData byAgentData = ByAgentDataList[index];
            RoleInfo byAgentInfo = byAgentData.AgentInfo;

            EndCombatEffectData data = new EndCombatEffectData();
            data.命中率 = AgentInfo.命中率.Value - byAgentInfo.闪避率.Value;
            data.暴击率 = AgentInfo.暴击率.Value + Mathf.Max(data.命中率 - 1f, 0);
            Calculate_CombatEffect(byAgentData, out data.护甲, out data.血量);
            Calculate_CombatEffect(byAgentData, out data.护甲_暴击, out data.血量_暴击, AgentInfo.暴击效果.Value);
            ByAgentDataList[index].EndCombatEffectData = data;
            return data;
        }

        private void Calculate_CombatEffect(CombatContext_ByAgentData byAgentData, out int 护甲影响, out int 血量影响, float coefficient = 1)
        {
            RoleInfo byAgentInfo = byAgentData.AgentInfo;
            护甲影响 = 0;
            血量影响 = 0;
            foreach (var effectData in byAgentData.StartCombatEffectDataList)
            {
                int effectValue = (int)(effectData.value * coefficient);
                if (effectData.type == 属性.治疗)
                {

                    if (血量影响 + effectValue + byAgentInfo.血量.Value > byAgentInfo.最大血量.Value)
                    {
                        护甲影响 += effectValue - (byAgentInfo.最大血量.Value - byAgentInfo.血量.Value) - byAgentInfo.抗性Array[(int)effectData.type].Value;
                        血量影响 += byAgentInfo.最大血量.Value - byAgentInfo.血量.Value;
                    }
                    else
                    {
                        血量影响 += effectValue;
                    }
                }
                else if (effectData.type == 属性.真实)
                {
                    血量影响 -= effectValue - byAgentInfo.抗性Array[(int)effectData.type].Value;
                }
                else
                {
                    if (护甲影响 + effectValue > byAgentInfo.临时护甲.Value)
                    {
                        血量影响 -= 护甲影响 + effectValue - byAgentInfo.临时护甲.Value - byAgentInfo.抗性Array[(int)effectData.type].Value;
                        护甲影响 -= byAgentInfo.临时护甲.Value;
                    }
                    else
                    {
                        护甲影响 -= effectValue;
                    }
                }
            }
        }

        /// <summary>
        /// 结算伤害
        /// </summary>
        public void Calculate_最终结算()
        {
            //遍历所有被攻击人
            for (int i = 0; i < ByAgentDataList.Count; i++)
            {
                CombatContext_ByAgentData byAgentData = ByAgentDataList[i];

                byAgentData.Agent.ForEachBuffApplyEffect(BuffTriggerType.受到战斗效果前, this, i);

                Calculate_命中与暴击(i);

                var byEndCombatEffectData = byAgentData.EndCombatEffectData;
                if (Random.Range(0, 1000) <= (int)(byEndCombatEffectData.命中率 * 1000))
                {
                    byEndCombatEffectData.is命中 = true;
                    var byAgent = byAgentData.Agent;
                    if (Random.Range(0, 1000) > (int)(byEndCombatEffectData.暴击率 * 1000))
                    {
                        byAgent.Info.临时护甲.Value += byEndCombatEffectData.护甲;
                        byAgent.Info.血量.Value += byEndCombatEffectData.血量;

                        byAgentData.LogBuilder.Append($"命中。");
                        if (byEndCombatEffectData.护甲 != 0) { byAgentData.LogBuilder.Append($"护甲{GetSign(byEndCombatEffectData.护甲)}，"); }
                        if (byEndCombatEffectData.血量 != 0) { byAgentData.LogBuilder.Append($"血量{GetSign(byEndCombatEffectData.血量)}，"); }
                        byAgentData.LogBuilder.EndCommaToPeriod();
                    }
                    else
                    {
                        byEndCombatEffectData.is暴击 = true;
                        byAgent.Info.临时护甲.Value += byEndCombatEffectData.护甲_暴击;
                        byAgent.Info.血量.Value += byEndCombatEffectData.血量_暴击;
                        byAgentData.LogBuilder.Append("暴击！");
                        if (byEndCombatEffectData.护甲_暴击 != 0) { byAgentData.LogBuilder.Append($"护甲{GetSign(byEndCombatEffectData.护甲_暴击)}，"); }
                        if (byEndCombatEffectData.血量_暴击 != 0) { byAgentData.LogBuilder.Append($"血量{GetSign(byEndCombatEffectData.血量_暴击)}，"); }
                        byAgentData.LogBuilder.EndCommaToPeriod();

                    }
                    byAgentData.EndCombatEffectData = byEndCombatEffectData;

                    byAgentData.Agent.ForEachBuffApplyEffect(BuffTriggerType.受到战斗效果后, this, i);
                }
                else
                {
                    byAgentData.LogBuilder.Append($"未命中。");
                }

                if (byAgentData.PlayAffectedAnimation)
                {
                    if (byEndCombatEffectData.护甲 + byEndCombatEffectData.血量 < 0) byAgentData.Agent.Play被攻击Animation();
                    else if (byEndCombatEffectData.护甲 + byEndCombatEffectData.血量 > 0) byAgentData.Agent.Play被治疗Animation();
                }
                byAgentData.Agent.UpdateBuffUI();
            }
            Agent.UpdateBuffUI();
        }



        public void Calculate_战斗技能初始化_直接对发起人结算(System.Span<StartCombatEffectData> startDatas)
        {
            施法方式type = 施法方式.直接触发;
            foreach (var data in startDatas)
            {
                StartCombatEffectDataList.Add(data);
                LogBuilder.Append($"{data.type}{data.value}，");
            }
            ByAgentDataList.Clear();
            AddByAgent(Agent, false);
            ByAgentDataList[0].LogBuilder.Clear();
            Calculate_最终结算();
            LogBuilder.Append(ByAgentDataList[0].LogBuilder);
            ClearByAgentData();
        }
    }

    public static class CombatContextEX
    {
        public static void EndCommaToPeriod(this StringBuilder sb)
        {
            if (sb[sb.Length - 1] == '，') sb[sb.Length - 1] = '。';
        }

        public static string GetSign(int value)
        {
            if (value > 0) return $"+{value}";
            else if (value < 0) return $"{value}";
            else return $" {value}";
        }
    }
}
