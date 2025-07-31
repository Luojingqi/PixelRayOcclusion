using PRO.Buff.Base;
using PRO.Buff.Base.IBuff;

namespace PRO.Buff
{
    /// <summary>
    /// 掩埋
    /// </summary>
    public partial class Buff_2_11 : BuffBase<Buff_2_11>, IBuff_UI, IBuff_独有
    {
        public override BuffTriggerType TriggerType => BuffTriggerType.不触发;

        public override string Name => "掩埋";

        public string Info => "无法移动";

        public override RoleBuffUpdateCheckBase<Buff_2_11> UpdateCheck => updateCheck;
        private Buff_2_11_UpdateCheck updateCheck;

        public override void ApplyEffect(CombatContext context, int index)
        {

        }

        public Buff_2_11()
        {
            updateCheck = new Buff_2_11_UpdateCheck(this);
        }

        public override void SetActive(bool active)
        {
            base.SetActive(active);
            if (active)
            {
                Agent.Rig2D.simulated = false;
            }
            else
            {
                Agent.Rig2D.simulated = true;
            }
        }
    }
}
