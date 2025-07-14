using PRO.Buff.Base;
using PRO.Buff.Base.IBuff;
using PRO;
using PRO.DataStructure;
using UnityEngine;

namespace PRO.Buff
{
    /// <summary>
    /// 浸油
    /// </summary>
    public partial class Buff_2_9 : BuffBase<Buff_2_9>, IBuff_比例, IBuff_UI, IBuff_独有
    {
        public override BuffTriggerType TriggerType => BuffTriggerType.受到战斗效果前;
        public override string Name => "浸油";
        public float Proportion
        {
            get { return proportion; }
            set
            {
                if (value <= 0) { SetActive(false); return; }
                if (value > ProportionMax) proportion = ProportionMax;
                else proportion = value;
                SetActive(true);
            }
        }
        public float proportion;
        public float ProportionMax { get; set; } = 1;

        public string Info => "每回合开始时比例下降15%。";

        public override RoleBuffUpdateCheckBase<Buff_2_9> UpdateCheck => updateCheck;
        private Buff_2_9_UpdateCheck updateCheck;

        public override void ApplyEffect(CombatContext context, int index)
        {
            var byAgentData = context.ByAgentDataList[index];
            for (int i = 0; i < byAgentData.StartCombatEffectDataList.Count; i++)
            {
                var startData = byAgentData.StartCombatEffectDataList[i];
                if (startData.type == 属性.火)
                {
                    startData.value = (int)(startData.value * (1 + Proportion));
                    byAgentData.LogBuilder.AppendLine($"触发“{Name}”：火属性伤害提升{Proportion * 100:F0}%，buff消失。");
                    byAgentData.StartCombatEffectDataList[i] = startData;
                    SetActive(false);
                    return;
                }
            }
        }

        public Buff_2_9()
        {
            updateCheck = new Buff_2_9_UpdateCheck(this);
            buff_回合开始 = new Buff_2_9_回合开始(this);
        }

        public override void RoleAddThis(Role role)
        {
            base.RoleAddThis(role);
            role.AddBuff(buff_回合开始);

        }
        public override void RoleRemoveThis()
        {
            Agent.RemoveBuff(buff_回合开始);
            base.RoleRemoveThis();
        }
        public override void SetActive(bool active)
        {
            base.SetActive(active);
            buff_回合开始.SetActive(active);
            if (active) BuffEx.Conflic(this, Agent.GetBuff<Buff_2_1>());
            else proportion = 0;
        }
    }
}
