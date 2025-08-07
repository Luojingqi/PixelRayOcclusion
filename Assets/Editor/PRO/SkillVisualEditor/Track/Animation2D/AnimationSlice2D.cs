using Sirenix.OdinInspector;
using UnityEngine;

namespace PRO.SkillEditor
{
    /// <summary>
    /// 在2D动画轨道中由一帧一帧的动画切片组成，此类为一个切片
    /// </summary>
    internal class AnimationSlice2D : SliceBase
    {
        public override void Init(Slice_DiskBase sliceDisk)
        {
            base.Init(sliceDisk);
            if (diskData.sprite != null)
                Background = new UnityEngine.UIElements.StyleBackground(diskData.sprite);
        }

        private AnimationSlice2D_Disk diskData => DiskData as AnimationSlice2D_Disk;

        public override void DrawGizmo(SkillPlayAgent agent)
        {

        }

        public override void DrawHandle(SkillPlayAgent agent)
        {

        }
        [LabelText("精灵图")]
        [ShowInInspector]
        public Sprite Sprite
        {
            get { return diskData.sprite; }
            set
            {
                diskData.sprite = value;
                Background = new UnityEngine.UIElements.StyleBackground(diskData.sprite);
            }
        }


    }
}
