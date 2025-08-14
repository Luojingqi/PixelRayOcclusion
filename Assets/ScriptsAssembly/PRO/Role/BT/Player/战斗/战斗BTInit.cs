using NodeCanvas.Framework;
namespace PRO.BT.战斗
{
    public class 战斗BTInit : ActionTask
    {
        protected override string OnInit()
        {
            blackboard.GetVariable<Role>("Agent").value = agent.GetComponent<Role>();
            
            return null;
        }
        protected override void OnUpdate()
        {
            EndAction(true);
        }
    }
}