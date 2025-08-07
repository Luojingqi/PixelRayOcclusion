using Sirenix.OdinInspector;

namespace PRO.SkillEditor
{
    internal class EventSlice_��ת���� : EventSlice
    {
        public override void Init(Slice_DiskBase sliceDisk)
        {
            base.Init(sliceDisk);
            if (sliceDisk.startFrame == -1)
            {
                Name = $"��ת->{diskData.goToFrame}";
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

        public override void DrawGizmo(SkillPlayAgent agent)
        {
            
        }

        public override void DrawHandle(SkillPlayAgent agent)
        {
            
        }
    }
}
