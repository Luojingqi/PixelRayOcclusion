using PRO.DataStructure;
using PRO.Tool;
using System.Collections.Generic;
using UnityEngine;

namespace PRO.SkillEditor
{
    public class SceneRuin_Disk : SliceBase_Disk
    {
        public Vector2Int offset;

        public Sprite sprite;

        public Dictionary<string, List<Vector2Int>> RuinPixelDic = new Dictionary<string, List<Vector2Int>>();


        public override void UpdateFrame(SkillPlayAgent agent, int frame, int frameIndex, int trackIndex)
        {
            try
            {
                foreach (var kv in RuinPixelDic)
                {
                    foreach (var pos in kv.Value)
                    {
                        Vector2Int gloabPos = Block.WorldToGlobal(agent.transform.position) + pos + offset;
                        Block block = SceneManager.Inst.NowScene.GetBlock(Block.GlobalToBlock(gloabPos));
                        Vector2Byte pixelPos = Block.GlobalToPixel(gloabPos);
                        Pixel pixel = Pixel.TakeOut("空气", 0, pixelPos);
                        block.SetPixel(pixel);
                        block.DrawPixelAsync(pixelPos, pixel.colorInfo.color);
                    }
                }
            }
            catch
            {
                Debug.Log("请在运行模式下查看效果：场景破坏轨道");
            }
#if UNITY_EDITOR
            EditorShow(agent, trackIndex);
#endif
        }
        public void EditorShow(SkillPlayAgent agent, int trackIndex)
        {
            Transform trans = agent.transform.Find($"场景破坏轨道{trackIndex}");
            SpriteRenderer renderer = null;
            if (trans == null)
            {
                renderer = new GameObject($"场景破坏轨道{trackIndex}").AddComponent<SpriteRenderer>();
                renderer.transform.parent = agent.transform;
                renderer.transform.position = Vector3.zero;
            }
            else
            {
                renderer = trans.GetComponent<SpriteRenderer>();
            }
            renderer.sprite = sprite;
            renderer.transform.position = agent.transform.position + Block.GlobalToWorld(offset);
        }
    }
}
