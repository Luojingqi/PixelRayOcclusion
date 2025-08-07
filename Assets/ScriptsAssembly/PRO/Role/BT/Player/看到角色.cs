using Opsive.BehaviorDesigner.Runtime.Tasks;
using Opsive.BehaviorDesigner.Runtime.Tasks.Conditionals;
using Opsive.GraphDesigner.Runtime.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PRO.BT
{
    public class 看到角色 : Conditional
    {
        public SharedVariable<Role> Agent;
        public SharedVariable<List<Role>> 视野内角色List;
        public override TaskStatus OnUpdate()
        {
            return base.OnUpdate();
        }
    }
}