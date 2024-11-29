using PRO.SkillEditor;
using UnityEngine;

public class Event_DiskTest : EventSlice_Disk
{
    public override void UpdateFrame(SkillPlayAgent agent, int frame, int index)
    {
        Debug.Log(agent.name);
    }
}
