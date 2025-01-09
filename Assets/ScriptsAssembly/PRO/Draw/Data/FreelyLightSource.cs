using PRO.Disk;
using UnityEngine;

namespace PRO
{
    public class FreelyLightSource
    {
        private Vector2Int? gloabPos;
        private LightSourceInfo info;
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
        public LightSourceInfo Info
        {
            get { return info; }
            set
            {
                if (value == null) return;
                info = value;
            }
        }

        private FreelyLightSource() { }
        public static FreelyLightSource New(Vector3Int color, string lightSourceName)
        {
            LightSourceInfo info = BlockMaterial.GetLightSourceInfo(lightSourceName);
            if (info == null) return null;
            FreelyLightSource fls = new FreelyLightSource();
            fls.info = info;
            fls.color = color;
            return fls;
        }
    }
}
