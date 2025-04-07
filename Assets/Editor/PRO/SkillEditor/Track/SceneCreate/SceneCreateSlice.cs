using UnityEditor;
using UnityEngine;

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
        public SceneCreate_Disk diskData => DiskData as SceneCreate_Disk;
    }
}
