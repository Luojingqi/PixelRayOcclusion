using UnityEditor;
using UnityEngine;

namespace PRO.SkillEditor
{
    internal class AttackTestSlice2D_Rect : AttackTestSlice2DBase
    {
        public AttackTestSlice2D_Rect(SliceBase_Disk sliceDisk) : base(sliceDisk)
        {
            Name = "矩形";
        }

        private AttackTestSlice2D_Rect_Disk diskData => DiskData as AttackTestSlice2D_Rect_Disk;

        public override void DrawGizmo(SkillPlayAgent agent)
        {
            Gizmos.matrix = Matrix4x4.TRS(diskData.position + agent.transform.position, diskData.rotation * agent.transform.rotation, V3mV3(diskData.scale, agent.transform.lossyScale));
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        }

        public override void DrawHandle(SkillPlayAgent agent)
        {
            HandlePosition(agent, diskData.rotation, ref diskData.position);
            HandleRotation(agent, diskData.position, ref diskData.rotation);
            HandleScale(agent, diskData.position, diskData.rotation, ref diskData.scale);
        }
    }
}
