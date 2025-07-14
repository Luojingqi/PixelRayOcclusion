using PRO.Buff.Base;
using PRO.Buff.Base.IBuff;
using PRO;
using PRO.SkillEditor;
using System;

namespace PRO.Buff
{
    /// <summary>
    /// 沾毒
    /// </summary>
    public partial class Buff_2_3 : BuffBase<Buff_2_3>, IBuff_叠加, IBuff_UI, IBuff_独有
    {
        public override BuffTriggerType TriggerType => BuffTriggerType.受到战斗效果前;
        public override string Name => "沾毒";

        public string Info => "回合结束时转换为中毒";

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

        public int StackNumberMax { get; set; }

        public override RoleBuffUpdateCheckBase<Buff_2_3> UpdateCheck => updateCheck;
        private Buff_2_3_UpdateCheck updateCheck;
        public override void ApplyEffect(CombatContext context, int index)
        {
            var data = context.ByAgentDataList[index];
            //if (data.StartHurtData.属性 == StartCombatEffectData.Hurt属性.火)

            //data.StartHurtData *= (1 - Proportion);
            //data.LogBuilder.AppendLine($"触发“{Name}”：火属性伤害下降{Proportion * 100:F0}%，buff消失。");
            //Vaporize(data.Agent);
            ////触发一次buff消失
            //SetActive(false);
            //Agent.UpdateBuffUI();
            //  Explosion(data.Agent);

        }
        public void Explosion(Role agent)
        {
            Particle particle = ParticleManager.Inst.GetPool("通用0").TakeOut(agent.Scene);
            Skill_Disk skill_Disk = AssetManagerEX.LoadSkillDisk("爆炸/爆炸");
            particle.SkillPlayAgent.Skill = skill_Disk;
            particle.RemainTime = skill_Disk.FrameTime * skill_Disk.MaxFrame;
            //Pixel 水蒸气实例 = Pixel.TakeOut("水蒸气", 0, new());
            //Vector2Int agentGlobal = agent.GlobalPos;
            //int num = (int)(Proportion * agent.Nav.AgentMould.area);
            //for (int y = agent.Nav.AgentMould.size.y - 1; y >= 0; y--)
            //    for (int x = agent.Nav.AgentMould.size.x - 1; x >= 0; x--)
            //        if (num > 0)
            //        {
            //            Vector2Int pos_G = new Vector2Int(x, y) - agent.Nav.AgentMould.offset + agentGlobal;
            //            Block block = SceneManager.Inst.NowScene.GetBlock(Block.GlobalToBlock(pos_G));
            //            if (block == null) continue;
            //            num--;
            //            Vector2Byte pos = Block.GlobalToPixel(pos_G);
            //            block.SetPixel(水蒸气实例.Clone(pos));
            //        }
            //Pixel.PutIn(水蒸气实例);
        }

        public Buff_2_4 buff_2_4 = new Buff_2_4();
        public Buff_2_3()
        {
            updateCheck = new Buff_2_3_UpdateCheck(this);
            buff_回合结束 = new Buff_2_3_回合结束(this);
        }
        public override void RoleAddThis(Role role)
        {
            base.RoleAddThis(role);
            StackNumberMax = role.nav.AgentMould.area;
            role.AddBuff(buff_回合结束);
            role.AddBuff(buff_2_4);
        }
        public override void RoleRemoveThis()
        {
            Agent.RemoveBuff(buff_回合结束);
            Agent.RemoveBuff(buff_2_4);
            base.RoleRemoveThis();
        }
        public override void SetActive(bool active)
        {
            base.SetActive(active);
            buff_回合结束.SetActive(active);
            if (active) { }// Buff_2_1.Conflic(this, Agent.GetBuff<Buff_2_1>());
            else stackNumber = 0;
        }


    }
}
