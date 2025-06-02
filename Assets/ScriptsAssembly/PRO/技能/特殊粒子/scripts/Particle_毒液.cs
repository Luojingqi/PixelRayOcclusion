using UnityEngine;

namespace PRO
{
    public class Particle_毒液 : Particle
    {
        public override void TakeOut(SceneEntity scene)
        {
            base.TakeOut(scene);
            Renderer.color = BlockMaterial.GetPixelColorInfo("毒液色0").color;
            UpdateEvent += UpdateEventAction;
            CollisionEnterEvent += CollisionEnterEventAction;
            gameObject.layer = (int)GameLayer.Particle_Block_Role;
        }

        private void UpdateEventAction(Particle p)
        {
            if (p.ElapsedTime < Random.Range(200, 800)) return;
            Vector2Int global = Block.WorldToGlobal(p.transform.position);
            Pixel pixel = p.Scene.GetPixel(BlockBase.BlockType.Block, global);
            if (pixel.typeInfo.fluidType == 1)
            {
                for (int i = 1; i <= 8; i++)
                {
                    Pixel upPixel = p.Scene.GetPixel(BlockBase.BlockType.Block, global + new Vector2Int(0, i));
                    if (upPixel.typeInfo.typeName == "空气")
                    {
                        upPixel.blockBase.SetPixel(Pixel.TakeOut("毒液", 0, upPixel.pos));
                        ParticleManager.Inst.GetPool(p.loadPath).PutIn(p);
                        return;
                    }
                }
            }
        }
        private void CollisionEnterEventAction(Particle p, Collision2D c)
        {
            Pixel upPixel = p.Scene.GetPixel(BlockBase.BlockType.Block, Block.WorldToGlobal(p.transform.position));
            if (upPixel.typeInfo.typeName == "空气")
            {
                upPixel.blockBase.SetPixel(Pixel.TakeOut("毒液", 0, upPixel.pos));
                ParticleManager.Inst.GetPool(p.loadPath).PutIn(p);
                return;
            }
        }
    }
}
