using NodeCanvas.BehaviourTrees;
using NodeCanvas.Framework;
using PRO.SkillEditor;
using UnityEngine;
namespace PRO.BT.Tool
{
    public class 播放SkillVisual : BTDecorator
    {
        public BBParameter<SkillPlayData> PlayData;
        public BBParameter<SkillPlayAgent> PlayAgent;

        protected override Status OnExecute(Component agent, IBlackboard blackboard)
        {
            var dataValue = PlayData.value;
            var agentValue = PlayAgent.value;
            if (dataValue.UpdateFrameScript(agentValue, TimeManager.deltaTime))
            {
                dataValue.ResetFrameIndex(agentValue);
                return Status.Success;
            }
            else
                decoratedConnection?.Execute(agent, blackboard);
            return Status.Running;
        }
    }
}
