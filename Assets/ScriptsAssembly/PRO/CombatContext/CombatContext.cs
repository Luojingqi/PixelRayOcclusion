using Google.FlatBuffers;
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


        public StringBuilder LogBuilder = new StringBuilder(50);


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
            RoleInfo.CloneValue(RoleInfo.empty, context.AgentInfo);
            context.LogBuilder.Clear();
            context.StartCombatEffectDataList.Clear();
            for (int i = 0; i < context.ByAgentDataList.Count; i++)
                CombatContext_ByAgentData.PutIn(context.ByAgentDataList[i]);
            context.ByAgentDataList.Clear();
            pool.PutIn(context);
        }
        #endregion

        #region 序列化与反序列化
        public Offset<Flat.CombatContextData> ToDisk(FlatBufferBuilder builder)
        {
            var roleGuidOffset = builder.CreateString(Agent.GUID);
            var roleInfoOffset = AgentInfo.ToDisk(builder);
            Flat.CombatContextData.StartStartCombatEffectDataListVector(builder, StartCombatEffectDataList.Count);
            for (int i = 0; i < StartCombatEffectDataList.Count; i++)
                StartCombatEffectDataList[i].ToDisk(builder);
            var startCombatEffectDataListOffset = builder.EndVector();
            var logBuilderOffset = builder.CreateString(LogBuilder.ToString());
            System.Span<int> byAgentDataListOffsetArray = stackalloc int[ByAgentDataList.Count];
            for (int i = 0; i < ByAgentDataList.Count; i++)
                byAgentDataListOffsetArray[i] = ByAgentDataList[i].ToDisk(builder).Value;
            var byAgentDataListOffset = builder.CreateVector_Offset(byAgentDataListOffsetArray);
            Flat.CombatContextData.StartCombatContextData(builder);
            Flat.CombatContextData.AddRoleGuid(builder, roleGuidOffset);
            Flat.CombatContextData.AddRoleInfo(builder, roleInfoOffset);
            Flat.CombatContextData.AddStartCombatEffectDataList(builder, startCombatEffectDataListOffset);
            Flat.CombatContextData.AddCastASpellType(builder, (int)施法方式type);
            Flat.CombatContextData.AddLogBuilder(builder, logBuilderOffset);
            Flat.CombatContextData.AddByAgentDataList(builder, byAgentDataListOffset);
            return Flat.CombatContextData.EndCombatContextData(builder);
        }
        public static CombatContext ToRAM(Flat.CombatContextData diskData, SceneEntity scene)
        {
            var data = TakeOut();
            data.Agent = scene.GetRole(diskData.RoleGuid);
            data.AgentInfo.ToRAM(diskData.RoleInfo.Value);
            for (int i = diskData.StartCombatEffectDataListLength - 1; i >= 0; i--)
                data.StartCombatEffectDataList.Add(StartCombatEffectData.ToRAM(diskData.StartCombatEffectDataList(i).Value));
            data.施法方式type = (施法方式)diskData.CastASpellType;
            data.LogBuilder.Append(diskData.LogBuilder);
            for (int i = diskData.ByAgentDataListLength - 1; i >= 0; i--)
                data.ByAgentDataList.Add(CombatContext_ByAgentData.ToRAM(diskData.ByAgentDataList(i).Value, scene));
            return data;
        }
        #endregion

        public class CombatContext_ByAgentData
        {
            public Role Agent;
            public RoleInfo AgentInfo = new RoleInfo();
            public List<StartCombatEffectData> StartCombatEffectDataList = new List<StartCombatEffectData>();
            public EndCombatEffectData EndCombatEffectData;
            public bool PlayAffectedAnimation = true;
            public StringBuilder LogBuilder = new StringBuilder(50);

            #region CombatContext_ByAgentData对象池
            private static ObjectPool<CombatContext_ByAgentData> pool = new ObjectPool<CombatContext_ByAgentData>();
            public static CombatContext_ByAgentData TakeOut() => pool.TakeOut();
            public static void PutIn(CombatContext_ByAgentData agentData)
            {
                agentData.Agent = null;
                RoleInfo.CloneValue(RoleInfo.empty, agentData.AgentInfo);
                agentData.StartCombatEffectDataList.Clear();
                agentData.EndCombatEffectData = new EndCombatEffectData();
                agentData.LogBuilder.Clear();
                agentData.PlayAffectedAnimation = true;
                pool.PutIn(agentData);
            }
            #endregion

            #region 序列化与反序列化
            public Offset<Flat.CombatContext_ByAgentData> ToDisk(FlatBufferBuilder builder)
            {
                var roleGuidOffset = builder.CreateString(Agent.GUID);
                var roleInfoOffset = AgentInfo.ToDisk(builder);
                Flat.CombatContext_ByAgentData.StartStartCombatEffectDataListVector(builder, StartCombatEffectDataList.Count);
                for (int i = 0; i < StartCombatEffectDataList.Count; i++)
                    StartCombatEffectDataList[i].ToDisk(builder);
                var startCombatEffectDataListOffset = builder.EndVector();
                var logBuidlderOffset = builder.CreateString(LogBuilder.ToString());
                Flat.CombatContext_ByAgentData.StartCombatContext_ByAgentData(builder);
                Flat.CombatContext_ByAgentData.AddRoleGuid(builder, roleGuidOffset);
                Flat.CombatContext_ByAgentData.AddRoleInfo(builder, roleInfoOffset);
                Flat.CombatContext_ByAgentData.AddStartCombatEffectDataList(builder, startCombatEffectDataListOffset);
                Flat.CombatContext_ByAgentData.AddPlayAffectedAnimation(builder, PlayAffectedAnimation);
                Flat.CombatContext_ByAgentData.AddLogBuilder(builder, logBuidlderOffset);
                return Flat.CombatContext_ByAgentData.EndCombatContext_ByAgentData(builder);
            }
            public static CombatContext_ByAgentData ToRAM(Flat.CombatContext_ByAgentData diskData, SceneEntity scene)
            {
                CombatContext_ByAgentData data = TakeOut();
                data.Agent = scene.GetRole(diskData.RoleGuid);
                data.AgentInfo.ToRAM(diskData.RoleInfo.Value);
                for (int i = diskData.StartCombatEffectDataListLength - 1; i >= 0; i--)
                    data.StartCombatEffectDataList.Add(StartCombatEffectData.ToRAM(diskData.StartCombatEffectDataList(i).Value));
                data.PlayAffectedAnimation = diskData.PlayAffectedAnimation;
                data.LogBuilder.Append(diskData.LogBuilder);
                return data;
            }
            #endregion
        }



        public void SetAgent(Role role)
        {
            Agent = role;
            RoleInfo.CloneValue(role.Info, AgentInfo);
            LogBuilder.Insert(0, $"{Agent.Name}：");
        }

        public void AddByAgent(Role byRole)
        {
            var byAgentData = CombatContext_ByAgentData.TakeOut();
            ByAgentDataList.Add(byAgentData);
            byAgentData.Agent = byRole;
            RoleInfo.CloneValue(byRole.Info, byAgentData.AgentInfo);
            byAgentData.LogBuilder.Append($"{byRole.Name}：被攻击。");
            foreach (var data in StartCombatEffectDataList)
                byAgentData.StartCombatEffectDataList.Add(data);
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
            data.暴击率 = AgentInfo.暴击率.Value + System.Math.Max(data.命中率 - 1f, 0);
            Calculate_CombatEffect(byAgentData, out data.护甲, out data.血量);
            Calculate_CombatEffect(byAgentData, out data.护甲_暴击, out data.血量_暴击, 2.0);
            ByAgentDataList[index].EndCombatEffectData = data;
            return data;
        }

        private void Calculate_CombatEffect(CombatContext_ByAgentData byAgentData, out int 护甲影响, out int 血量影响, double coefficient = 1.0)
        {
            RoleInfo byAgentInfo = byAgentData.AgentInfo;
            护甲影响 = 0;
            血量影响 = 0;
            foreach (var effectData in byAgentData.StartCombatEffectDataList)
            {
                int effectValue = (int)(effectData.value * coefficient);
                switch (effectData.type)
                {
                    case 属性.治疗: 血量影响 += effectValue; break;
                    case 属性.护甲修复: 护甲影响 += effectValue; break;
                    case 属性.真实: 血量影响 -= effectValue; break;
                    default:
                        if (护甲影响 + effectValue > byAgentInfo.护甲.Value)
                        {
                            血量影响 -= 护甲影响 + effectValue - byAgentInfo.护甲.Value - byAgentInfo.抗性Array[(int)effectData.type].Value;
                            护甲影响 -= byAgentInfo.护甲.Value;
                        }
                        else
                            护甲影响 -= effectValue;
                        break;
                }
            }
        }

        /// <summary>
        /// 结算伤害
        /// </summary>
        public void Calculate_最终结算()
        {
            Agent.ForEachBuffApplyEffect(BuffTriggerType.攻击前, this, -1);
            //遍历所有被攻击人
            for (int i = 0; i < ByAgentDataList.Count; i++)
            {
                CombatContext_ByAgentData byAgentData = ByAgentDataList[i];
                Agent.ForEachBuffApplyEffect(BuffTriggerType.造成战斗效果前, this, i);
                byAgentData.Agent.ForEachBuffApplyEffect(BuffTriggerType.受到战斗效果前, this, i);

                Calculate_命中与暴击(i);

                var byEndCombatEffectData = byAgentData.EndCombatEffectData;
                if (Random.Range(0, 1000) <= (int)(byEndCombatEffectData.命中率 * 1000))
                {
                    byEndCombatEffectData.is命中 = true;
                    var byAgent = byAgentData.Agent;
                    if (Random.Range(0, 1000) > (int)(byEndCombatEffectData.暴击率 * 1000))
                    {
                        byAgent.Info.护甲.Value_基础 += byEndCombatEffectData.护甲;
                        byAgent.Info.血量.Value_基础 += byEndCombatEffectData.血量;

                        byAgentData.LogBuilder.Append($"命中。");
                        if (byEndCombatEffectData.护甲 != 0) { byAgentData.LogBuilder.Append($"护甲{GetSign(byEndCombatEffectData.护甲)}，"); }
                        if (byEndCombatEffectData.血量 != 0) { byAgentData.LogBuilder.Append($"血量{GetSign(byEndCombatEffectData.血量)}，"); }
                        byAgentData.LogBuilder.EndCommaToPeriod();
                    }
                    else
                    {
                        byEndCombatEffectData.is暴击 = true;
                        byAgent.Info.护甲.Value_基础 += byEndCombatEffectData.护甲_暴击;
                        byAgent.Info.血量.Value_基础 += byEndCombatEffectData.血量_暴击;
                        byAgentData.LogBuilder.Append("暴击！");
                        if (byEndCombatEffectData.护甲_暴击 != 0) { byAgentData.LogBuilder.Append($"护甲{GetSign(byEndCombatEffectData.护甲_暴击)}，"); }
                        if (byEndCombatEffectData.血量_暴击 != 0) { byAgentData.LogBuilder.Append($"血量{GetSign(byEndCombatEffectData.血量_暴击)}，"); }
                        byAgentData.LogBuilder.EndCommaToPeriod();
                    }
                    byAgentData.EndCombatEffectData = byEndCombatEffectData;

                    byAgentData.Agent.ForEachBuffApplyEffect(BuffTriggerType.受到战斗效果后, this, i);
                    Agent.ForEachBuffApplyEffect(BuffTriggerType.造成战斗效果后, this, i);
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

            Agent.ForEachBuffApplyEffect(BuffTriggerType.攻击后, this, -1);
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
            AddByAgent(Agent);
            var byData = ByAgentDataList[0];
            byData.LogBuilder.Clear();
            Calculate_CombatEffect(byData, out byData.EndCombatEffectData.护甲, out byData.EndCombatEffectData.血量);
            Agent.Info.护甲.Value_基础 += byData.EndCombatEffectData.护甲;
            Agent.Info.血量.Value_基础 += byData.EndCombatEffectData.血量;
            Agent.ForEachBuffApplyEffect(BuffTriggerType.受到战斗效果后, this, 0);
            LogBuilder.Append(ByAgentDataList[0].LogBuilder);
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
