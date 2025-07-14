using PRO.Buff.Base;
using PRO;
using PRO.DataStructure;
using UnityEngine;

namespace PRO.Buff
{
    public partial class Buff_4_0
    {
        private class Buff_4_0_UpdateCheck : RoleBuffUpdateCheckBase<Buff_4_0>
        {
            public Buff_4_0_UpdateCheck(Buff_4_0 buff) : base(buff)
            {
            }

            public override void Update()
            {
                Role agent = buff.Agent;
                var buff_湿润 = agent.GetBuff<Buff_2_0>();
                if (buff_湿润 != null && buff_湿润.GetActive()) return;
                int num = 0;
                Vector2Int global = agent.GlobalPos;
                for (int y = agent.nav.AgentMould.size.y + 1; y >= 0; y--)
                    for (int x = agent.nav.AgentMould.size.x + 1; x >= 0; x--)
                    {
                        Vector2Int now = global + new Vector2Int(x, y) - agent.nav.AgentMould.offset - Vector2Int.one;
                        Pixel pixel = agent.Scene.GetPixel(BlockBase.BlockType.Block, now);
                        if (pixel != null && pixel.typeInfo.typeName == "水")
                        {
                            num++;
                        }
                    }

                if (num > 0) return;
                CombatContext context = CombatContext.TakeOut();
                context.SetAgent(agent, null, null);
                context.LogBuilder.Append("鱼离开了水");
                context.Calculate_战斗技能初始化_直接对发起人结算(stackalloc StartCombatEffectData[]
                {
                    new(属性.真实 , 99)
                });
                CombatContext.PutIn(context);
                agent.RemoveBuff(buff);
                agent.UpdateBuffUI();
            }
        }
    }
}
