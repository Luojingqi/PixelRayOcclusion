using Sirenix.OdinInspector;
using UnityEngine;

namespace PRO.SkillEditor
{
    internal class EventSlice_吹风 : EventSlice
    {
        public EventSlice_吹风(EventDisk_Base sliceDisk) : base(sliceDisk)
        {
        }

        private EventDisk_吹风 diskData => DiskData as EventDisk_吹风;

        [LabelText("位置")]
        [ShowInInspector]
        public Vector2 Position { get => diskData.position; set => diskData.position = value; }
        [LabelText("旋转")]
        [ShowInInspector]
        public Quaternion Rotation { get => diskData.rotation; set => diskData.rotation = value; }
        [LabelText("缩放")]
        [ShowInInspector]
        public Vector2 Scale { get => diskData.scale; set => diskData.scale = value; }


        [LabelText("力")]
        [ShowInInspector]
        public float force
        {
            get { return diskData.force; }
            set { diskData.force = value; }
        }

        public override void DrawGizmo(SkillPlayAgent agent)
        {
            Gizmos.matrix = Matrix4x4.TRS(agent.transform.position, agent.transform.rotation, agent.transform.lossyScale) * Matrix4x4.TRS(diskData.position, diskData.rotation, diskData.scale);
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
            Gizmos.DrawLine(new Vector3(-1 / 4f, 1 / 64f), new Vector3(1 / 4f, 1 / 64f));
            Gizmos.DrawLine(new Vector3(-1 / 4f, -1 / 64f), new Vector3(1 / 4f, -1 / 64f));
            Gizmos.DrawLine(new Vector3(1 / 8f, 1 / 8f), new Vector3(1 / 4f, 0));
            Gizmos.DrawLine(new Vector3(1 / 8f, -1 / 8f), new Vector3(1 / 4f, 0));
        }

        public override void DrawHandle(SkillPlayAgent agent)
        {
            Vector3 position = diskData.position;
            HandlePosition(agent, diskData.rotation, ref position);
            diskData.position = position;
            Quaternion rotation = diskData.rotation;
            HandleRotation(agent, diskData.position, ref rotation);
            diskData.rotation = rotation;
            Vector3 scale = diskData.scale;
            HandleScale(agent, diskData.position, diskData.rotation, ref scale);
            diskData.scale = scale;            
        }
    }
}
