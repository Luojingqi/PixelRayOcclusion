using PRO.Buff.Base;
using UnityEngine;

namespace PRO.Buff
{
    /// <summary>
    /// 湿润
    /// </summary>
    public partial class Buff_2_0 : BuffBase, IBuff_比例
    {
        public override BuffTriggerType TriggerType => BuffTriggerType.不触发;
        public float Proportion
        {
            get { return proportion; }
            set
            {
                if (value <= 0) { SetAtive(false); return; }
                if (value > ProportionMax) proportion = ProportionMax;
                else proportion = value;
                SetAtive(true);
            }
        }
        public float proportion;
        public float ProportionMax { get; set; } = 1;

        public override void ApplyEffect(CombatContext context, int index) { }

        public Buff_2_0()
        {
            buff_受到战斗效果前 = new(this);
            buff_回合开始 = new(this);
            AddChildBuuff(buff_受到战斗效果前);
            AddChildBuuff(buff_回合开始);
        }

        private bool active;
        private void SetAtive(bool active)
        {
            if (this.active == active) return;
            this.active = active;
            if (active)
            {
               // BuffEx.Conflic(this, Agent.GetBuff<Buff_2_1>());
            }
            else
            {
                proportion = 0;
            }
        }



        public override void Update()
        {
            if (Proportion >= ProportionMax) return;
            float minAdd = 1f / Agent.nav.AgentMould.area;
            float nowP = Proportion;
            bool change = false;
            Vector2Int global = Agent.GlobalPos;
            for (int y = Agent.nav.AgentMould.size.y - 1; y >= 0; y--)
                for (int x = Agent.nav.AgentMould.size.x - 1; x >= 0; x--)
                {
                    Vector2Int now = global + new Vector2Int(x, y) - Agent.nav.AgentMould.offset;
                    Pixel pixel = Agent.Scene.GetPixel(BlockBase.BlockType.Block, now);
                    if (pixel != null && pixel.typeInfo.typeName == "水")
                        if (nowP <= ProportionMax)
                        {
                            pixel.blockBase.SetPixel(Pixel.空气.Clone(pixel.pos));
                            nowP += minAdd;
                            change = true;
                        }
                }
            if (change == false) return;
            Proportion = Mathf.Clamp(nowP, 0, 1);
        }
    }
}
