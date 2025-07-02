//using PRO.TurnBased;
//using System.Collections.Generic;
//namespace PRO.Skill
//{
//    public class Skill_0_0 : OperateFSMBase, IOperate_射线选择
//    {
//        public SkillPointer_范围内射线类 SkillPointer { get; set; }

//        protected override void InitState()
//        {
//            AddState(new Skill_0_0_T0());
//            AddState(new Skill_0_0_T1());
//            AddState(new Skill_0_0_T2());
//        }

//        public class Skill_0_0_T0 : OperateStateBase_T0
//        {
//            public override OperateFSMBase Operate { get => operate; set => operate = (Skill_0_0)value; }
//            private Skill_0_0 operate;
//        }
//        public class Skill_0_0_T1 : OperateStateBase_T1
//        {
//            public override OperateFSMBase Operate { get => operate; set => operate = (Skill_0_0)value; }
//            private Skill_0_0 operate;

//            public CombatContext context => operate.context;
//            public override TriggerState Trigger(out IOperateRecord operateRecord)
//            {
//                operateRecord = null;
//                return TriggerState.toT2;
//            }

//            public override void 扩展节点(ref List<IOperateRecord> operateRecordList)
//            {
//                //for (int i = 0; i < 6; i++)
//                //{
//                //    var record = OperateRecord_TimeDelay.TakeOut();
//                //    operateRecordList.Add(record);
//                //    record.time = (i + 1) * 0.5f;
//                //}
//            }

//            public override void 节点执行(IOperateRecord operateRecord)
//            {
//                //var record = operateRecord as OperateRecord_TimeDelay;
//                //while (record.time > 0)
//                //{
//                //    record.time -= TimeManager.physicsDeltaTime;
//                //    TimeManager.Inst.ScriptUpdate(TimeManager.physicsDeltaTime);
//                //}
//            }

//            //public class OperateRecord_TimeDelay : IOperateRecord
//            //{
//            //    private static ObjectPool<OperateRecord_TimeDelay> pool = new ObjectPool<OperateRecord_TimeDelay>();
//            //    public static OperateRecord_TimeDelay TakeOut() => pool.TakeOut();
//            //    public override void PutIn()
//            //    {
//            //        pool.PutIn(this);
//            //    }

//            //    public float time;
//            //}
//        }
//        public class Skill_0_0_T2 : OperateStateBase_T2
//        {
//            public override OperateFSMBase Operate { get => operate; set => operate = (Skill_0_0)value; }
//            private Skill_0_0 operate;

//            public override void Enter(IOperateRecord operateRecord)
//            {
//            }

//            protected override TriggerState Trigger()
//            {
//                return TriggerState.toT0;
//            }

//            public override void Exit()
//            {
//                throw new System.NotImplementedException();
//            }

//            //public override StringBuilder ToDisk()
//            //{
//            //    throw new System.NotImplementedException();
//            //}

//            //public override int ToRAM(string text, int offset)
//            //{
//            //    throw new System.NotImplementedException();
//            //}
//        }
//    }
//}