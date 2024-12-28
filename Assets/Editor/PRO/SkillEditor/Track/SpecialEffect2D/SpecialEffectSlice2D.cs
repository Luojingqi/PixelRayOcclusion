using Sirenix.OdinInspector;
using UnityEngine;

namespace PRO.SkillEditor
{
    internal class SpecialEffectSlice2D : SliceBase
    {
        public SpecialEffectSlice2D(SpecialEffectSlice2D_Disk sliceDisk) : base(sliceDisk)
        {
            base.Background_Sprite = sliceDisk.sprite;
        }

        public override void DrawGizmo(SkillPlayAgent agent)
        {
            Vector2 wh = new Vector2(diskData.sprite.rect.width / diskData.sprite.pixelsPerUnit, diskData.sprite.rect.height / diskData.sprite.pixelsPerUnit);
            Gizmos.matrix = Matrix4x4.TRS(agent.transform.position, agent.transform.rotation, agent.transform.lossyScale) * Matrix4x4.TRS(diskData.position, diskData.rotation, diskData.scale);
            Gizmos.DrawWireCube(wh * (new Vector2(0.5f, 0.5f) - sprite.pivot / new Vector2(sprite.rect.width, sprite.rect.height)), wh);
        }

        public override void DrawHandle(SkillPlayAgent agent)
        {
            HandlePosition(agent, diskData.rotation, ref diskData.position);
            HandleRotation(agent, diskData.position, ref diskData.rotation);
            HandleScale(agent, diskData.position, diskData.rotation, ref diskData.scale);
        }
        public override void Select()
        {
            base.Select();
            if (SkillEditorWindow.Inst.Config.Agent == null) return;
            DiskData.UpdateFrame(SkillEditorWindow.Inst.Config.Agent, DiskData.startFrame, 0, Track.trackIndex);
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
