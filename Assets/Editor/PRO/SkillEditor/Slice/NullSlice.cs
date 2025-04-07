
using UnityEngine;

namespace PRO.SkillEditor
{
    internal class NullSlice : SliceBase
    {
        public NullSlice(Slice_DiskBase sliceDisk) : base(sliceDisk)
        {
            if (sliceDisk.startFrame == -1)
            {
                Name = "空";
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
