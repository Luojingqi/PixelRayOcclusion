using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static PRO.SkillEditor.SceneCreate_Disk;

namespace PRO.SkillEditor
{
    internal class SceneCreateSlice : SliceBase
    {
        public SceneCreateSlice(Slice_DiskBase sliceDisk) : base(sliceDisk)
        {
        }

        public override void DrawGizmo(SkillPlayAgent agent)
        {

        }

        public override void DrawHandle(SkillPlayAgent agent)
        {
            if (Tools.current != UnityEditor.Tool.Move) return;
            EditorGUI.BeginChangeCheck();
            Vector3 ret = Handles.PositionHandle(Block.GlobalToWorld(diskData.offset) + agent.transform.position, Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                diskData.offset = Block.WorldToGlobal(ret - agent.transform.position);
            }
        }
        private SceneCreate_Disk diskData => DiskData as SceneCreate_Disk;

        [LabelText("偏移")]
        [ShowInInspector]
        public Vector2Int offset { get => diskData.offset; set => diskData.offset = value; }
        [LabelText("放置的层")]
        [ShowInInspector]
        public BlockBase.BlockType BlockType { get => diskData.BlockType; set => diskData.BlockType = value; }
        [LabelText("创建的点集")]
        [ShowInInspector]
        public List<PixelData> CreatePixelList { get => diskData.CreatePixelList; set => diskData.CreatePixelList = value; }
    }
}
