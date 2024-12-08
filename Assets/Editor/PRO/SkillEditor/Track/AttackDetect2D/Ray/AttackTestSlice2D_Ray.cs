using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace PRO.SkillEditor
{
    internal class AttackTestSlice2D_Ray : AttackTestSlice2DBase
    {
        public AttackTestSlice2D_Ray(SliceBase_Disk sliceDisk) : base(sliceDisk)
        {
            Name = "射线";
        }



        public override void DrawGizmo(SkillPlayAgent agent)
        {
            Gizmos.matrix = Matrix4x4.TRS(DiskData_AT.position, DiskData_AT.rotation, DiskData_AT.scale);
            Gizmos.DrawRay(DiskData_AT.position, Direction);
        }

        public override void DrawHandle(SkillPlayAgent agent)
        {
            
        }

        [LabelText("方向")]
        [ShowInInspector]
        public Vector3 Direction { get => (DiskData as AttackTestSlice2D_Ray_Disk).direction; set => (DiskData as AttackTestSlice2D_Ray_Disk).direction = value; }

    }
}
