using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.BehaviourTrees
{

    [System.Obsolete("Use Conditional Evaluator")]
    [Name("Interrupt")]
    [Category("Decorators")]
    [Description("Executes and returns the child status if the condition is false. Returns the specified status if the condition is or becomes true.")]
    [ParadoxNotion.Design.Icon("Interruptor")]
    public class Interruptor : BTDecorator, ITaskAssignable<ConditionTask>
    {

        [SerializeField]
        private ConditionTask _condition;

        [Tooltip("The status that will be returned if the assigned condition is or becomes true.")]
        public FinalStatus conditionSuccessReturn = FinalStatus.Failure;

        public ConditionTask condition {
            get { return _condition; }
            set { _condition = value; }
        }

        public Task task {
            get { return condition; }
            set { condition = (ConditionTask)value; }
        }

        protected override Status OnExecute(Component agent, IBlackboard blackboard) {

            if ( decoratedConnection == null ) {
                return Status.Optional;
            }

            if ( condition == null ) {
                return decoratedConnection.Execute(agent, blackboard);
            }

            if ( status == Status.Resting ) {
                condition.Enable(agent, blackboard);
            }

            if ( condition.Check(agent, blackboard) == false ) {
                return decoratedConnection.Execute(agent, blackboard);
            }

            if ( decoratedConnection.status == Status.Running ) {
                decoratedConnection.Reset();
            }

            return (Status)conditionSuccessReturn;
        }

        protected override void OnReset() {
            condition?.Disable();
        }
    }
}