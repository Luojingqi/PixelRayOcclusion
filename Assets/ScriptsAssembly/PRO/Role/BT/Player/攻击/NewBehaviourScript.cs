using Google.FlatBuffers;
using NodeCanvas.BehaviourTrees;
using PRO;
using Sirenix.OdinInspector;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public BehaviourTreeOwner owner;

    public FlatBufferBuilder builder = new FlatBufferBuilder(1024 * 10);
    [Button]
    public void Save()
    {
        builder.Finish(owner.behaviour.ToDisk(builder).Value);
    }
    [Button]
    public void Load()
    {
        var diskData = PRO.BT.Flat.Base.BehaviourTreeData.GetRootAsBehaviourTreeData(builder.DataBuffer);
        owner.StartBehaviour();
        owner.behaviour.ToRAM(diskData);
    }

    public void Update()
    {
        owner.Tick(TimeManager.deltaTime);
    }
}
