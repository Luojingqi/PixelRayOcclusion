using PRO.Buff.Base;
using PRO.Buff.Base.IBuff;
using System;

namespace PRO.Buff
{
    /// <summary>
    /// 
    /// </summary>
    public class Buff_2_6 : BuffBase<Buff_2_6>, IBuff_UI, IBuff_叠加, IBuff_独有
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
        public string Info => "叠加上限为5，水属性伤害每级伤害提升20%。受到水属性伤害后buff消失。。";

        public override RoleBuffUpdateCheckBase<Buff_2_6> UpdateCheck => null;

        public override void ApplyEffect(CombatContext context, int index)
        {
            CombatContext c = CombatContext.TakeOut();
            c.SetAgent(context);
            c.LogBuilder.Append("受到中毒伤害：");
            c.Calculate_战斗技能初始化_直接对发起人结算(stackalloc StartCombatEffectData[]
            {
                new (){ value = StackNumber * 3,type = 属性.毒}
            });
            context.LogBuilder.Append(c.LogBuilder);
            CombatContext.PutIn(c);
            SetActive(false);
        }

        public override void SetActive(bool active)
        {
            base.SetActive(active);
            if (active) { }
            else stackNumber = 0;
        }
    }
}
