namespace PRO.SkillEditor
{
    public class EventDisk_��ת���� : EventDisk_Base
    {
        public int goToFrame;

        public AllowLogicChangeValue_EventDisk_��ת���� changeValue = new();

        public override void UpdateFrame(SkillPlayAgent agent, SkillPlayData playData, FrameData frameData)
        {
            for (int logicIndex = 0; logicIndex < playData.SkillLogicList.Count; logicIndex++)
                playData.SkillLogicList[logicIndex].Before_Event(agent, playData, this, frameData);
            if (changeValue.goToFrame >= 0 && changeValue.goToFrame < playData.SkillVisual.MaxFrame)
            {
                playData.nowFrame = changeValue.goToFrame;
                playData.time = 0;
                playData.SkillVisual.UpdateFrame(agent, playData);
            }
            if (frameData.sliceFrame == frameLength - 1)
                for (int logicIndex = 0; logicIndex < playData.SkillLogicList.Count; logicIndex++)
                    playData.SkillLogicList[logicIndex].After_Event(agent, playData, this, frameData);

            changeValue.Reset(this);
        }

        public class AllowLogicChangeValue_EventDisk_��ת���� : AllowLogicChangeValueBase
        {
            public int goToFrame;
            public void Reset(EventDisk_��ת���� slice)
            {
                goToFrame = slice.goToFrame;
            }
        }
    }
}
