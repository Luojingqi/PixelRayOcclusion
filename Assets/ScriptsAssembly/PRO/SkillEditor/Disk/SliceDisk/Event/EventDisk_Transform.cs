using PRO.SkillEditor;
using UnityEngine;

public class EventDisk_Transform : EventDisk_Base
{
    public Vector3 position;
    public Quaternion rotation = Quaternion.identity;
    public Vector3 scale;

    public override void UpdateFrame(SkillPlayAgent agent, SkillPlayData playData, FrameData frameData)
    {
        for (int logicIndex = 0; logicIndex < playData.SkillLogicList.Count; logicIndex++)
            playData.SkillLogicList[logicIndex].Before_Event(agent, playData, this, frameData);


        if (frameData.sliceFrame == frameLength - 1)
            for (int logicIndex = 0; logicIndex < playData.SkillLogicList.Count; logicIndex++)
                playData.SkillLogicList[logicIndex].After_Event(agent, playData, this, frameData);
    }

    public override void Update(SkillPlayAgent agent, SkillPlayData data, FrameData frameData, float deltaTime, float time)
    {
        base.Update(agent, data, frameData, deltaTime, time);
        agent.transform.localPosition += Vector3.Lerp(Vector3.zero, position, deltaTime);
        agent.transform.localRotation *= Quaternion.Lerp(Quaternion.identity, rotation, deltaTime);
        agent.transform.localScale += Vector3.Lerp(Vector3.zero, scale, deltaTime);
        Debug.Log(Quaternion.Lerp(Quaternion.identity, rotation, deltaTime).eulerAngles + "|" + deltaTime);

    }
}
