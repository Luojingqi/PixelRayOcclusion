using NodeCanvas.BehaviourTrees;
using NodeCanvas.Framework;
using UnityEngine;


namespace PRO.BT.Tool
{
    public class 获取角色坐标 : BTDecorator
    {

        public BBParameter<Role> Role;
        public BBParameter<Vector2Int> GlobalPos;

        protected override Status OnExecute(Component agent, IBlackboard blackboard)
        {
            GlobalPos.value = Role.value.GlobalPos;
            return decoratedConnection.Execute(agent, blackboard);
        }
    }
}