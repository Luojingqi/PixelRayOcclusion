namespace PRO.SkillEditor
{
    public class EventDisk_Ìø×ª²¥·Å : EventDisk_Base
    {
        public int goToFrame;
        public override void UpdateFrame(SkillPlayAgent agent, int frame, int frameIndex, int trackIndex)
        {
            agent.NowFrame = goToFrame;
            agent.Skill?.UpdateFrame(agent, agent.NowFrame);
        }
    }
}
