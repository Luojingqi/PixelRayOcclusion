
namespace PRO.SkillEditor
{
    internal class NullSlice : SliceBase
    {
        public NullSlice(SliceBase_Disk sliceDisk) : base(sliceDisk)
        {
            Name = "空";
        }

        public override void DrawGizmo()
        {

        }

    }
}
