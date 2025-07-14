using PRO.Buff.Base;
using PRO.Buff.Base.IBuff;
using System;

namespace PRO.Buff
{
    /// <summary>
    /// 寒冷
    /// </summary>
    public class Buff_2_7 : BuffBase<Buff_2_7>, IBuff_UI, IBuff_叠加, IBuff_独有
    {
        public override BuffTriggerType TriggerType => BuffTriggerType.受到战斗效果后;

        public override string Name => "寒冷";

        private int stackNumber = 0;
        public int StackNumber
        {
            get { return stackNumber; }
            set
            {
                if (value <= 0) { SetActive(false); return; }
                stackNumber = Math.Min(value, StackNumberMax);
                SetActive(true);
            }
        }
        public int StackNumberMax { get; set; } = 5;
        public string Info => "叠加上限为5\r\n水属性伤害每级伤害提升30%，受到水属性伤害后buff消失。\r\n火属性伤害每级伤害下降30%，受到火属性伤害后buff消失。";

        public override RoleBuffUpdateCheckBase<Buff_2_7> UpdateCheck => null;

        public override void ApplyEffect(CombatContext context, int index)
        {
            var byAgentData = context.ByAgentDataList[index];
            for (int i = 0; i < byAgentData.StartCombatEffectDataList.Count; i++)
            {
                var startData = byAgentData.StartCombatEffectDataList[i];
                if (startData.type == 属性.水)
                {
                    startData.value = (int)(startData.value * (1 + stackNumber * 0.3f));
                    byAgentData.LogBuilder.Append($"触发寒冷buff：水属性伤害提升{stackNumber * 0.3f}。");
                    byAgentData.StartCombatEffectDataList[i] = startData;
                    SetActive(false);
                    return;
                }
                else if (startData.type == 属性.火)
                {
                    startData.value = (int)(startData.value * (1 - stackNumber * 0.3f));
                    byAgentData.LogBuilder.Append($"触发寒冷buff：火属性伤害下降{stackNumber * 0.3f}。");
                    byAgentData.StartCombatEffectDataList[i] = startData;
                    SetActive(false);
                    return;
                }
            }
        }

        public override void SetActive(bool active)
        {
            base.SetActive(active);
            if (active) { }
            else stackNumber = 0;
        }
    }
}
