using Google.FlatBuffers;
using PRO.Tool;
using PRO.TurnBased;
using UnityEngine;
namespace PRO.Skill
{
    /// <summary>
    /// 结束回合
    /// </summary>
    public class Skill_0_0 : OperateFSMBase
    {
        public Skill_0_0(string GUID) : base(GUID) { }

        public SkillPointer_范围内射线类 SkillPointer { get; set; }

        protected override void InitState()
        {
            AddState(new Skill_0_0_T0());
            AddState(new Skill_0_0_T1());
            AddState(new Skill_0_0_T2());
            ShortcutKey = KeyCode.Escape;
        }

        public class Skill_0_0_T0 : OperateStateBase_T0
        {
            public override OperateFSMBase Operate { get => operate; set => operate = (Skill_0_0)value; }
            private Skill_0_0 operate;

            public override bool CheckUp()
            {
                return base.CheckUp() &&  operate.Turn.State_Operate.NowOperateList_T2.Count == 0; ;
            }
        }
        public class Skill_0_0_T1 : OperateStateBase_T1
        {
            public override OperateFSMBase Operate { get => operate; set => operate = (Skill_0_0)value; }
            private Skill_0_0 operate;

            public CombatContext context => operate.context;
            public override TriggerState Trigger(FlatBufferBuilder operateRecord)
            {
                return TriggerState.toT2;
            }

            public override void 节点扩展(ref ReusableList<FlatBufferBuilder> recordList)
            {
                recordList.Add(FlatBufferBuilder.TakeOut(1024));
            }

            public override void 节点执行(FlatBufferBuilder record, Operator form)
            {
            }
        }
        public class Skill_0_0_T2 : OperateStateBase_T2
        {
            public override OperateFSMBase Operate { get => operate; set => operate = (Skill_0_0)value; }
            private Skill_0_0 operate;

            public override void Enter(FlatBufferBuilder operateRecord)
            {
            }

            protected override TriggerState Trigger()
            {
                return TriggerState.toT0;
            }
            public override void Exit()
            {
                operate.Turn.SwitchState(TurnStateEnum.end);
                operate.context.LogBuilder.Clear();
            }

            public override void ToDisk(FlatBufferBuilder builder)
            {
                throw new System.NotImplementedException();
            }

            public override void ToRAM(FlatBufferBuilder builder)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}