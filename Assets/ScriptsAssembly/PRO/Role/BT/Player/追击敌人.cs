using Opsive.BehaviorDesigner.Runtime.Tasks;
using Opsive.BehaviorDesigner.Runtime.Tasks.Actions;
using Opsive.GraphDesigner.Runtime.Variables;
using PRO.DataStructure;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PRO.BT
{
    public class 追击敌人 : ActionNode
    {
        public SharedVariable<Role> 敌人;
        public SharedVariable<Role> Agent;

        public class Data
        {
            public PriorityQueue<Vector2Int> queue = new PriorityQueue<Vector2Int>();
            public Dictionary<Vector2Int, Vector2Int> dic = new Dictionary<Vector2Int, Vector2Int>();
            public List<Vector2Int> navList = new List<Vector2Int>(36);
            public int index;

            public void Clear()
            {
                queue.Clear(); dic.Clear(); navList.Clear();
                index = -1;
            }
        }
        private Data data = new Data();

        public override void OnStart()
        {
            var myRole = Agent.Value;
            Nav.TryNav(myRole, myRole.GlobalPos, 敌人.Value.GlobalPos, data.queue, data.dic, data.navList);
            if (data.navList.Count > 1)
            {
                data.index = 1;
            }
            else
            {
                data.Clear();
            }
        }

        //public override TaskStatus OnUpdate()
        //{
        //    if (data.index == -1) return TaskStatus.Failure;
        //}
    }
}