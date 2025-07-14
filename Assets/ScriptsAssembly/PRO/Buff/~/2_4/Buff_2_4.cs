using PRO.Buff.Base;
using PRO.Buff.Base.IBuff;
using System;

namespace PRO.Buff
{
    /// <summary>
    /// 中毒
    /// </summary>
    public class Buff_2_4 : BuffBase<Buff_2_4>, IBuff_UI, IBuff_叠加, IBuff_独有
    {
        public override BuffTriggerType TriggerType => BuffTriggerType.回合开始时;

        public override string Name => "中毒";

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
        public int StackNumberMax { get; set; } = int.MaxValue;
        public string Info => "每回合开始时受到层数的毒属性伤害。";

        public override RoleBuffUpdateCheckBase<Buff_2_4> UpdateCheck => null;

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
