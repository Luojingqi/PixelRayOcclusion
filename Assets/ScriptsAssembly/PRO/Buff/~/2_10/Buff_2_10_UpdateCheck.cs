using PRO.Buff.Base;
using PRO;
using UnityEngine;

namespace PRO.Buff
{
    public partial class Buff_2_10
    {
        private class Buff_2_10_UpdateCheck : RoleBuffUpdateCheckBase<Buff_2_10>
        {
            public Buff_2_10_UpdateCheck(Buff_2_10 buff) : base(buff)
            {

            }

            public override void Update()
            {
                Role agent = buff.Agent;
                int num = 0;
                Vector2Int global = agent.GlobalPos;
                for (int y = agent.nav.AgentMould.size.y - 1; y >= agent.nav.AgentMould.center.y; y--)
                    for (int x = agent.nav.AgentMould.size.x - 1; x >= 0; x--)
                    {
                        Vector2Int now = global + new Vector2Int(x, y) - agent.nav.AgentMould.offset;
                        Pixel pixel_Block = agent.Scene.GetPixel(BlockBase.BlockType.Block, now);
                        if (pixel_Block != null && pixel_Block.typeInfo.typeName != "空气") num++;
                    }

                if (num < agent.nav.AgentMould.area / 2) return;
                buff.StackNumber += 1;
                agent.UpdateBuffUI();
            }
        }
    }
}
