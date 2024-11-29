using System.Collections.Generic;
using UnityEngine;

namespace PRO.SkillEditor
{
    internal class SceneRuinSlice : SliceBase
    {
        public SceneRuinSlice(SceneRuin_Disk sliceDisk) : base(sliceDisk)
        {
            if (diskData.texture != null)
                Background = new UnityEngine.UIElements.StyleBackground(diskData.texture);
        }

        public override void DrawGizmo() { }


        private SceneRuin_Disk diskData => DiskData as SceneRuin_Disk;
        public Texture2D Texture
        {
            get { return diskData.texture; }
            set
            {
                diskData.texture = value;
                Background = new UnityEngine.UIElements.StyleBackground(diskData.texture);
                for (int y = 0; y < diskData.texture.height; y++)
                    for (int x = 0; x < diskData.texture.width; x++)
                    {
                        Color color = diskData.texture.GetPixel(x, y);
                        if (color.a != 1) continue;
                        Color32 color32 = new Color32((byte)(color.r * 255), (byte)(color.g * 255), (byte)(color.b * 255), 255);
                        if (diskData.RuinPixelDic.TryGetValue(color32.ToString(), out List<Vector2Int> pixelList) == false)
                        {
                            pixelList = new List<Vector2Int>();
                            diskData.RuinPixelDic.Add(color32.ToString(), pixelList);
                        }
                        pixelList.Add(new Vector2Int(x, y));
                    }

            }
        }
    }
}
