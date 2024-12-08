using UnityEngine;

namespace PRO.SkillEditor
{
    public class AnimationSlice2D_Disk : SliceBase_Disk
    {
        public Sprite sprite;

        public override void UpdateFrame(SkillPlayAgent agent, int frame, int frameIndex, int trackIndex)
        {
            agent.AgentSprice.sprite = sprite;
        }
    }
}
