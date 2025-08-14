using System.Collections.Generic;
using UnityEngine;

namespace PRO
{
    public class Particle_火苗 : Particle
    {
        public override void TakeOut(SceneEntity scene)
        {
            base.TakeOut(scene);
            SurviveTimeRange = new Vector2Int(500, 1500);
            Rig2D.gravityScale = Random.Range(-0.05f, -0.001f);
            UpdateEvent += UpdateEventAction;
            gameObject.layer = GameLayer.Particle.ToUnityLayer();
        }

        private void UpdateEventAction()
        {
            Renderer.sprite = spriteList[Mathf.Clamp((int)(Mathf.Pow((float)ElapsedTime / (ElapsedTime + RemainTime), 5) * spriteList.Count), 0, spriteList.Count - 1)];
        }


        public List<Sprite> spriteList = new List<Sprite>();
    }
}
