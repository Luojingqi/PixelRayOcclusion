namespace PRO.SkillEditor
{
    public class EventDisk_��ת���� : EventSlice_Disk
    {
        public int goToFrame;
        public override void UpdateFrame(SkillPlayAgent agent, int frame, int frameIndex, int trackIndex)
        {
            agent.NowFrame = goToFrame;
            agent.Skill?.UpdateFrame(agent, agent.NowFrame);
        }
    }
}
