using System.Collections;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.BehaviourTrees
{

    [Category("Decorators")]
    [Description("Limits the access of its child every specific amount of time.")]
    [ParadoxNotion.Design.Icon("Filter")]
    public class Cooldown : BTDecorator
    {

        [Tooltip("The cooldown time.")]
        public BBParameter<float> coolDownTime = 5f;
        [Tooltip("The status that will be returned when cooling down.")]
        public FinalStatus coolingStatus = FinalStatus.Optional;

        private float currentTime;

        public override void OnGraphStoped() {
            currentTime = 0;
        }

        protected override Status OnExecute(Component agent, IBlackboard blackboard) {

            if ( decoratedConnection == null ) {
                return Status.Optional;
            }

            if ( currentTime > 0 ) {
                return (Status)coolingStatus;
            }

            status = decoratedConnection.Execute(agent, blackboard);
            if ( status == Status.Success || status == Status.Failure ) {
                StartCoroutine(Internal_Cooldown());
            }

            return status;
        }


        IEnumerator Internal_Cooldown() {
            currentTime = coolDownTime.value;
            while ( currentTime > 0 ) {
                yield return null;
                currentTime -= Time.deltaTime;
            }
        }


        ///----------------------------------------------------------------------------------------------
        ///---------------------------------------UNITY EDITOR-------------------------------------------
#if UNITY_EDITOR

        protected override void OnNodeGUI() {
            GUILayout.Space(25);
            var pRect = new Rect(5, GUILayoutUtility.GetLastRect().y, rect.width - 10, 20);
            UnityEditor.EditorGUI.ProgressBar(pRect, currentTime / coolDownTime.value, currentTime > 0 ? "Cooling..." : "Cooled");
        }

#endif
    }
}