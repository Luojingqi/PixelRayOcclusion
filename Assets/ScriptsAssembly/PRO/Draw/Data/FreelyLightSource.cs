using PRO.Disk;
using UnityEngine;

namespace PRO
{
    public class FreelyLightSource
    {
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
                        SceneManager.Inst.NowScene.GetBlock(Block.GlobalToBlock(gloabPos.Value))?.FreelyLightSourceHash.Remove(this);
                }
                else
                {
                    if (gloabPos == null)
                        SceneManager.Inst.NowScene.GetBlock(Block.GlobalToBlock(value.Value))?.FreelyLightSourceHash.Add(this);
                    else
                    {
                        Vector2Int blockPos0 = Block.GlobalToBlock(value.Value);
                        Vector2Int blockPos1 = Block.GlobalToBlock(gloabPos.Value);
                        if (blockPos0 != blockPos1)
                        {
                            SceneManager.Inst.NowScene.GetBlock(blockPos0)?.FreelyLightSourceHash.Add(this);
                            SceneManager.Inst.NowScene.GetBlock(blockPos1)?.FreelyLightSourceHash.Remove(this);
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
        public static FreelyLightSource New(Vector3Int color, int radius)
        {
            if (radius <= 0 || radius > BlockMaterial.LightRadiusMax) return null;
            FreelyLightSource fls = new FreelyLightSource();
            fls.radius = radius;
            fls.color = color;
            return fls;
        }
        public static FreelyLightSource New(PixelColorInfo info)
        {
            if (info == null) return null;
            FreelyLightSource fls = new FreelyLightSource();
            fls.radius = info.luminousRadius;
            fls.color = new Vector3Int(info.color.r, info.color.g, info.color.b);
            return fls;
        }
        public static FreelyLightSource New(Color32 color, int radius)
        {
            if (radius <= 0 || radius > BlockMaterial.LightRadiusMax) return null;
            FreelyLightSource fls = new FreelyLightSource();
            fls.radius = radius;
            fls.color = new Vector3Int(color.r, color.g, color.b);
            return fls;
        }
    }
}
