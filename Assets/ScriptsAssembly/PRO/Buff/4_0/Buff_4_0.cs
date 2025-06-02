using PRO.Buff.Base;
using PRO.Buff.Base.IBuff;

namespace PRO.Buff
{
    public partial class Buff_4_0 : BuffBase<Buff_4_0>, IBuff_UI, IBuff_回合, IBuff_独有
    {
        public override BuffTriggerType TriggerType => BuffTriggerType.受到战斗效果前;

        public override string Name => "鱼形";

        public string Info => "闪避＋100%，离开水时，受到99点真实伤害";

        private int round = 0;
        public int Round
        {
            get => round;
            set
            {
                if (value <= 0) { RoleRemoveThis(); return; }
                round = value;
                SetActive(true);
            }
        }
        public override RoleBuffUpdateCheckBase<Buff_4_0> UpdateCheck => updateCheck;
        private Buff_4_0_UpdateCheck updateCheck;
        public override void ApplyEffect(CombatContext context, int index)
        {
            var byAgentData = context.ByAgentDataList[index];
            byAgentData.LogBuilder.Append($"{Name}：闪避增加100%");
            byAgentData.AgentInfo.闪避率.Value += 1f;
        }
        public Buff_4_0()
        {
            buff_回合结束 = new Buff_4_0_回合结束(this);
            updateCheck = new Buff_4_0_UpdateCheck(this);
        }
        public override void RoleAddThis(Role role)
        {
            base.RoleAddThis(role);
            role.AddBuff(buff_回合结束);
        }
        public override void RoleRemoveThis()
        {
            Agent.RemoveBuff(buff_回合结束);
            base.RoleRemoveThis();
        }
        public override void SetActive(bool active)
        {
            base.SetActive(active);
            buff_回合结束.SetActive(active);
            if (active)
            {

            }
            else
            {
                RoleRemoveThis();
            }
        }
    }
}
