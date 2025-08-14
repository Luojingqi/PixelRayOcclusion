using UnityEngine;

namespace PRO
{
    public class Particle_水 : Particle
    {
        public override void TakeOut(SceneEntity scene)
        {
            base.TakeOut(scene);
            Renderer.color = Pixel.GetPixelColorInfo("水色0").color;
            UpdateEvent += UpdateEventAction;
            CollisionEnterEvent += CollisionEnterEventAction;
            gameObject.layer = GameLayer.Particle_Block_Role.ToUnityLayer();
        }

        public override void PutIn()
        {
            base.PutIn();
        }

        private void UpdateEventAction()
        {
            Vector2Int global = Block.WorldToGlobal(transform.position);
            Pixel pixel = Scene.GetPixel(BlockBase.BlockType.Block, global);
            if (pixel.typeInfo.fluidType == 1)
            {
                for (int i = 1; i <= 25; i++)
                {
                    Pixel upPixel = Scene.GetPixel(BlockBase.BlockType.Block, global + new Vector2Int(0, i));
                    if (upPixel.typeInfo.typeName == "空气")
                    {
                        upPixel.Replace("水", 0);
                        break;
                    }
                }
                ParticleManager.Inst.GetPool(loadPath).PutIn(this);
            }
        }

        private void CollisionEnterEventAction(Collision2D c)
        {
            Pixel upPixel = Scene.GetPixel(BlockBase.BlockType.Block, Block.WorldToGlobal(transform.position));
            if (upPixel.typeInfo.typeName == "空气")
            {
                upPixel.Replace("水", 0);
            }
            ParticleManager.Inst.GetPool(loadPath).PutIn(this);
        }
    }
}
