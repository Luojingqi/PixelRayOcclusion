using PRO.Buff.Base;
using PRO.Buff.Base.IBuff;
using System;

namespace PRO.Buff
{
    /// <summary>
    /// 燃烧
    /// </summary>
    public partial class Buff_2_1 : BuffBase<Buff_2_1>, IBuff_UI, IBuff_叠加, IBuff_独有
    {
        public override BuffTriggerType TriggerType => BuffTriggerType.受到战斗效果前;

        public override string Name => "燃烧";

        public int stackNumber = 0;
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
        public int StackNumberMax { get; set; }
        public string Info => "回合结束时层数转换为烧伤。";

        public override RoleBuffUpdateCheckBase<Buff_2_1> UpdateCheck => updateCheck;
        private Buff_2_1_UpdateCheck updateCheck;

        public override void ApplyEffect(CombatContext context, int index)
        {
            var byAgentData = context.ByAgentDataList[index];
            for (int i = 0; i < byAgentData.StartCombatEffectDataList.Count; i++)
            {
                var startData = byAgentData.StartCombatEffectDataList[i];
                if (startData.type == 属性.水)
                {
                    int replaceAmount = Math.Min(StackNumber, startData.value);
                    StackNumber -= replaceAmount;
                    startData.value -= replaceAmount;
                    byAgentData.LogBuilder.Append($"触发{Name}，燃烧层数1:1替换水属性伤害，替换{replaceAmount}点伤害，剩余{StackNumber}层燃烧。");
                    byAgentData.StartCombatEffectDataList[i] = startData;
                }
            }
        }

        public Buff_2_2 buff_2_2 = new Buff_2_2() { };

        public Buff_2_1()
        {
            updateCheck = new Buff_2_1_UpdateCheck(this);
            buff_回合结束 = new Buff_2_1_回合结束(this);
        }

        public override void RoleAddThis(Role role)
        {
            base.RoleAddThis(role);
            StackNumberMax = role.nav.AgentMould.area;
            role.AddBuff(buff_回合结束);
            role.AddBuff(buff_2_2);
        }
        public override void RoleRemoveThis()
        {
            Agent.RemoveBuff(buff_回合结束);
            Agent.RemoveBuff(buff_2_2);
            base.RoleRemoveThis();
        }
        public override void SetActive(bool active)
        {
            base.SetActive(active);
            buff_回合结束.SetActive(active);
            if (active)
            {
                BuffEx.Conflic(this, Agent.GetBuff<Buff_2_0>());
                BuffEx.Conflic(this, Agent.GetBuff<Buff_2_9>());
            }
            else stackNumber = 0;
        }


    }
}
