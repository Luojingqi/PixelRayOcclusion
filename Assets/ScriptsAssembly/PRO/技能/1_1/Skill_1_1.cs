//using DG.Tweening;
//using PRO.Buff;
//using PRO.DataStructure;
//using PRO.TurnBased;
//using System.Collections.Generic;
//using UnityEngine;

//namespace PRO.Skill
//{
//    public class Skill_1_1 : OperateFSMBase
//    {

//        protected override void InitState()
//        {
//            AddState(new Skill_1_1_T0());
//            AddState(new Skill_1_1_T1());
//            AddState(new Skill_1_1_T2());
//        }

//        public List<Particle> particleList = new List<Particle>();
//        public PriorityQueue<Vector2Int> queue = new PriorityQueue<Vector2Int>();
//        public Dictionary<Vector2Int, Vector2Int> dic = new Dictionary<Vector2Int, Vector2Int>();
//        public List<Vector2Int> navList = new List<Vector2Int>();

//        public class Skill_1_1_T0 : OperateStateBase_T0
//        {
//            public override OperateFSMBase Operate { get => operate; set => operate = (Skill_1_1)value; }
//            private Skill_1_1 operate;
//            public override bool CheckUp() => base.CheckUp() && operate.Turn.Agent.GetBuff<Buff_2_11>() != null && operate.Turn.Agent.GetBuff<Buff_2_11>().GetActive() == false;

//            public override void Trigger()
//            {

//            }
//        }
//        public class Skill_1_1_T1 : OperateStateBase_T1
//        {
//            public override OperateFSMBase Operate { get => operate; set => operate = (Skill_1_1)value; }
//            private Skill_1_1 operate;

//            public override TriggerState Trigger()
//            {
//                operate.Turn.Agent.LookAt(MousePoint.worldPos);
//                var pool = ParticleManager.Inst.GetPool("单像素");
//                foreach (var p in operate.particleList)
//                    pool.PutIn(p);
//                operate.particleList.Clear();

//                Vector3 m = Input.mousePosition; m.z = 1;
//                m = Camera.main.ScreenToWorldPoint(m);
//                Vector2Int endGlobal = Block.WorldToGlobal(m);
//                Nav nav = NavManager.NavDic["默认"];
//                if (nav.ChackCanNav(operate.Turn.Agent.Scene, endGlobal) == false)
//                {
//                    if (nav.ChackCanNav(operate.Turn.Agent.Scene, endGlobal + Vector2Int.up)) endGlobal += Vector2Int.up;
//                    else
//                    {
//                        bool canNav = false;
//                        for (int y = -1; y >= -5; y--)
//                            if (nav.ChackCanNav(operate.Turn.Agent.Scene, endGlobal + new Vector2Int(0, y)))
//                            {
//                                endGlobal += new Vector2Int(0, y);
//                                canNav = true;
//                                break;
//                            }
//                        if (canNav == false) return TriggerState.update;
//                    }
//                }
//                nav.TryNav(operate.Turn.Agent.Scene, Block.WorldToGlobal(operate.Turn.Agent.transform.position), endGlobal, operate.queue, operate.dic, operate.navList);

//                for (int i = 0; i < operate.navList.Count; i++)
//                {
//                    Particle particle = pool.TakeOut(operate.Turn.Agent.Scene);
//                    particle.SetGlobal(operate.navList[i]);
//                    operate.particleList.Add(particle);
//                    particle.Renderer.color = new Color(0.02f * i, 0.8f, 0.8f, 1);
//                    particle.Rig2D.simulated = false;
//                }
//                if (Input.GetKeyDown(KeyCode.Mouse0) && operate.navList.Count > 0)
//                {
//                    return TriggerState.toT2;
//                }
//                if (Input.GetKeyUp(KeyCode.Mouse1))
//                {
//                    return TriggerState.toT0;
//                }
//                return TriggerState.update;
//            }

//            public override void DestroyPointer()
//            {
//                Debug.Log("De" + operate.particleList.Count);
//                var pool = ParticleManager.Inst.GetPool("单像素");
//                foreach (var p in operate.particleList)
//                    pool.PutIn(p);
//                operate.particleList.Clear();
//            }
//        }
//        public class Skill_1_1_T2 : OperateStateBase_T2
//        {
//            public override OperateFSMBase Operate { get => operate; set => operate = (Skill_1_1)value; }

//            private Skill_1_1 operate;
//            public float time;
//            int index;
//            public override void Enter()
//            {
//                base.Enter();
//                operate.Turn.Agent.Rig2D.simulated = false;
//                Vector3[] path = new Vector3[operate.navList.Count];
//                for (int i = 0; i < operate.navList.Count; i++)
//                    path[i] = Block.GlobalToWorld(operate.navList[i]);
//                operate.Turn.Agent.transform.DOPath(path, 0.1f * operate.navList.Count, PathType.CatmullRom).SetEase(Ease.Linear);
//                time = 0.1f * (operate.navList.Count + 1);
//            }

//            protected override TriggerState Trigger()
//            {
//                time -= TimeManager.deltaTime;
//                if (time <= 0)
//                {
//                    operate.navList.Clear();
//                    operate.Turn.Agent.Rig2D.simulated = true;
//                    return TriggerState.toT0;
//                }
//                return TriggerState.update;
//            }
//        }
//    }
//}
