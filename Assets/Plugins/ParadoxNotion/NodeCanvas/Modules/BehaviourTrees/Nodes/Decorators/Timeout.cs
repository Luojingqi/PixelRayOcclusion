using Google.FlatBuffers;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using PRO.BT.Flat.Decorator;
using UnityEngine;


namespace NodeCanvas.BehaviourTrees
{

    [Category("Decorators")]
    [Description("Interupts the decorated child node and returns the specified status if the child node is still Running after the timeout period. Otherwise the child node status is returned.")]
    [ParadoxNotion.Design.Icon("Timeout")]
    public class Timeout : BTDecorator
    {

        [Tooltip("The timeout period in seconds.")]
        public BBParameter<float> timeout = 1;

        [Tooltip("The Status that will be returned if timeout occurs.")]
        public FinalStatus timeoutStatus = FinalStatus.Failure;

        protected override Status OnExecute(Component agent, IBlackboard blackboard) {

            if ( decoratedConnection == null ) {
                return Status.Optional;
            }

            status = decoratedConnection.Execute(agent, blackboard);
            if ( status == Status.Running ) {
                if ( elapsedTime >= timeout.value ) {
                    decoratedConnection.Reset();
                    return (Status)timeoutStatus;
                }
            }

            return status;
        }

        protected override void ExtendToDisk(FlatBufferBuilder builder)
        {
            TimeoutData.StartTimeoutData(builder);
            if (timeout.isDefined == false)
                TimeoutData.AddTimeOut(builder, timeout.value);
            builder.Finish(builder.EndTable());
        }

        protected override void ExtendToRAM(FlatBufferBuilder builder)
        {
            var diskData = TimeoutData.GetRootAsTimeoutData(builder.DataBuffer);
            if (timeout.isDefined == false)
                timeout.value = diskData.TimeOut;
        }

        ///----------------------------------------------------------------------------------------------
        ///---------------------------------------UNITY EDITOR-------------------------------------------
#if UNITY_EDITOR

        protected override void OnNodeGUI() {
            GUILayout.Space(25);
            var pRect = new Rect(5, GUILayoutUtility.GetLastRect().y, rect.width - 10, 20);
            var t = 1 - ( elapsedTime / timeout.value );
            UnityEditor.EditorGUI.ProgressBar(pRect, t, elapsedTime > 0 ? string.Format("({0})", elapsedTime.ToString("0.0")) : "Ready");
        }

#endif
        ///----------------------------------------------------------------------------------------------

    }
}