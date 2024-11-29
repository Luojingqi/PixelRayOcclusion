using UnityEngine;

namespace PRO.SkillEditor
{
    /// <summary>
    /// ��2D�����������һ֡һ֡�Ķ�����Ƭ��ɣ�����Ϊһ����Ƭ
    /// </summary>
    internal class AnimationSlice2D : SliceBase
    {
        public AnimationSlice2D(AnimationSlice2D_Disk sliceDisk) : base(sliceDisk)
        {
            if (sliceDisk.sprite != null)
                Background = new UnityEngine.UIElements.StyleBackground(diskData.sprite);
        }

        private AnimationSlice2D_Disk diskData => DiskData as AnimationSlice2D_Disk;

        public override void DrawGizmo()
        {

        }

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
