
using UnityEngine;

namespace PRO.SkillEditor
{
    internal class NullSlice : SliceBase
    {
        public NullSlice(SliceBase_Disk sliceDisk) : base(sliceDisk)
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
