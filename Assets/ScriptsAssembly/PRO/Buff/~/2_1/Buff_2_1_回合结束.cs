using PRO.Buff.Base.BuffAux;

namespace PRO.Buff
{
    public partial class Buff_2_1
    {
        private Buff_2_1_回合结束 buff_回合结束;

        private class Buff_2_1_回合结束 : BuffAuxBase_回合开始
        {
            public Buff_2_1 buff;
            public Buff_2_1_回合结束(Buff_2_1 buff)
            {
                this.buff = buff;
            }

            public override void ApplyEffect(CombatContext context, int index)
            {
                if (buff.StackNumber == 0) return;
                buff.buff_2_2.StackNumber += buff.StackNumber;
                buff.StackNumber -= 10;
                context.LogBuilder.Append($"{buff.Name}转换为{buff.buff_2_2.Name}，{buff.Name}下降{10}层。");
            }
        }

    }
}
