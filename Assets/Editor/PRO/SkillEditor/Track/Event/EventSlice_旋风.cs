using Sirenix.OdinInspector;
using UnityEngine;

namespace PRO.SkillEditor
{
    internal class EventSlice_旋风 : EventSlice
    {
        public EventSlice_旋风(EventDisk_Base sliceDisk) : base(sliceDisk)
        {
        }

        private EventDisk_旋风 diskData => DiskData as EventDisk_旋风;

        [LabelText("位置")]
        [ShowInInspector]
        public Vector2 Position { get => diskData.position; set => diskData.position = value; }

        [LabelText("半径")]
        [ShowInInspector]
        public int radius
        {
            get { return diskData.radius; }
            set { diskData.radius = value; }
        }

        [LabelText("力")]
        [ShowInInspector]
        public float force
        {
            get { return diskData.force; }
            set { diskData.force = value; }
        }

        public override void DrawGizmo(SkillPlayAgent agent)
        {
            Gizmos.matrix = Matrix4x4.TRS(agent.transform.position, agent.transform.rotation, agent.transform.lossyScale) * Matrix4x4.TRS(diskData.position, Quaternion.identity, Vector3.one);
            Gizmos.DrawWireSphere(Vector3.zero, radius * Pixel.Size);
        }

        public override void DrawHandle(SkillPlayAgent agent)
        {
            Vector3 position = diskData.position;
            HandlePosition(agent, Quaternion.identity, ref position);
            diskData.position = position;
        }
    }
}
