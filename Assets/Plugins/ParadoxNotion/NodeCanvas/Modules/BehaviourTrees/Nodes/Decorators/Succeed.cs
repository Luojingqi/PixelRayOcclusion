using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.BehaviourTrees
{

    [Name("Succeed", -1)]
    [Category("Decorators")]
    [Description("Force return Success status.")]
    [ParadoxNotion.Design.Icon("Success")]
    public class Succeed : BTDecorator
    {

        protected override Status OnExecute(Component agent, IBlackboard blackboard) {

            if ( decoratedConnection == null ) { return Status.Success; }
            if ( status == Status.Resting ) { decoratedConnection.Reset(); }

            status = decoratedConnection.Execute(agent, blackboard);
            return status == Status.Running ? Status.Running : Status.Success;
        }
    }
}