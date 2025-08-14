using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.BehaviourTrees
{

    [Name("Conditional")]
    [Category("Decorators")]
    [Description("Executes and returns the child status if the condition is true. Returns the specified status if the condition is or becomes false. If Dynamic is enabled and the condition becomes false while the child node is Running, the child node will be interrupted.")]
    [ParadoxNotion.Design.Icon("Accessor")]
    public class ConditionalEvaluator : BTDecorator, ITaskAssignable<ConditionTask>
    {

        [Name("Dynamic"), Tooltip("If enabled, the condition is re-evaluated per tick and the child is interrupted if the condition becomes false.")]
        public bool isDynamic;
        [Tooltip("The status that will be returned if the assigned condition is or becomes false.")]
        public FinalStatus conditionFailReturn = FinalStatus.Failure;

        [SerializeField]
        private ConditionTask _condition;
        private bool accessed;

        public Task task {
            get { return condition; }
            set { condition = (ConditionTask)value; }
        }

        private ConditionTask condition {
            get { return _condition; }
            set { _condition = value; }
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

            if ( isDynamic ) {

                if ( condition.Check(agent, blackboard) ) {
                    return decoratedConnection.Execute(agent, blackboard);
                }
                decoratedConnection.Reset();
                return (Status)conditionFailReturn;

            } else {

                if ( status != Status.Running ) {
                    accessed = condition.Check(agent, blackboard);
                }

                return accessed ? decoratedConnection.Execute(agent, blackboard) : (Status)conditionFailReturn;
            }
        }

        protected override void OnReset() {
            condition?.Disable();
            accessed = false;
        }

        ///----------------------------------------------------------------------------------------------
        ///---------------------------------------UNITY EDITOR-------------------------------------------
#if UNITY_EDITOR

        protected override void OnNodeGUI() {
            if ( isDynamic ) { GUILayout.Label("<b>DYNAMIC</b>"); }
        }

        protected override void OnNodeInspectorGUI() {
            base.OnNodeInspectorGUI();
            EditorUtils.Separator();
        }

#endif
    }
}