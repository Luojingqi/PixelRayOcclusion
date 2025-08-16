using Google.FlatBuffers;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using PRO.BT.Flat.Base;
using UnityEngine;


namespace NodeCanvas.BehaviourTrees
{

    [Name("Action")]
    [Description("Executes an action and returns Success or Failure when the action is finished.\nReturns Running until the action is finished.")]
    [ParadoxNotion.Design.Icon("Action")]
    // [Color("ff6d53")]
    public class ActionNode : BTNode, ITaskAssignable<ActionTask>
    {

        [SerializeField]
        private ActionTask _action;

        public Task task
        {
            get { return action; }
            set { action = (ActionTask)value; }
        }

        public ActionTask action
        {
            get { return _action; }
            set { _action = value; }
        }

        public override string name => base.name.ToUpper();

        protected override Status OnExecute(Component agent, IBlackboard blackboard)
        {

            if (action == null)
            {
                return Status.Optional;
            }

            if (status == Status.Resting || status == Status.Running)
            {
                return action.Execute(agent, blackboard);
            }

            return status;
        }

        protected override void OnReset()
        {
            action?.EndAction(null);
        }

        public override void OnGraphPaused()
        {
            action?.Pause();
        }

        protected override void ExtendToDisk(FlatBufferBuilder builder)
        {
            if (action == null) return;
            builder.Finish(action.ToDisk(builder).Value);
        }
        protected override void ExtendToRAM(FlatBufferBuilder builder)
        {
            if (action == null) return;
            action.ToRAM(ActionTaskData.GetRootAsActionTaskData(builder.DataBuffer));
        }
    }
}