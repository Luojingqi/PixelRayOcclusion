using PRO.Buff.Base;
using PRO;
using UnityEngine;

namespace PRO.Buff
{
    public partial class Buff_2_0
    {
        private class Buff_2_0_UpdateCheck : RoleBuffUpdateCheckBase<Buff_2_0>
        {
            public Buff_2_0_UpdateCheck(Buff_2_0 buff) : base(buff)
            {
            }

            public override void Update()
            {
                if (buff.Proportion >= buff.ProportionMax) return;
                Role agent = buff.Agent;
                float minAdd = 1f / agent.nav.AgentMould.area;
                float nowP = buff.Proportion;
                bool change = false;
                Vector2Int global = agent.GlobalPos;
                for (int y = agent.nav.AgentMould.size.y - 1; y >= 0; y--)
                    for (int x = agent.nav.AgentMould.size.x - 1; x >= 0; x--)
                    {
                        Vector2Int now = global + new Vector2Int(x, y) - agent.nav.AgentMould.offset;
                        Pixel pixel = agent.Scene.GetPixel(BlockBase.BlockType.Block, now);
                        if (pixel != null && pixel.typeInfo.typeName == "水")
                            if (nowP <= buff.ProportionMax)
                            {
                                pixel.blockBase.SetPixel(Pixel.空气.Clone(pixel.pos));
                                nowP += minAdd;
                                change = true;
                            }
                    }
                if (change == false) return;
                buff.Proportion = Mathf.Clamp(nowP, 0, 1);
                agent.UpdateBuffUI();
            }
        }
    }
}
