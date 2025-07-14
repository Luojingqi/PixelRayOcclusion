using PRO.Buff.Base;

namespace PRO.Buff
{
    public partial class Buff_2_0
    {
        private Buff_2_0_回合开始 buff_回合开始;
        private class Buff_2_0_回合开始 : BuffBase, IChildBuff
        {
            private Buff_2_0 buff;
            public Buff_2_0_回合开始(Buff_2_0 buff) { this.buff = buff; }

            public override BuffTriggerType TriggerType => BuffTriggerType.回合开始时;

            public override void ApplyEffect(CombatContext context, int index)
            {
                if (buff.active == false) return;
                context.LogBuilder.AppendLine($"{buff.Config.Name}下降15%。");
                buff.Proportion -= 0.15f;
            }
        }
    }
}