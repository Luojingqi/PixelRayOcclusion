using UnityEngine;

namespace PRO.SkillEditor
{
    internal class AttackTestSlice2D_Rect : AttackTestSlice2DBase
    {
        public AttackTestSlice2D_Rect(SliceBase_Disk sliceDisk) : base(sliceDisk)
        {
            Name = "矩形";
        }


        public override void DrawGizmo()
        {
            Gizmos.matrix = Matrix4x4.TRS(DiskData_AT.position, DiskData_AT.rotation, DiskData_AT.scale);
            Gizmos.DrawWireCube(DiskData_AT.position, new Vector3(DiskData_AT.scale.x, DiskData_AT.scale.y, 0));
        }
    }
}
