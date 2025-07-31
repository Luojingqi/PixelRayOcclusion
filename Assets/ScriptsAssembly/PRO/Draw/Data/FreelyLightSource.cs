using PRO.Disk;
using UnityEngine;
using UnityEngine.Profiling;

namespace PRO
{
    public class FreelyLightSource : IScene
    {
        public SceneEntity Scene => _scene;
        private SceneEntity _scene;
        private Vector2Int? gloabPos;
        private int radius = 1;
        public Vector3Int color;


        public Vector2Int? GloabPos
        {
            get { return gloabPos; }
            set
            {
                if (value == null)
                {
                    if (gloabPos != null)
                        _scene.GetBlock(Block.GlobalToBlock(gloabPos.Value))?.FreelyLightSourceHash.Remove(this);
                }
                else
                {
                    if (gloabPos == null)
                        _scene.GetBlock(Block.GlobalToBlock(value.Value))?.FreelyLightSourceHash.Add(this);
                    else
                    {
                        Vector2Int blockPos0 = Block.GlobalToBlock(value.Value);
                        Vector2Int blockPos1 = Block.GlobalToBlock(gloabPos.Value);
                        if (blockPos0 != blockPos1)
                        {
                            _scene.GetBlock(blockPos0)?.FreelyLightSourceHash.Add(this);
                            _scene.GetBlock(blockPos1)?.FreelyLightSourceHash.Remove(this);
                        }
                    }
                }
                gloabPos = value;
            }
        }
        public int Radius
        {
            get { return radius; }
            set
            {
                if (value <= 0 || value > BlockMaterial.LightRadiusMax) return;
                radius = value;
            }
        }

        private FreelyLightSource() { }
        public static FreelyLightSource New(SceneEntity scene, Vector3Int color, int radius)
        {
            if (radius <= 0 || radius > BlockMaterial.LightRadiusMax) return null;
            FreelyLightSource fls = new FreelyLightSource();
            fls.radius = radius;
            fls.color = color;
            fls._scene = scene;
            return fls;
        }
        public static FreelyLightSource New(SceneEntity scene, PixelColorInfo info)
        {
            if (info == null) return null;
            FreelyLightSource fls = new FreelyLightSource();
            fls.radius = info.luminousRadius;
            fls.color = new Vector3Int(info.color.r, info.color.g, info.color.b);
            fls._scene = scene;
            return fls;
        }
        public static FreelyLightSource New(SceneEntity scene, Color32 color, int radius)
        {
            if (radius <= 0 || radius > BlockMaterial.LightRadiusMax) return null;
            FreelyLightSource fls = new FreelyLightSource();
            fls.radius = radius;
            fls.color = new Vector3Int(color.r, color.g, color.b);
            fls._scene = scene;
            return fls;
        }
    }
}
