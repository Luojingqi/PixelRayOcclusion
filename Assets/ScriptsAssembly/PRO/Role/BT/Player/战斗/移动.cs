using NodeCanvas.Framework;
using NUnit.Framework;
using PRO.DataStructure;
using System.Collections.Generic;
using UnityEngine;
namespace PRO.BT.战斗
{
    public class 移动 : ActionTask
    {
        public class Data
        {
            public PriorityQueue<Nav.Node> queue = new();
            public Dictionary<Nav.Node, Nav.Node> dic = new();
            public List<Nav.Node> navList = new(36);
            public int index = -1;
            public float deltaTime = 0;

            public List<Particle> list = new();
            public void Clear()
            {
                queue.Clear(); dic.Clear(); navList.Clear();
                index = -1;
                deltaTime = 0;

                var pool = ParticleManager.Inst.GetPool("单像素");
                for (int i = 0; i < list.Count; i++)
                    pool.PutIn(list[i]);
                list.Clear();
            }
        }
        private Data data = new Data();


        public BBParameter<Role> Agent;
        public BBParameter<Vector2Int> 移动目标;
        protected override string OnInit()
        {
            var agent = Agent.value;
            //agent.Info.移动速度.Value_基础 = 5;
            agent.Info.跳跃高度.Value_基础 = 10;
            return base.OnInit();
        }

        protected override void OnExecute()
        {
            var agent = Agent.value;
            Nav.TryNav(agent.Scene, agent.Info.NavMould, agent.Info.移动速度.Value, agent.Info.跳跃高度.Value, agent.GlobalPos, 移动目标.value, data.queue, data.dic, data.navList);
            if (data.navList.Count > 1)
            {
                var pool = ParticleManager.Inst.GetPool("单像素");
                for (int i = 0; i < data.navList.Count; i++)
                {
                    var p = pool.TakeOut(agent.Scene);
                    p.Rig2D.simulated = false;
                    data.list.Add(p);
                    p.SetGlobal(data.navList[i].globalPos);
                    if (data.navList[i].jumpValue == 100)
                        p.Renderer.color = Color.blue;
                }
                data.index = 1;
            }
        }
        protected override void OnUpdate()
        {
            if (data.index == -1) { EndAction(false); return; }
            var agent = Agent.value;
            if (Vector2Int.Distance(agent.GlobalPos, data.navList[data.index - 1].globalPos) > 1.5f)
            { EndAction(false); return; }
            if (agent.GlobalPos == data.navList[data.index].globalPos)
            {
                data.index++;
                if (data.index >= data.navList.Count)
                {
                    EndAction(true);
                    return;
                }
                else
                {
                    var startPos = agent.transform.position;
                    var endPos = Block.GlobalToWorld(data.navList[data.index].globalPos);
                    var v = (endPos - startPos).normalized * (float)agent.Info.移动速度.Value;
                    //agent.V
                }
            }

            //data.deltaTime += TimeManager.deltaTime;

            //var startPos = agent.GlobalPos;
            //var endPos = data.navList[data.index].globalPos;
            //if (startPos.y != endPos.y)
            //{
            //    if (startPos.y > endPos.y)
            //        endPos = new Vector2Int(endPos.x, startPos.y);
            //    else
            //        endPos = new Vector2Int(startPos.x, endPos.y);
            //}
            //var d = ((Vector2)endPos - startPos).normalized * Pixel.Size * data.deltaTime * (float)agent.Info.移动速度.Value;
            //if (d.sqrMagnitude > minDSpr)
            //{
            //    agent.Rig2D.MovePosition((Vector2)agent.transform.position + d);
            //    data.deltaTime = 0;
            //}
        }
        protected override void OnStop()
        {
            data.Clear();
        }

        private static float minDSpr = Mathf.Pow(0.005f, 2);
    }
}