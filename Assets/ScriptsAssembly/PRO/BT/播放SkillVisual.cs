using NodeCanvas.BehaviourTrees;
using NodeCanvas.Framework;
using PRO.SkillEditor;
using UnityEngine;
namespace PRO.BT.Tool
{
    public class 播放SkillVisual : BTDecorator
    {
        public BBParameter<SkillPlayData> playData;
        public BBParameter<SkillPlayAgent> playAgent;

        protected override Status OnExecute(Component agent, IBlackboard blackboard)
        {
            var dataValue = playData.value;
            var agentValue = playAgent.value;
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
