using PRO.Buff.Base;
using PRO;
using PRO.DataStructure;
using UnityEngine;

namespace PRO.Buff
{
    public partial class Buff_2_3
    {
        private class Buff_2_3_UpdateCheck : RoleBuffUpdateCheckBase<Buff_2_3>
        {
            public Buff_2_3_UpdateCheck(Buff_2_3 buff) : base(buff)
            {
            }

            public override void Update()
            {
                if (buff.StackNumber >= buff.StackNumberMax) return;
                Role agent = buff.Agent;
                int num = 0;
                Vector2Int global = agent.GlobalPos;
                for (int y = agent.nav.AgentMould.size.y + 1; y >= 0; y--)
                    for (int x = agent.nav.AgentMould.size.x + 1; x >= 0; x--)
                    {
                        Vector2Int now = global + new Vector2Int(x, y) - agent.nav.AgentMould.offset - Vector2Int.one;
                        Pixel pixel = agent.Scene.GetPixel(BlockBase.BlockType.Block, now);
                        if (pixel != null && pixel.typeInfo.typeName == "毒液" && buff.StackNumber + num < buff.StackNumberMax)
                        {
                            pixel.blockBase.SetPixel(Pixel.空气.Clone(pixel.pos));
                            num++;
                        }
                    }

                if (num == 0) return;
                buff.StackNumber += num;
                agent.UpdateBuffUI();
            }
        }
    }
}
