using Google.FlatBuffers;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using PRO.BT.Flat;
using UnityEngine;


namespace NodeCanvas.BehaviourTrees
{

    [Name("Selector", 9)]
    [Category("Composites")]
    [Description("Executes its children in order and returns Failure if all children return Failure. As soon as a child returns Success, the Selector will stop and return Success as well.")]
    [ParadoxNotion.Design.Icon("Selector")]
    [Color("b3ff7f")]
    public partial class Selector : BTComposite
    {

        [Tooltip("If true, then higher priority children are re-evaluated per tick and if either returns Success, then the Selector will immediately stop and return Success as well.")]
        public bool dynamic;
        [Tooltip("If true, the children order of execution is shuffled each time the Selector resets.")]
        public bool random;

        private int lastRunningNodeIndex;

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

                    case Status.Success:

                        if ( dynamic && i < lastRunningNodeIndex ) {
                            for ( var j = i + 1; j <= lastRunningNodeIndex; j++ ) {
                                outConnections[j].Reset();
                            }
                        }

                        return Status.Success;
                }
            }

            return Status.Failure;
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
            SelectorData.StartSelectorData(builder);
            SelectorData.AddLastRunningNodeIndex(builder, lastRunningNodeIndex);
            builder.Finish(builder.EndTable());
        }
        protected override void ExtendToRAM(FlatBufferBuilder builder)
        {
            var diskData = SelectorData.GetRootAsSelectorData(builder.DataBuffer);
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