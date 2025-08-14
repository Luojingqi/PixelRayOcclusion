using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.BehaviourTrees
{

    [Name("Fail", -2)]
    [Category("Decorators")]
    [Description("Force return Failure status.")]
    [ParadoxNotion.Design.Icon("Failure")]
    public class Fail : BTDecorator
    {

        protected override Status OnExecute(Component agent, IBlackboard blackboard) {

            if ( decoratedConnection == null ) { return Status.Failure; }
            if ( status == Status.Resting ) { decoratedConnection.Reset(); }

            status = decoratedConnection.Execute(agent, blackboard);
            return status == Status.Running ? Status.Running : Status.Failure;
        }
    }
}