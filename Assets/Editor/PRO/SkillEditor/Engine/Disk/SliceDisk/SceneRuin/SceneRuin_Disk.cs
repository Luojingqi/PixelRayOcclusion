using PRO.DataStructure;
using System.Collections.Generic;
using UnityEngine;

namespace PRO.SkillEditor
{
    public class SceneRuin_Disk : Slice_DiskBase
    {
        public Vector2Int offset;

        public BlockBase.BlockType BlockType;
        /// <summary>
        /// 破坏的坚硬度
        /// </summary>
        public int hardness = -1;
        /// <summary>
        /// 破坏的耐久度
        /// </summary>
        public int durability = int.MaxValue;
        /// <summary>
        /// 破坏的像素点
        /// </summary>
        public List<Vector2Int> pixelList = new List<Vector2Int>();

        public Sprite sprite;


        public override void UpdateFrame(SkillPlayAgent agent, int frame, int frameIndex, int trackIndex)
        {
            try
            {
                var nor = PixelPosRotate.New(agent.transform.rotation.eulerAngles);
                Vector2Int agentPos = Block.WorldToGlobal(agent.transform.position);
                foreach (var pos in pixelList)
                {
                    Vector2Int gloabPos = agentPos + nor.RotatePos(pos + offset);
                    BlockBase blockBase = null;
                    if (BlockType == BlockBase.BlockType.Block)
                        blockBase = SceneManager.Inst.NowScene.GetBlock(Block.GlobalToBlock(gloabPos));
                    else
                        blockBase = SceneManager.Inst.NowScene.GetBackground(Block.GlobalToBlock(gloabPos));
                    if (blockBase == null) continue;
                    Vector2Byte pixelPos = Block.GlobalToPixel(gloabPos);
                    blockBase.TryDestroyPixel(blockBase.GetPixel(pixelPos), hardness, durability);
                }
            }
            catch
            {
                Debug.Log("请在运行模式下查看效果：场景破坏轨道");
            }
#if UNITY_EDITOR
            if (Application.isPlaying == false)
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
                renderer.sortingOrder = 20;
            }
            renderer.sprite = sprite;
            renderer.transform.position = agent.transform.position + Block.GlobalToWorld(offset);
        }
    }
}
