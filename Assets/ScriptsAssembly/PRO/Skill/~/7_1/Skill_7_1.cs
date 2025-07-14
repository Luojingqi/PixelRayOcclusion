using GamePlay.Buff;
using GamePlay.TurnBased;
using UnityEngine;
namespace GamePlay
{
    public class Skill_7_1 : OperateFSMBase
    {
        protected override void InitState()
        {
            AddState(new Skill_7_1_T0());
            AddState(new Skill_7_1_T1());
            AddState(new Skill_7_1_T2());
        }

        public class Skill_7_1_T0 : OperateStateBase_T0
        {
            public override OperateFSMBase Operate { get => operate; set => operate = (Skill_7_1)value; }
            private Skill_7_1 operate;
             

            public override void Trigger()
            {
                operate.Turn.Agent.Select();
            }
        }
        public class Skill_7_1_T1 : OperateStateBase_T1
        {
            public override OperateFSMBase Operate { get => operate; set => operate = (Skill_7_1)value; }
            private Skill_7_1 operate;

            public override void DestroyPointer()
            {
                operate.Turn.Agent.UnSelect();
            }
            public CombatContext context => operate.context;
            public override TriggerState Trigger()
            {
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    return TriggerState.toT2;
                }
                if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    return TriggerState.toT0;
                }
                return TriggerState.update;
            }
        }
        public class Skill_7_1_T2 : OperateStateBase_T2
        {
            public override OperateFSMBase Operate { get => operate; set => operate = (Skill_7_1)value; }
            private Skill_7_1 operate;
            public override void Enter()
            {
                base.Enter();
            }

            protected override TriggerState Trigger()
            {
                operate.Turn.Agent.AddBuff(new Buff_4_0() { Round = 10 });
                operate.Turn.Agent.UpdateBuffUI();
                return TriggerState.toT0;
            }
        }
    }
}