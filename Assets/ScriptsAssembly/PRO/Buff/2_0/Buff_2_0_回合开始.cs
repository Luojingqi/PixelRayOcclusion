using PRO.Buff.Base.BuffAux;

namespace PRO.Buff
{
    public partial class Buff_2_0
    {
        private Buff_2_0_回合开始 buff_回合开始;
        private class Buff_2_0_回合开始 : BuffAuxBase_回合开始
        {
            private Buff_2_0 buff;
            public Buff_2_0_回合开始(Buff_2_0 buff) { this.buff = buff; }
            public override void ApplyEffect(CombatContext context, int index)
            {
                context.LogBuilder.AppendLine($"{buff.Name}下降15%。");
                buff.Proportion -= 0.15f;
            }
        }
    }
}