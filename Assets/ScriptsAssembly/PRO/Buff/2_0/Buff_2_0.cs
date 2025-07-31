using PRO.Buff.Base;
using UnityEngine;

namespace PRO.Buff
{
    /// <summary>
    /// 湿润
    /// </summary>
    public partial class Buff_2_0 : BuffBase, IBuff_比例, IBuff_倒计时
    {
        public override BuffTriggerType TriggerType => BuffTriggerType.不触发;
        public float Proportion
        {
            get => proportion;
            set
            {
                if (value <= 0) { SetAtive(false); return; }
                if (value > ProportionMax) proportion = ProportionMax;
                else proportion = value;
                SetAtive(true);
            }
        }
        private float proportion;
        public float ProportionMax { get; set; }


        public float Countdown
        {
            get => countdown;
            set
            {
                if (value <= 0)
                {
                    Proportion -= 0.04f;
                    countdown = CountdownMax;
                }
                else countdown = value;
            }
        }
        private float countdown;
        public float CountdownMax { get; set; }

        public override void ApplyEffect(CombatContext context, int index) { }

        public Buff_2_0()
        {
            buff_受到战斗效果前 = new(this);
            AddChildBuuff(buff_受到战斗效果前);
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
                InitValue();
            }
        }
        public override void InitValue()
        {
            ProportionMax = 1f;
            proportion = 0f;
            CountdownMax = 1f;
            countdown = CountdownMax;
        }



        public override void Update()
        {
            Countdown -= TimeManager.deltaTime;
            if (Proportion >= ProportionMax) return;
            float minAdd = 1f / Agent.Info.NavMould.mould.area;
            float nowP = Proportion;
            bool change = false;
            Vector2Int global = Agent.GlobalPos;
            for (int y = Agent.Info.NavMould.mould.size.y - 1; y >= 0; y--)
                for (int x = Agent.Info.NavMould.mould.size.x - 1; x >= 0; x--)
                {
                    Vector2Int now = global + new Vector2Int(x, y) - Agent.Info.NavMould.mould.offset;
                    Pixel pixel = Agent.Scene.GetPixel(BlockBase.BlockType.Block, now);
                    if (pixel != null && pixel.typeInfo.typeName == "水")
                        if (nowP <= ProportionMax)
                        {
                            pixel.Replace(Pixel.空气);
                            nowP += minAdd;
                            change = true;
                        }
                        else break;
                }
            if (change == false) return;
            Proportion = System.Math.Clamp(nowP, 0, 1);
        }
    }
}
