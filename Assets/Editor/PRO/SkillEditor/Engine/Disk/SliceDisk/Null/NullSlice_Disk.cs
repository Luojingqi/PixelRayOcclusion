using UnityEngine;

namespace PRO.SkillEditor
{
    public class NullSlice_Disk : SliceBase_Disk
    {
        public override void UpdateFrame(SkillPlayAgent agent, int frame, int index)
        {
            Debug.Log(frame);
        }
    }
}
