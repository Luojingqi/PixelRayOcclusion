using Sirenix.OdinInspector;
using UnityEngine;

namespace PRO.SkillEditor
{
    internal class AttackTestSlice2D_Circle : AttackTestSlice2DBase
    {
        public override void Init(Slice_DiskBase sliceDisk)
        {
            base.Init(sliceDisk);
            if (sliceDisk.startFrame == -1)
            {
                Name = "圆形";
            }
        }

        private AttackTestSlice2D_Circle_Disk diskData => DiskData as AttackTestSlice2D_Circle_Disk;

        public override void DrawGizmo(SkillPlayAgent agent)
        {
            Gizmos.matrix = Matrix4x4.TRS(agent.transform.position, Quaternion.Euler(0, 0, agent.transform.rotation.eulerAngles.z), Vector3.one) *
                            Matrix4x4.TRS(diskData.position, Quaternion.identity, Vector3.one);
            Gizmos.DrawWireSphere(Vector3.zero, diskData.radius);
        }

        public override void DrawHandle(SkillPlayAgent agent)
        {
            base.DrawHandle(agent);

            Vector3 scale = Vector3.one * radius;
            if (HandleScale(
                Matrix4x4.TRS(agent.transform.position, Quaternion.Euler(0, 0, agent.transform.rotation.eulerAngles.z), Vector3.one) *
                Matrix4x4.TRS(diskData.position, Quaternion.identity, Vector3.one),
                ref scale))
                radius = scale.x;
        }
        [LabelText("半径")]
        [ShowInInspector]
        public float radius { get => diskData.radius; set => diskData.radius = value * Mathf.Sign(value); }
    }
}
