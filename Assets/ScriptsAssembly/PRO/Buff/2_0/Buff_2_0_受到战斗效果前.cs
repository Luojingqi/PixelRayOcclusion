using PRO.Buff.Base;
using PRO.DataStructure;
using UnityEngine;

namespace PRO.Buff
{
    public partial class Buff_2_0
    {
        private Buff_2_0_受到战斗效果前 buff_受到战斗效果前;
        private class Buff_2_0_受到战斗效果前 : BuffBase, IChildBuff
        {
            private Buff_2_0 buff;
            public Buff_2_0_受到战斗效果前(Buff_2_0 buff) { this.buff = buff; }

            public override BuffTriggerType TriggerType => BuffTriggerType.受到战斗效果前;

            public override void ApplyEffect(CombatContext context, int index)
            {
                if (buff.active == false) return;
                var byAgentData = context.ByAgentDataList[index];
                for (int i = 0; i < byAgentData.StartCombatEffectDataList.Count; i++)
                {
                    var startData = byAgentData.StartCombatEffectDataList[i];
                    if (startData.type == 属性.火)
                    {
                        startData.value = (int)(startData.value * (1 - buff.Proportion));
                        byAgentData.LogBuilder.AppendLine($"触发“{Name}”：火属性伤害下降{buff.Proportion * 100:F0}%，buff消失。");
                        byAgentData.StartCombatEffectDataList[i] = startData;
                        Vaporize(byAgentData.Agent);
                        //触发一次buff消失
                        RoleRemoveThis();
                        return;
                    }
                    else if (startData.type == 属性.冰)
                    {
                        //var buff_寒冷 = byAgentData.Agent.GetBuff<Buff_2_7>();
                        //if (buff_寒冷 == null) continue;
                        //if (Proportion >= 0.3f)
                        //{
                        //    int num = 1;
                        //    if (Proportion >= 0.7f) num += 1;
                        //    buff_寒冷.StackNumber += num;
                        //}
                        //SetActive(false);
                        return;
                    }
                }
            }
            public void Vaporize(Role agent)
            {
                var typeInfo = Pixel.GetPixelTypeInfo("水蒸气");
                var colorInfo = Pixel.GetPixelColorInfo(typeInfo.availableColors[0]);
                Vector2Int agentGlobal = agent.GlobalPos;
                int num = (int)(buff.Proportion * agent.nav.AgentMould.area);
                for (int y = agent.nav.AgentMould.size.y - 1; y >= 0; y--)
                    for (int x = agent.nav.AgentMould.size.x - 1; x >= 0; x--)
                        if (num > 0)
                        {
                            Vector2Int pos_G = new Vector2Int(x, y) - agent.nav.AgentMould.offset + agentGlobal;
                            Block block = agent.Scene.GetBlock(Block.GlobalToBlock(pos_G));
                            if (block == null) continue;
                            num--;
                            Vector2Byte pos = Block.GlobalToPixel(pos_G);
                            block.GetPixel(pos).Replace(typeInfo, colorInfo);
                        }
            }
        }
    }
}
