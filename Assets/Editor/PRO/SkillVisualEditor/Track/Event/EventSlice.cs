

namespace PRO.SkillEditor
{
    internal abstract class EventSlice : SliceBase
    {
        public override void Init(Slice_DiskBase sliceDisk)
        {
            base.Init(sliceDisk);
            if (sliceDisk.startFrame == -1)
            {
                Name = sliceDisk.GetType().Name.Split('_', 2)[1];
            }
        }
    }
}
