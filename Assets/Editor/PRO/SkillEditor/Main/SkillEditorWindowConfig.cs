using Sirenix.OdinInspector;

namespace PRO.SkillEditor
{
    internal class SkillEditorWindowConfig : SerializedScriptableObject
    {
        public SkillPlayAgent Agent
        {
            get { return agent; }
            set
            {
                agent = value;
                if (agent != null)
                    agent.Skill = skill_Disk;
            }
        }
        private SkillPlayAgent agent;
        public Skill_Disk Skill_Disk
        {
            get { return skill_Disk; }
            set
            {
                skill_Disk = value;
                if (agent != null)
                    agent.Skill = skill_Disk;
            }
        }
        private Skill_Disk skill_Disk;
    }
}
