using UnityEngine;

namespace PRO.SkillEditor
{
    public class AnimationSlice2D_Disk : Slice_DiskBase
    {
        public Sprite sprite;


        public override void UpdateFrame(SkillPlayAgent agent, SkillPlayData playData, FrameData frameData)
        {
            agent.AgentSprice.sprite = sprite;
        }

        public override void EndFrame(SkillPlayAgent agent, SkillPlayData playData, FrameData frameData)
        {
            
        }


        public class AllowLogicChangeValue_AnimationSlice2D_Disk : AllowLogicChangeValueBase
        {
            public Sprite sprite;

            public void Reset(AnimationSlice2D_Disk slice)
            {
                sprite = slice.sprite;
            }
        }
    }
}
