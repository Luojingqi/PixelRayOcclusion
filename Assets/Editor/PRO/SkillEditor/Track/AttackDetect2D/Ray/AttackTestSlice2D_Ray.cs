using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace PRO.SkillEditor
{
    internal class AttackTestSlice2D_Ray : AttackTestSlice2DBase
    {
        public AttackTestSlice2D_Ray(Slice_DiskBase sliceDisk) : base(sliceDisk)
        {
            if (sliceDisk.startFrame == -1)
            {
                Name = "射线";
            }
        }

        private AttackTestSlice2D_Ray_Disk diskData => DiskData as AttackTestSlice2D_Ray_Disk;

        public override void DrawGizmo(SkillPlayAgent agent)
        {
            Gizmos.matrix = Matrix4x4.TRS(agent.transform.position, agent.transform.rotation, agent.transform.lossyScale) * Matrix4x4.TRS(diskData.position, diskData.rotation, diskData.scale);
            Gizmos.DrawRay(Vector3.zero, Direction);
        }

        public override void DrawHandle(SkillPlayAgent agent)
        {

        }

        [LabelText("方向")]
        [ShowInInspector]
        public Vector3 Direction { get => diskData.direction; set => diskData.direction = value; }

    }
}
