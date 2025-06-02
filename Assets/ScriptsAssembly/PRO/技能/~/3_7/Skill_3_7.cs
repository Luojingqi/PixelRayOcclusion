using GamePlay.TurnBased;
using PRO;
using UnityEngine;
namespace GamePlay
{
    public class Skill_3_7 : OperateFSMBase, IOperate_范围选择
    {
        public SkillPointer_范围内选择类 SkillPointer { get  ; set  ; }

        protected override void InitState()
        {
            AddState(new Skill_3_7_T0());
            AddState(new Skill_3_7_T1());
            AddState(new Skill_3_7_T2());
        }

        public class Skill_3_7_T0 : OperateStateBase_T0
        {
            public override OperateFSMBase Operate { get => operate; set => operate = (Skill_3_7)value; }
            private Skill_3_7 operate;
             

            public override void Trigger()
            {
                operate.context.Calculate_战斗技能初始化(施法方式.近战, stackalloc StartCombatEffectData[]
                {
                    new (属性.电 , 3)
                });
            }
        }
        public class Skill_3_7_T1 : OperateStateBase_T1
        {
            public override OperateFSMBase Operate { get => operate; set => operate = (Skill_3_7)value; }
            private Skill_3_7 operate;

            public override void DestroyPointer()
            {
                operate.SkillPointer.Close(); operate.SkillPointer = null;
            }
            public CombatContext context => operate.context;
            public override TriggerState Trigger()
            {
                operate.Turn.Agent.LookAt(MousePoint.worldPos);
                context.ClearByAgentData();
                if (operate.SkillPointer.Chack(MousePoint.worldPos))
                {
                    var hit = Physics2D.Raycast(MousePoint.worldPos, Vector2.zero, 0, 1 << (int)GameLayer.Role);
                    if (GamePlayMain.Inst.GetRole(hit.transform, out Role byRole) && byRole != operate.Turn.Agent)
                    {
                        context.AddByAgent(byRole, true);
                    }
                }

                if (Input.GetKeyDown(KeyCode.Mouse0))
                    return TriggerState.toT2;
                if (Input.GetKeyDown(KeyCode.Mouse1))
                    return TriggerState.toT0;
                return TriggerState.update;
            }
        }
        public class Skill_3_7_T2 : OperateStateBase_T2
        {
            public override OperateFSMBase Operate { get => operate; set => operate = (Skill_3_7)value; }
            private Skill_3_7 operate;
            private float time;
            public override void Enter()
            {
                base.Enter();
                time = 0;
            }

            protected override TriggerState Trigger()
            {
                time += TimeManager.deltaTime;
                if (time > 0.15f)
                {
                    return TriggerState.toT0;
                }
                return TriggerState.update;
            }
        }
    }
}