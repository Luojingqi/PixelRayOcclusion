using Sirenix.OdinInspector;

namespace PRO.SkillEditor
{
    internal class EventSlice_跳转播放 : EventSlice
    {
        public EventSlice_跳转播放(EventDisk_跳转播放 sliceDisk) : base(sliceDisk)
        {
            if (sliceDisk.startFrame == -1)
            {
                Name = $"跳转->{sliceDisk.goToFrame}";
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
    }
}
