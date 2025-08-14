using ParadoxNotion;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace NodeCanvas.Tasks.Actions
{

    [Category("✫ Utility")]
    [Description("Force Finish the current graph this Task is assigned to.")]
    public class ForceFinishGraph : ActionTask
    {

        public BooleanStatus finishStatus = BooleanStatus.Success;

        protected override void OnExecute() {
            var graph = ownerSystem as Graph;
            if ( graph != null ) {
                graph.Stop(finishStatus == BooleanStatus.Success);
            }
            EndAction(finishStatus == BooleanStatus.Success);
        }
    }
}