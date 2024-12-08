
using Sirenix.OdinInspector;

namespace PRO.SkillEditor
{
    internal class EventSlice_RepeatPlay : EventSlice
    {
        public EventSlice_RepeatPlay(EventDisk_GotoPlay sliceDisk) : base(sliceDisk)
        {
            Name = $"跳转->{sliceDisk.goToFrame}";
        }

        private EventDisk_GotoPlay diskData => (EventDisk_GotoPlay)DiskData;

        
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
