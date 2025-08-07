using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;

namespace PRO.SkillEditor
{
    internal class AttackTestSlice2D_Rect : AttackTestSlice2DBase
    {
        public override void Init(Slice_DiskBase sliceDisk)
        {
            base.Init(sliceDisk);
            if (sliceDisk.startFrame == -1)
            {
                Name = "矩形";
            }
        }

        private AttackTestSlice2D_Rect_Disk diskData => DiskData as AttackTestSlice2D_Rect_Disk;

        public override void DrawGizmo(SkillPlayAgent agent)
        {
            Gizmos.matrix = Matrix4x4.TRS(agent.transform.position, Quaternion.Euler(0, 0, agent.transform.rotation.eulerAngles.z), Vector3.one) *
                            Matrix4x4.TRS(diskData.position, Quaternion.Euler(0, 0, diskData.angle), diskData.scale);
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        }

        public override void DrawHandle(SkillPlayAgent agent)
        {
            base.DrawHandle(agent);
            Quaternion rotation = Quaternion.Euler(0, 0, diskData.angle);
            if (HandleRotation(
                Matrix4x4.TRS(agent.transform.position, Quaternion.Euler(0, 0, agent.transform.rotation.eulerAngles.z), Vector3.one) *
                Matrix4x4.TRS(diskData.position, Quaternion.identity, Vector3.one),
                ref rotation))
                Angle = rotation.eulerAngles.z;

            Vector3 scale = diskData.scale;
            if (HandleScale(
                Matrix4x4.TRS(agent.transform.position, Quaternion.Euler(0, 0, agent.transform.rotation.eulerAngles.z), Vector3.one) *
                Matrix4x4.TRS(diskData.position, rotation, Vector3.one),
                ref scale))
                Scale = scale;
        }
        [LabelText("旋转")]
        [ShowInInspector]
        public float Angle { get => diskData.angle; set => diskData.angle = value % 360; }
        [LabelText("缩放")]
        [ShowInInspector]
        public Vector2 Scale { get => diskData.scale; set => diskData.scale = value; }
    }
}
