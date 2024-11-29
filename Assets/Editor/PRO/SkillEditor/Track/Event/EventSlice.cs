

namespace PRO.SkillEditor
{
    internal class EventSlice : SliceBase
    {
        public EventSlice(EventSlice_Disk sliceDisk) : base(sliceDisk)
        {
            Name = DiskData.GetType().Name;
        }

        public override void DrawGizmo() { }


    }
}
