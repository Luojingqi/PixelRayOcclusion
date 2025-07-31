using PRO.Buff.Base;
using System;

namespace PRO.Buff
{
    /// <summary>
    /// 高度差
    /// </summary>
    public class Buff_1_0 : BuffBase
    {
        public override BuffTriggerType TriggerType => BuffTriggerType.造成战斗效果前;

        private static int min高度差 = 10;
        public override void ApplyEffect(CombatContext context, int index)
        {
            if (context.施法方式type != 施法方式.远程) return;
            var byAgentData = context.ByAgentDataList[index];
            int 高度差num = (int)(context.Agent.transform.position.y / Pixel.Size) - (int)(byAgentData.Agent.transform.position.y / Pixel.Size);
            if (高度差num < min高度差) return;
            //超过10格触发
            高度差num -= min高度差;
            byAgentData.AgentInfo.闪避率.Value_额外 -= (0.05 + 0.015 * 高度差num);
            byAgentData.LogBuilder.Append($"触发“{Config.Name}”：{byAgentData.Agent.Name} 闪避率 减少{(0.05f + 0.015f * 高度差num) * 100:F1}%。");
        }

        public override void InitValue()
        {
            
        }
    }
}
