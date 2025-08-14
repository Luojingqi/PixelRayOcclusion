using Google.FlatBuffers;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using PRO.BT.Flat;
using System;
using UnityEngine;


namespace NodeCanvas.BehaviourTrees
{

    [Name("Parallel", 8)]
    [Category("Composites")]
    [Description("Executes all children simultaneously and return Success or Failure depending on the selected Policy.")]
    [ParadoxNotion.Design.Icon("Parallel")]
    [Color("ff64cb")]
    public partial class Parallel : BTComposite
    {
        public enum ParallelPolicy
        {
            FirstFailure = 0,
            FirstSuccess = 1,
            FirstSuccessOrFailure = 2,
        }

        [Tooltip("The policy determines when the Parallel node will end and return its Status.")]
        public ParallelPolicy policy = ParallelPolicy.FirstFailure;
        [Name("Repeat"), Tooltip("If true, finished children are repeated until the Policy set is met, or until all children have had a chance to finish at least once.")]
        public bool dynamic;

        private bool[] finishedConnections;
        private int finishedConnectionsCount;

        public override void OnGraphStarted() {
            finishedConnections = new bool[outConnections.Count];
            finishedConnectionsCount = 0;
        }

        protected override Status OnExecute(Component agent, IBlackboard blackboard) {

            var defferedStatus = Status.Resting;
            for ( var i = 0; i < outConnections.Count; i++ ) {
                var connection = outConnections[i];
                var isConnectionFinished = finishedConnections[i] == true;

                if ( !dynamic && isConnectionFinished ) {
                    continue;
                }

                if ( connection.status != Status.Running && isConnectionFinished ) {
                    connection.Reset();
                }

                status = connection.Execute(agent, blackboard);

                if ( defferedStatus == Status.Resting ) {
                    if ( status == Status.Failure && ( policy == ParallelPolicy.FirstFailure || policy == ParallelPolicy.FirstSuccessOrFailure ) ) {
                        defferedStatus = Status.Failure;
                    }

                    if ( status == Status.Success && ( policy == ParallelPolicy.FirstSuccess || policy == ParallelPolicy.FirstSuccessOrFailure ) ) {
                        defferedStatus = Status.Success;
                    }
                }

                if ( status != Status.Running && !isConnectionFinished ) {
                    finishedConnections[i] = true;
                    finishedConnectionsCount++;
                }
            }

            if ( defferedStatus != Status.Resting ) {
                ResetRunning();
                status = defferedStatus;
                return defferedStatus;
            }

            if ( finishedConnectionsCount == outConnections.Count ) {
                ResetRunning();
                switch ( policy ) {
                    case ParallelPolicy.FirstFailure:
                        return Status.Success;
                    case ParallelPolicy.FirstSuccess:
                        return Status.Failure;
                }
            }

            return Status.Running;
        }

        protected override void OnReset() {
            for ( var i = 0; i < finishedConnections.Length; i++ ) { finishedConnections[i] = false; }
            finishedConnectionsCount = 0;
        }

        void ResetRunning() {
            for ( var i = 0; i < outConnections.Count; i++ ) {
                if ( outConnections[i].status == Status.Running ) {
                    outConnections[i].Reset();
                }
            }
        }

        protected override void ExtendToDisk(FlatBufferBuilder builder)
        {
            var finishedConnectionsOffset = builder.CreateVector_Data(finishedConnections.AsSpan());
            ParallelData.StartParallelData(builder);
            ParallelData.AddFinishedConnections(builder, finishedConnectionsOffset);
            ParallelData.AddFinishedConnectionsCount(builder, finishedConnectionsCount);
            builder.Finish(builder.EndTable());
        }
        protected override void ExtendToRAM(FlatBufferBuilder builder)
        {
            var diskData = ParallelData.GetRootAsParallelData(builder.DataBuffer);
            for (int i = finishedConnections.Length - 1; i >= 0; i--)
                finishedConnections[finishedConnections.Length - i - 1] = diskData.FinishedConnections(i);
            finishedConnectionsCount = diskData.FinishedConnectionsCount;
        }

        ///----------------------------------------------------------------------------------------------
        ///---------------------------------------UNITY EDITOR-------------------------------------------

#if UNITY_EDITOR

        public override string GetConnectionInfo(int i) {
            if ( dynamic && status == Status.Running ) {
                return string.Format("Branch Finished: {0}", finishedConnections[i].ToString());
            }
            return null;
        }


        protected override void OnNodeGUI() {
            GUILayout.Label(( dynamic ? "<b>REPEAT</b>\n" : string.Empty ) + ( string.Format("<b>{0}</b>", policy.ToString().SplitCamelCase()) ));
        }

#endif
    }
}