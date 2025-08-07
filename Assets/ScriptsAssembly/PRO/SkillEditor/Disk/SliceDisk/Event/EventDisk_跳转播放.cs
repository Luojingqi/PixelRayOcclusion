using System.Collections.Generic;

namespace PRO.SkillEditor
{
    public class EventDisk_跳转播放 : EventDisk_Base
    {
        public int goToFrame;

        public AllowLogicChangeValue_EventDisk_跳转播放 changeValue = new();

        public override void UpdateFrame(SkillPlayAgent agent, SkillVisual_Disk visual, IEnumerable<SkillLogicBase> logics, FrameData frameData)
        {
            foreach (var logic in logics)
                logic.Before_Event(this, frameData);
            if (changeValue.goToFrame >= 0 && changeValue.goToFrame < visual.MaxFrame)
            {
                var data = agent.GetSkill(visual.loadPath);
                data.nowFrame = changeValue.goToFrame;
                data.time = 0;
                visual.UpdateFrame(agent, logics, changeValue.goToFrame);
            }
            if (frameData.sliceFrame == frameLength - 1)
                foreach (var logic in logics)
                    logic.After_Event(this, frameData);

            changeValue.Reset(this);
        }

        public class AllowLogicChangeValue_EventDisk_跳转播放 : AllowLogicChangeValueBase
        {
            public int goToFrame;
            public void Reset(EventDisk_跳转播放 slice)
            {
                goToFrame = slice.goToFrame;
            }
        }
    }
}
