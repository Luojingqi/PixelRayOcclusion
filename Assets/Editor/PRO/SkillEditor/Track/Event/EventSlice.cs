﻿

namespace PRO.SkillEditor
{
    internal class EventSlice : SliceBase
    {
        public EventSlice(EventSlice_Disk sliceDisk) : base(sliceDisk)
        {
            Name = sliceDisk.name;
        }

        public override void DrawGizmo(SkillPlayAgent agent)
        {
            throw new System.NotImplementedException();
        }

        public override void DrawHandle(SkillPlayAgent agent)
        {
            throw new System.NotImplementedException();
        }
    }
}
