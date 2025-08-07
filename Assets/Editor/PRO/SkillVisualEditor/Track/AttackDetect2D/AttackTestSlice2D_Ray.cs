using Sirenix.OdinInspector;
using UnityEngine;

namespace PRO.SkillEditor
{
    internal class AttackTestSlice2D_Ray : AttackTestSlice2DBase
    {
        public override void Init(Slice_DiskBase sliceDisk)
        {
            base.Init(sliceDisk);
            if (sliceDisk.startFrame == -1)
            {
                Name = "射线";
            }
        }

        private AttackTestSlice2D_Ray_Disk diskData => DiskData as AttackTestSlice2D_Ray_Disk;

        public override void DrawGizmo(SkillPlayAgent agent)
        {
            Gizmos.matrix = Matrix4x4.TRS(agent.transform.position, Quaternion.Euler(0, 0, agent.transform.rotation.eulerAngles.z), Vector3.one)
                        * Matrix4x4.TRS(diskData.position, Quaternion.identity, Vector3.one);
            Gizmos.DrawRay(Vector3.zero, Direction * Distance);
            Gizmos.DrawSphere(Direction * Distance, 0.03f);
        }

        public override void DrawHandle(SkillPlayAgent agent)
        {
            base.DrawHandle(agent);
            var rts = Matrix4x4.TRS(agent.transform.position, Quaternion.Euler(0, 0, agent.transform.rotation.eulerAngles.z), Vector3.one) *
                       Matrix4x4.TRS(DiskData_AT.position, Quaternion.identity, Vector3.one);
            Vector3 direction = diskData.direction * diskData.distance;
            if (HandlePosition(rts, ref direction))
            {
                diskData.direction = direction.normalized;
                diskData.distance = direction.magnitude;
            }
        }

        [LabelText("方向")]
        [ShowInInspector]
        public Vector2 Direction { get => diskData.direction; set => diskData.direction = value; }

        [LabelText("距离")]
        [ShowInInspector]
        public float Distance { get => diskData.distance; set => diskData.distance = value; }

        public override void UnSelect()
        {
            base.UnSelect();
            diskData.direction.Normalize();
        }


    }
}
