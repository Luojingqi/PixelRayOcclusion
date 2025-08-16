using Sirenix.OdinInspector;
using UnityEngine;

namespace PRO.SkillEditor
{
    internal class SpecialEffectSlice2D : SliceBase
    {
        public override void Init(Slice_DiskBase sliceDisk)
        {
            base.Init(sliceDisk);
            Background_Sprite = diskData.sprite;
        }
        public override void DrawGizmo(SkillPlayAgent agent)
        {
            Vector2 wh = new(0.5f, 0.5f);
            if (diskData.sprite != null)
                wh = new Vector2(diskData.sprite.rect.width / diskData.sprite.pixelsPerUnit, diskData.sprite.rect.height / diskData.sprite.pixelsPerUnit);
            Gizmos.matrix = Matrix4x4.TRS(agent.transform.position, agent.transform.rotation, agent.transform.lossyScale) * Matrix4x4.TRS(diskData.position, diskData.rotation, diskData.scale);
            if (diskData.sprite != null)
                Gizmos.DrawWireCube(wh * (new Vector2(0.5f, 0.5f) - sprite.pivot / new Vector2(sprite.rect.width, sprite.rect.height)), wh);
            else
                Gizmos.DrawWireCube(Vector2.zero, wh);
        }

        public override void DrawHandle(SkillPlayAgent agent)
        {
            Vector3 position = diskData.position;
            if (HandlePosition(
                Matrix4x4.TRS(agent.transform.position, agent.transform.rotation, Vector3.one),
                ref position))
                diskData.position = position;

            Quaternion rotation = diskData.rotation;
            if (HandleRotation(
                Matrix4x4.TRS(agent.transform.position, Quaternion.Euler(0, 0, agent.transform.rotation.eulerAngles.z), Vector3.one) *
                Matrix4x4.TRS(diskData.position, Quaternion.identity, Vector3.one),
                ref rotation))
                diskData.rotation = rotation;

            Vector3 scale = diskData.scale;
            if (HandleScale(
                Matrix4x4.TRS(agent.transform.position, Quaternion.Euler(0, 0, agent.transform.rotation.eulerAngles.z), Vector3.one) *
                Matrix4x4.TRS(diskData.position, rotation, Vector3.one),
                ref scale))
                diskData.scale = scale;
            SkillVisualEditorWindow.Inst.PlaySlice(this);
        }
        public override void Select()
        {
            base.Select();
            if (SkillVisualEditorWindow.Inst.Config.Agent == null) return;
            SkillVisualEditorWindow.Inst.PlaySlice(this);
        }

        private SpecialEffectSlice2D_Disk diskData => DiskData as SpecialEffectSlice2D_Disk;

        [LabelText("精灵特效")]
        [ShowInInspector]
        public Sprite sprite
        {
            get { return diskData.sprite; }
            set { diskData.sprite = value; Background_Sprite = value; }
        }

        [LabelText("位置")]
        [ShowInInspector]
        public Vector3 Position { get => diskData.position; set => diskData.position = value; }
        [LabelText("旋转")]
        [ShowInInspector]
        public Quaternion Rotation { get => diskData.rotation; set => diskData.rotation = value; }
        [LabelText("缩放")]
        [ShowInInspector]
        public Vector3 Scale { get => diskData.scale; set => diskData.scale = value; }

    }
}
