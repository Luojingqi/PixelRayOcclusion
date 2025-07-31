using PRO.Buff.Base;
using PRO.Buff.Base.IBuff;
using PRO;
using System;
using UnityEngine;

namespace PRO.Buff
{
    /// <summary>
    /// 窒息
    /// </summary>
    public partial class Buff_2_10 : BuffBase<Buff_2_10>, IBuff_UI, IBuff_叠加, IBuff_独有
    {
        public override BuffTriggerType TriggerType => BuffTriggerType.回合开始时;

        public override string Name => "窒息";

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
        public int StackNumberMax { get; set; } = int.MaxValue;
        public string Info => "回合开始时，受到每层窒息造成20%的真伤。";

        public override RoleBuffUpdateCheckBase<Buff_2_10> UpdateCheck => updateCheck;
        private Buff_2_10_UpdateCheck updateCheck;

        public override void ApplyEffect(CombatContext context, int index)
        {
            var c = CombatContext.TakeOut();
            c.SetAgent(context);
            c.LogBuilder.Append($"触发窒息：受到{StackNumber * 0.2f * 100:F0}%真伤。");
            c.Calculate_战斗技能初始化_直接对发起人结算(stackalloc StartCombatEffectData[]
            {
                new (属性.真实 , (int)(StackNumber * 0.2f * context.AgentInfo.最大血量.Value))
            });
            context.LogBuilder.Append(c.LogBuilder);
            CombatContext.PutIn(c);

            Role agent = context.Agent;
            Vector2Int global = agent.GlobalPos;
            int num = 0;
            for (int y = agent.nav.AgentMould.size.y - 1; y >= agent.nav.AgentMould.center.y; y--)
                for (int x = agent.nav.AgentMould.size.x - 1; x >= 0; x--)
                {
                    Vector2Int now = global + new Vector2Int(x, y) - agent.nav.AgentMould.offset;
                    Pixel pixel_Block = agent.Scene.GetPixel(BlockBase.BlockType.Block, now);
                    if (pixel_Block != null && pixel_Block.typeInfo.typeName != "空气") num++;
                }

            if (num < agent.nav.AgentMould.area / 2)
                StackNumber -= 1;
            else
                StackNumber += 1;
        }

        public Buff_2_10()
        {
           // updateCheck = new Buff_2_10_UpdateCheck(this);
        }

        public override void SetActive(bool active)
        {
            base.SetActive(active);
            if (active) { }
            else stackNumber = 0;
        }
    }
}
