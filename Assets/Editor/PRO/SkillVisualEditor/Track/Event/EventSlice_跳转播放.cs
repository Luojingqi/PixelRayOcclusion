using Sirenix.OdinInspector;

namespace PRO.SkillEditor
{
    internal class EventSlice_跳转播放 : EventSlice
    {
        public override void Init(Slice_DiskBase sliceDisk)
        {
            base.Init(sliceDisk);
            if (sliceDisk.startFrame == -1)
            {
                Name = $"跳转->{diskData.goToFrame}";
            }
        }

        private EventDisk_跳转播放 diskData => (EventDisk_跳转播放)DiskData;


        [LabelText("跳转到帧")]
        [ShowInInspector]
        public int GoFrame
        {
            get { return diskData.goToFrame; }
            set
            {
                diskData.goToFrame = value;
                Name = $"跳转->{value}";
            }
        }

        public override void DrawGizmo(SkillPlayAgent agent)
        {
            
        }

        public override void DrawHandle(SkillPlayAgent agent)
        {
            
        }
    }
}
