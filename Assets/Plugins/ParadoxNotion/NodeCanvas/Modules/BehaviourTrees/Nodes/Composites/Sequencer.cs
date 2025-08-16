using Google.FlatBuffers;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using PRO.BT.Flat.Composite;
using UnityEngine;

namespace NodeCanvas.BehaviourTrees
{

    [Name("Sequencer", 10)]
    [Category("Composites")]
    [Description("Executes its children in order and returns Success if all children return Success. As soon as a child returns Failure, the Sequencer will stop and return Failure as well.")]
    [ParadoxNotion.Design.Icon("Sequencer")]
    [Color("bf7fff")]
    public partial class Sequencer : BTComposite
    {

        [Tooltip("If true, then higher priority children are re-evaluated per tick and if either returns Failure, then the Sequencer will immediately stop and return Failure as well.")]
        public bool dynamic;
        [Tooltip("If true, the children order of execution is shuffled each time the Sequencer resets.")]
        public bool random;

        private int lastRunningNodeIndex = 0;

        protected override Status OnExecute(Component agent, IBlackboard blackboard) {

            for ( var i = dynamic ? 0 : lastRunningNodeIndex; i < outConnections.Count; i++ ) {

                status = outConnections[i].Execute(agent, blackboard);

                switch ( status ) {
                    case Status.Running:

                        if ( dynamic && i < lastRunningNodeIndex ) {
                            for ( var j = i + 1; j <= lastRunningNodeIndex; j++ ) {
                                outConnections[j].Reset();
                            }
                        }

                        lastRunningNodeIndex = i;
                        return Status.Running;

                    case Status.Failure:

                        if ( dynamic && i < lastRunningNodeIndex ) {
                            for ( var j = i + 1; j <= lastRunningNodeIndex; j++ ) {
                                outConnections[j].Reset();
                            }
                        }

                        return Status.Failure;
                }
            }

            return Status.Success;
        }

        protected override void OnReset() {
            lastRunningNodeIndex = 0;
            if ( random ) { outConnections = outConnections.Shuffle(); }
        }

        public override void OnChildDisconnected(int index) {
            if ( index != 0 && index == lastRunningNodeIndex ) {
                lastRunningNodeIndex--;
            }
        }

        public override void OnGraphStarted() { OnReset(); }

        protected override void ExtendToDisk(FlatBufferBuilder builder)
        {
            SequencerData.StartSequencerData(builder);
            SequencerData.AddLastRunningNodeIndex(builder, lastRunningNodeIndex);
            builder.Finish(builder.EndTable());
        }
        protected override void ExtendToRAM(FlatBufferBuilder builder)
        {
            var diskData = SequencerData.GetRootAsSequencerData(builder.DataBuffer);
            lastRunningNodeIndex = diskData.LastRunningNodeIndex;
        }

        ///----------------------------------------------------------------------------------------------
        ///---------------------------------------UNITY EDITOR-------------------------------------------
#if UNITY_EDITOR

        public override string GetConnectionInfo(int index) {
            return random && graph.isRunning ? index.ToString() : null;
        }

        protected override void OnNodeGUI() {
            if ( dynamic ) { GUILayout.Label("<b>DYNAMIC</b>"); }
            if ( random ) { GUILayout.Label("<b>RANDOM</b>"); }
        }
#endif

    }
}