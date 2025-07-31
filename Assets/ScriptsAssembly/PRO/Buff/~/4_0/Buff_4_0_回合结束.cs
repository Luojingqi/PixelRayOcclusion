using PRO.Buff.Base.BuffAux;

namespace PRO.Buff
{
    public partial class Buff_4_0
    {
        private Buff_4_0_回合结束 buff_回合结束;
        private class Buff_4_0_回合结束 : BuffAuxBase_回合结束
        {
            private Buff_4_0 buff;
            public Buff_4_0_回合结束(Buff_4_0 buff) { this.buff = buff; }
            public override void ApplyEffect(CombatContext context, int index)
            {
            }
        }
    }
}
