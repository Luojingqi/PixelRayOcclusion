using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using ParadoxNotion;

namespace NodeCanvas.BehaviourTrees
{

    [Category("Decorators")]
    [ParadoxNotion.Design.Icon("Eye")]
    [Description("Monitors the decorated child for a returned status and executes an action when that is the case. The final status returned to the parent can either be the original decorated child status, or the new decorator action status.")]
    public class Monitor : BTDecorator, ITaskAssignable<ActionTask>
    {

        public enum ReturnStatusMode
        {
            OriginalDecoratedChildStatus,
            NewDecoratorActionStatus,
        }

        [Name("Monitor"), Tooltip("The Status to monitor for.")]
        public BehaviourPolicy monitorMode;
        [Name("Return"), Tooltip("The Status to return after (and if) the Action is executed.")]
        public ReturnStatusMode returnMode;

        private Status decoratorActionStatus;

        [SerializeField]
        private ActionTask _action;

        public ActionTask action {
            get { return _action; }
            set { _action = value; }
        }

        public Task task {
            get { return action; }
            set { action = (ActionTask)value; }
        }

        protected override Status OnExecute(Component agent, IBlackboard blackboard) {

            if ( decoratedConnection == null ) {
                return Status.Optional;
            }

            if (action.isRunning){
                decoratorActionStatus = action.Execute(agent, blackboard);
            }

            var newChildStatus = decoratedConnection.status;
            if (!action.isRunning){
                newChildStatus = decoratedConnection.Execute(agent, blackboard);
                if ( status != newChildStatus ) {
                    var executeAction = false;
                    executeAction |= newChildStatus == Status.Success && monitorMode == BehaviourPolicy.OnSuccess;
                    executeAction |= newChildStatus == Status.Failure && monitorMode == BehaviourPolicy.OnFailure;
                    executeAction |= monitorMode == BehaviourPolicy.OnSuccessOrFailure && newChildStatus != Status.Running;

                    if ( executeAction ) {
                        decoratorActionStatus = action.Execute(agent, blackboard);
                    }
                }
            }

            if (decoratorActionStatus == Status.Running){
                return Status.Running;
            }

            return returnMode == ReturnStatusMode.NewDecoratorActionStatus && decoratorActionStatus != Status.Resting ? decoratorActionStatus : newChildStatus;
        }

        protected override void OnReset() {
            if ( action != null ) {
                action.EndAction(null);
                decoratorActionStatus = Status.Resting;
            }
        }


        ///----------------------------------------------------------------------------------------------
        ///---------------------------------------UNITY EDITOR-------------------------------------------
#if UNITY_EDITOR

        protected override void OnNodeGUI() {
            GUILayout.Label(string.Format("<b>[{0}]</b>", monitorMode.ToString().SplitCamelCase()));
        }

#endif
        ///---------------------------------------UNITY EDITOR-------------------------------------------
    }
}