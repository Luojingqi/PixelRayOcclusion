using System.Collections.Generic;
using UnityEngine;

namespace PRO.SkillEditor
{
    public class SceneRuin_Disk : SliceBase_Disk
    {
        public Vector3 position;

        public Texture2D texture;

        public Dictionary<string, List<Vector2Int>> RuinPixelDic = new Dictionary<string, List<Vector2Int>>();


        public override void UpdateFrame(SkillPlayAgent agent, int frame, int index)
        {
            foreach (var kv in RuinPixelDic)
            {
                foreach (var pos in kv.Value)
                {
                    Vector2Int gloabPos = Block.WorldToGloab(agent.transform.position + position) + pos;
                    Block block = SceneManager.Inst.NowScene.GetBlock(Block.GloabToBlock(gloabPos));
                    block.SetPixel(Pixel.TakeOut("空气", "空气色", block.GloabToPixel(gloabPos)));
                }
            }
        }
    }
}
