namespace PRO.SkillEditor
{
    public abstract class EventDisk_Base : Slice_DiskBase
    {
        public virtual void Update(SkillPlayAgent agent, SkillPlayData data, FrameData frameData, float deltaTime, float time)
        {
            for (int logicIndex = 0; logicIndex < data.SkillLogicList.Count; logicIndex++)
                data.SkillLogicList[logicIndex].Update_Event(agent, data, this, frameData, deltaTime, time);
        }
    }
}
