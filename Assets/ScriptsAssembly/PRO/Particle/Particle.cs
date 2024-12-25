using PRO.Tool;
using UnityEngine;

namespace PRO
{
    internal class Particle : MonoBehaviour
    {
        public SpriteRenderer Renderer { get; private set; }
        public Rigidbody2D Rigidbody { get; private set; }
        public void Init()
        {
            Renderer = GetComponent<SpriteRenderer>();
            Rigidbody = GetComponent<Rigidbody2D>();
        }





        private static Texture2DPool texture2DPool = new Texture2DPool(50, true);
        private class Texture2DPool : ObjectPoolBase<Texture2D>
        {
            public Texture2DPool(int maxNuber, bool isCanExceed) : base(maxNuber, isCanExceed)
            {
            }

            public override void Destroy(Texture2D item)
            {
                GameObject.Destroy(item);
            }

            protected override Texture2D NewObject()
            {
                var ret = new Texture2D(0, 0);
                ret.filterMode = FilterMode.Point;
                return ret;
            }
        }
    }
}
