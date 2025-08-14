using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.BehaviourTrees
{

    [Name("Limit")]
    [Category("Decorators")]
    [Description("Limits the access of its child a specific number of times.")]
    [ParadoxNotion.Design.Icon("Filter")]
    public class Limiter : BTDecorator
    {

        [Name("Max Times"), Tooltip("The max ammount of times to allow the child to execute until the tree is completely restarted.")]
        public BBParameter<int> maxCount = 1;
        [Name("Increase Count Policy"), Tooltip("Only increase count if the selected status is returned from the child.")]
        public BehaviourPolicy policy = BehaviourPolicy.OnSuccessOrFailure;
        [Tooltip("The Status that will be returned when the max number of times has been reached.")]
        public FinalStatus limitedStatus = FinalStatus.Optional;

        private int executedCount;

        public override void OnGraphStoped() {
            executedCount = 0;
        }

        protected override Status OnExecute(Component agent, IBlackboard blackboard) {

            if ( decoratedConnection == null ) {
                return Status.Optional;
            }

            if ( executedCount >= maxCount.value ) {
                return (Status)limitedStatus;
            }

            status = decoratedConnection.Execute(agent, blackboard);
            if
            (
                ( status == Status.Success && policy == BehaviourPolicy.OnSuccess ) ||
                ( status == Status.Failure && policy == BehaviourPolicy.OnFailure ) ||
                ( ( status == Status.Success || status == Status.Failure ) && policy == BehaviourPolicy.OnSuccessOrFailure )
            ) {
                executedCount++;
            }

            return status;
        }

        ///----------------------------------------------------------------------------------------------
        ///---------------------------------------UNITY EDITOR-------------------------------------------
#if UNITY_EDITOR

        protected override void OnNodeGUI() {
            GUILayout.Label(executedCount + " / " + maxCount.value);
        }

#endif
    }
}