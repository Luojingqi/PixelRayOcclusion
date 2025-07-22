using PRO.Buff;
using PRO.Tool;
using System.Collections.Generic;
using UnityEngine;

namespace PRO
{
    public class Particle_火焰 : Particle
    {
        public override void TakeOut(SceneEntity scene)
        {
            base.TakeOut(scene);
            Renderer.color = Pixel.GetPixelColorInfo("火焰色7").color;
            UpdateEvent += UpdateEventAction;
            CollisionEnterEvent += CollisionEnterEventAction;
            RemainTimeIsZeroEvent += RemainTimeIsZeroEventAction;
            gameObject.layer = (int)GameLayer.Particle_Block_Role;
        }

        private void RemainTimeIsZeroEventAction()
        {
            DrawTool.GetCircle(Block.WorldToGlobal(transform.position), 1, ref list);
            //foreach (var pos_G in list)
            //    BuffEx.火焰(Scene, pos_G);
        }

        private List<Vector2Int> list = new List<Vector2Int>();

        private void UpdateEventAction()
        {
            if (ElapsedTime < Random.Range(200, 800)) return;
            Vector2Int global = Block.WorldToGlobal(transform.position);
            Pixel pixel = Scene.GetPixel(BlockBase.BlockType.Block, global);
            if (pixel != null && pixel.typeInfo.typeName == "水")
            {
                RemainTime = 0;
            }
        }
        private void CollisionEnterEventAction(Collision2D c)
        {
            RemainTime = 0;
        }
    }
}
