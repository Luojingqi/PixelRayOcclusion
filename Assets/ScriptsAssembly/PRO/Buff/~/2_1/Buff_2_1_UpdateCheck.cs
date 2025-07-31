using PRO.Buff.Base;
using PRO;
using UnityEngine;

namespace PRO.Buff
{
    public partial class Buff_2_1
    {
        private class Buff_2_1_UpdateCheck : RoleBuffUpdateCheckBase<Buff_2_1>
        {
            public Buff_2_1_UpdateCheck(Buff_2_1 buff) : base(buff)
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
                        Pixel pixel_Block = agent.Scene.GetPixel(BlockBase.BlockType.Block, now);
                        Pixel pixel_Background = agent.Scene.GetPixel(BlockBase.BlockType.BackgroundBlock, now);
                        if (pixel_Block != null && pixel_Block.typeInfo.typeName == "火焰") num++;
                        if (pixel_Background != null && pixel_Background.typeInfo.typeName == "火焰") num++;
                    }

                if (num == 0 || num <= buff.StackNumber) return;
                buff.StackNumber = num;
                agent.UpdateBuffUI();
            }
        }
    }
}
