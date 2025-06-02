using PRO.Buff.Base;
using PRO.Buff.Base.IBuff;
using PRO;
using PRO.DataStructure;
using UnityEngine;

namespace PRO.Buff
{
    /// <summary>
    /// 湿润
    /// </summary>
    public partial class Buff_2_0 : BuffBase<Buff_2_0>, IBuff_比例, IBuff_UI, IBuff_独有
    {
        public override BuffTriggerType TriggerType => BuffTriggerType.受到战斗效果前;
        public override string Name => "湿润";
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

        public override RoleBuffUpdateCheckBase<Buff_2_0> UpdateCheck => updateCheck;
        private Buff_2_0_UpdateCheck updateCheck;

        public override void ApplyEffect(CombatContext context, int index)
        {
            var byAgentData = context.ByAgentDataList[index];
            for (int i = 0; i < byAgentData.StartCombatEffectDataList.Count; i++)
            {
                var startData = byAgentData.StartCombatEffectDataList[i];
                if (startData.type == 属性.火)
                {
                    startData.value = (int)(startData.value * (1 - Proportion));
                    byAgentData.LogBuilder.AppendLine($"触发“{Name}”：火属性伤害下降{Proportion * 100:F0}%，buff消失。");
                    byAgentData.StartCombatEffectDataList[i] = startData;
                    Vaporize(byAgentData.Agent);
                    //触发一次buff消失
                    SetActive(false);
                    return;
                }
                else if (startData.type == 属性.冰)
                {
                    var buff_寒冷 = byAgentData.Agent.GetBuff<Buff_2_7>();
                    if (buff_寒冷 == null) continue;
                    if (Proportion >= 0.3f)
                    {
                        int num = 1;
                        if (Proportion >= 0.7f) num += 1;
                        buff_寒冷.StackNumber += num;
                    }
                    SetActive(false);
                    return;
                }
            }
        }
        public void Vaporize(Role agent)
        {
            Pixel 水蒸气实例 = Pixel.TakeOut("水蒸气", 0, new());
            Vector2Int agentGlobal = agent.GlobalPos;
            int num = (int)(Proportion * agent.nav.AgentMould.area);
            for (int y = agent.nav.AgentMould.size.y - 1; y >= 0; y--)
                for (int x = agent.nav.AgentMould.size.x - 1; x >= 0; x--)
                    if (num > 0)
                    {
                        Vector2Int pos_G = new Vector2Int(x, y) - agent.nav.AgentMould.offset + agentGlobal;
                        Block block = agent.Scene.GetBlock(Block.GlobalToBlock(pos_G));
                        if (block == null) continue;
                        num--;
                        Vector2Byte pos = Block.GlobalToPixel(pos_G);
                        block.SetPixel(水蒸气实例.Clone(pos));
                    }
            Pixel.PutIn(水蒸气实例);
        }
        public Buff_2_0()
        {
            updateCheck = new Buff_2_0_UpdateCheck(this);
            buff_回合开始 = new Buff_2_0_回合开始(this);
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
