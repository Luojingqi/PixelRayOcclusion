using Sirenix.OdinInspector;

namespace PRO.SkillEditor
{
    internal class EventSlice_��ת���� : EventSlice
    {
        public EventSlice_��ת����(EventDisk_��ת���� sliceDisk) : base(sliceDisk)
        {
            if (sliceDisk.startFrame == -1)
            {
                Name = $"��ת->{sliceDisk.goToFrame}";
            }
        }

        private EventDisk_��ת���� diskData => (EventDisk_��ת����)DiskData;


        [LabelText("��ת��֡")]
        [ShowInInspector]
        public int GoFrame
        {
            get { return diskData.goToFrame; }
            set
            {
                diskData.goToFrame = value;
                Name = $"��ת->{value}";
            }
        }
    }
}
