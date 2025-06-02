using PRO.Buff.Base;
using PRO.Buff.Base.IBuff;
using PRO;
using System;

namespace PRO.Buff
{
    /// <summary>
    /// 高度差
    /// </summary>
    public class Buff_1_0 : BuffBase<Buff_1_0>, IBuff_独有
    {
        public override BuffTriggerType TriggerType => BuffTriggerType.技能选中敌人;

        public override string Name => "高度差";

        public override RoleBuffUpdateCheckBase<Buff_1_0> UpdateCheck => null;

        public override void ApplyEffect(CombatContext context, int index)
        {
            if (context.施法方式type != 施法方式.远程) return;
            var byAgentData = context.ByAgentDataList[index];
            int 高度差num = (int)(context.Agent.transform.position.y / Pixel.Size) - (int)(byAgentData.Agent.transform.position.y / Pixel.Size);
            int sign = Math.Sign(高度差num);
            //超过10格触发
            高度差num = Math.Abs(高度差num) - 10;
            if (高度差num >= 0)
            {
                byAgentData.AgentInfo.闪避率.Value -= sign * (0.05f + 0.015f * 高度差num);
                byAgentData.LogBuilder.Append($"触发“{Name}”：闪避率{(sign < 0 ? "增加" : "减少")}{(0.05f + 0.015f * 高度差num) * 100:F1}%。");//，造成伤害{(sign > 0 ? "增加" : "减少")}{1}。");
            }
        }

        public Buff_1_0()
        {
            active = true;
        }
    }
}
