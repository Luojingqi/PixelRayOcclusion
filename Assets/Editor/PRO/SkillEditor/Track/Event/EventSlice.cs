

namespace PRO.SkillEditor
{
    internal class EventSlice : SliceBase
    {
        public EventSlice(EventDisk_Base sliceDisk) : base(sliceDisk)
        {
            if (sliceDisk.startFrame == -1)
            {
                Name = sliceDisk.GetType().Name.Split('_', 2)[1];
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
