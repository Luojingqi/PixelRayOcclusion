using PRO.Buff.Base.BuffAux;

namespace PRO.Buff
{
    public partial class Buff_2_3
    {
        private Buff_2_3_回合结束 buff_回合结束;
        private class Buff_2_3_回合结束 : BuffAuxBase_回合结束
        {
            public Buff_2_3 buff;
            public Buff_2_3_回合结束(Buff_2_3 buff)
            {
                this.buff = buff;
            }

            public override void ApplyEffect(CombatContext context, int index)
            {
                context.LogBuilder.AppendLine($"{buff.Name}转换为{buff.buff_2_4.Name}，层数{buff.StackNumber}");
                buff.buff_2_4.StackNumber += buff.StackNumber;
                buff.StackNumber = 0;
            }
        }
    }
}
