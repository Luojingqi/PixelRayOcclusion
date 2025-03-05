using PRO.DataStructure;
using System.Collections.Generic;
using UnityEngine;

namespace PRO.SkillEditor
{
    public class SceneCreate_Disk : SliceBase_Disk
    {
        public List<PixelData> CreatePixelList = new List<PixelData>();
        public Vector2Int offset;

        public override void UpdateFrame(SkillPlayAgent agent, int frame, int frameIndex, int trackIndex)
        {
            try
            {
                foreach (var data in CreatePixelList)
                {
                    Vector2Int gloabPos = Block.WorldToGlobal(agent.transform.position) + data.pos + offset;
                    Block block = SceneManager.Inst.NowScene.GetBlock(Block.GlobalToBlock(gloabPos));
                    Vector2Byte pixelPos = Block.GlobalToPixel(gloabPos);
                    Pixel pixel = Pixel.TakeOut(data.typeName, data.colorName, pixelPos);
                    block.SetPixel(pixel);
                }
            }
            catch
            {
                Debug.Log("请在运行模式下查看效果：场景创建轨道");
            }
        }


        public struct PixelData
        {
            public string typeName;
            public string colorName;
            public Vector2Int pos;
        }
    }
}
