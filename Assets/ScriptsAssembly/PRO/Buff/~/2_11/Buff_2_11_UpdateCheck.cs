using PRO.Buff.Base;
using PRO;
using UnityEngine;

namespace PRO.Buff
{
    public partial class Buff_2_11
    {
        private class Buff_2_11_UpdateCheck : RoleBuffUpdateCheckBase<Buff_2_11>
        {
            public Buff_2_11_UpdateCheck(Buff_2_11 buff) : base(buff)
            {

            }

            public override void Update()
            {
                Role agent = buff.Agent;
                int num = 0;
                Vector2Int global = agent.GlobalPos;
                for (int y = agent.nav.AgentMould.size.y - 1; y >= 0; y--)
                    for (int x = agent.nav.AgentMould.size.x - 1; x >= 0; x--)
                    {
                        Vector2Int now = global + new Vector2Int(x, y) - agent.nav.AgentMould.offset;
                        Pixel pixel_Block = agent.Scene.GetPixel(BlockBase.BlockType.Block, now);
                        if (pixel_Block != null && pixel_Block.typeInfo.collider) num++;
                    }

                bool newState = num >= agent.nav.AgentMould.area * 2 / 5;
                if (newState == buff.GetActive()) return;
                buff.SetActive(newState);
                agent.UpdateBuffUI();
            }
        }
    }
}
