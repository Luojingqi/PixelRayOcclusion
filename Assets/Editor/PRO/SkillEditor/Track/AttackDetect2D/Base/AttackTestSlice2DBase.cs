using Sirenix.OdinInspector;
using UnityEngine;

namespace PRO.SkillEditor
{
    internal abstract class AttackTestSlice2DBase : SliceBase
    {
        protected AttackTestSlice2DBase(SliceBase_Disk sliceDisk) : base(sliceDisk)
        {
        }

        public AttackTestSlice2DBase_Disk DiskData_AT { get => DiskData as AttackTestSlice2DBase_Disk; }

        [LabelText("位置")]
        [ShowInInspector]
        public Vector3 Position { get => DiskData_AT.position; set => DiskData_AT.position = value; }
        [LabelText("旋转")]
        [ShowInInspector]
        public Vector3 Rotation { get => rotation; set { DiskData_AT.rotation = Quaternion.Euler(value); rotation = value; } }
        private Vector3 rotation;
        [LabelText("缩放")]
        [ShowInInspector]
        public Vector3 Scale { get => DiskData_AT.scale; set => DiskData_AT.scale = value; }
    }
}
