using Sirenix.OdinInspector;
using UnityEngine;
namespace PRO.SkillEditor
{
    internal class EventSlice_Transform : EventSlice
    {
        public override void DrawGizmo(SkillPlayAgent agent)
        {
            Gizmos.matrix = Matrix4x4.TRS(agent.transform.position, agent.transform.rotation, agent.transform.lossyScale) * Matrix4x4.TRS(diskData.position, Quaternion.Euler(diskData.rotation), diskData.scale);
            Gizmos.DrawWireCube(Vector2.zero, new(0.5f, 0.5f));
        }

        public override void DrawHandle(SkillPlayAgent agent)
        {
            Vector3 position = diskData.position;
            if (HandlePosition(
                Matrix4x4.TRS(agent.transform.position, agent.transform.rotation, Vector3.one),
                ref position))
                diskData.position = position;

            Quaternion rotation = Quaternion.Euler(diskData.rotation);
            if (HandleRotation(
                Matrix4x4.TRS(agent.transform.position, Quaternion.Euler(0, 0, agent.transform.rotation.eulerAngles.z), Vector3.one) *
                Matrix4x4.TRS(diskData.position, Quaternion.identity, Vector3.one),
                ref rotation))
                diskData.rotation = rotation.eulerAngles;

            Vector3 scale = diskData.scale;
            if (HandleScale(
                Matrix4x4.TRS(agent.transform.position, Quaternion.Euler(0, 0, agent.transform.rotation.eulerAngles.z), Vector3.one) *
                Matrix4x4.TRS(diskData.position, rotation, Vector3.one),
                ref scale))
                diskData.scale = scale;
            SkillVisualEditorWindow.Inst.PlaySlice(this);
        }

        private EventDisk_Transform diskData => DiskData as EventDisk_Transform;

        [LabelText("Î»ÖÃ")]
        [ShowInInspector]
        public Vector3 Position { get => diskData.position; set => diskData.position = value; }
        [ShowInInspector]
        public bool IsPosition { get => diskData.isPosition; set => diskData.isPosition = value; }
        [LabelText("Ðý×ª")]
        [ShowInInspector]
        public Vector3 Rotation { get => diskData.rotation; set { diskData.rotation = value; } }
        [ShowInInspector]
        public bool IsRotation { get => diskData.isRotation; set => diskData.isRotation = value; }
        [LabelText("Ëõ·Å")]
        [ShowInInspector]
        public Vector3 Scale { get => diskData.scale; set => diskData.scale = value; }
        [ShowInInspector]
        public bool IsScale { get => diskData.isScale; set => diskData.isScale = value; }

    }
}