using NodeCanvas.Framework;
using ParadoxNotion.Design;
using ParadoxNotion.Serialization.FullSerializer;
using UnityEngine;


namespace NodeCanvas.BehaviourTrees
{

    [Name("Guard")]
    [Category("Decorators")]
    [ParadoxNotion.Design.Icon("Shield")]
    [Description("Protects the decorated child from running if another Guard with the same token is already guarding (Running) that token. The token is a blackboard variable, therefore the scope of the guard depends on the scope of the variable (eg graph scope if used with a graph blackboard variable, gameobject scope for all gameobject's behaviour trees if used with a gameobject blackboard variable, or even global scope if used with a global blackboard variable).")]
    public class Guard : BTDecorator
    {
        [System.Serializable, fsAutoInstance]
        public class GuardToken
        {
            public Guard currentGuard { get; set; }
        }

        public enum GuardMode
        {
            Failure = 0,
            Success = 1,
            Optional = 5,
            RunningUntilReleased = 2,
        }

        [BlackboardOnly, Tooltip("The variable to use as a guard token. ( You can simply 'Create New' from the dropdown )")]
        public BBParameter<GuardToken> token;
        [Tooltip("The status to return in case the token is already guarded by another Guard.")]
        public GuardMode guardedStatus = GuardMode.Failure;

        protected override Status OnExecute(Component agent, IBlackboard blackboard) {

            if ( decoratedConnection == null ) {
                return Status.Optional;
            }

            if ( token.value.currentGuard == null ) {
                token.value.currentGuard = this;
            }

            if ( token.value.currentGuard == this ) {
                return decoratedConnection.Execute(agent, blackboard);
            }

            return (Status)guardedStatus;
        }

        protected override void OnReset() {
            if ( token.value.currentGuard == this ) {
                token.value.currentGuard = null;
            }
        }

        ///----------------------------------------------------------------------------------------------
        ///---------------------------------------UNITY EDITOR-------------------------------------------
#if UNITY_EDITOR

        protected override void OnNodeGUI() {
            GUILayout.Label(string.Format("<b>{0}</b>", token));
        }

#endif
    }
}