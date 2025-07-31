using GamePlay.TurnBased;
using PRO;
using UnityEngine;
namespace GamePlay
{
    public class Skill_3_5 : OperateFSMBase, IOperate_范围选择
    { 
        public Role ByAgent;
        public Particle CloneByAgent;
        public Vector2 d;

        public SkillPointer_范围内选择类 SkillPointer { get  ; set  ; }

        protected override void InitState()
        {
            AddState(new Skill_3_5_T0());
            AddState(new Skill_3_5_T1());
            AddState(new Skill_3_5_T2());
        }

        public class Skill_3_5_T0 : OperateStateBase_T0
        {
            public override OperateFSMBase Operate { get => operate; set => operate = (Skill_3_5)value; }
            private Skill_3_5 operate;
             

            public override void Trigger()
            {
                operate.CloneByAgent = ParticleManager.Inst.GetPool("通用0").TakeOut(operate.Turn.Agent.Scene);
                operate.CloneByAgent.Renderer.color = new Color(1, 1, 1, 0.5f);
            }
        }
        public class Skill_3_5_T1 : OperateStateBase_T1
        {
            public override OperateFSMBase Operate { get => operate; set => operate = (Skill_3_5)value; }
            private Skill_3_5 operate;

            public override void DestroyPointer()
            {
                operate.SkillPointer.Close(); operate.SkillPointer = null;
                operate.ByAgent?.UnSelect();
                ParticleManager.Inst.GetPool(operate.CloneByAgent.loadPath).PutIn(operate.CloneByAgent);
                operate.CloneByAgent = null;
            }
            public override TriggerState Trigger()
            {
                operate.CloneByAgent.Renderer.sprite = null;
                operate.ByAgent?.UnSelect();
                operate.Turn.Agent.LookAt(MousePoint.worldPos);
                if (operate.SkillPointer.Chack(MousePoint.worldPos))
                {
                    var hit = Physics2D.Raycast(MousePoint.worldPos, Vector2.zero, 0, 1 << (int)GameLayer.Role);
                    if (GamePlayMain.Inst.GetRole(hit.transform, out Role role) && role != operate.Turn.Agent)
                    {
                        operate.ByAgent = role;
                        operate.ByAgent.Select();
                        operate.CloneByAgent.Renderer.sprite = operate.ByAgent.Icon;
                        operate.CloneByAgent.Renderer.flipY = operate.ByAgent.SpriteRenderer.flipY;
                        operate.d = operate.ByAgent.transform.position - operate.Turn.Agent.transform.position;
                        operate.d.Normalize();
                        operate.CloneByAgent.transform.position = operate.ByAgent.transform.position + (Vector3)operate.d * 5 * Pixel.Size;
                    }
                }

                if (Input.GetKeyDown(KeyCode.Mouse0) && operate.ByAgent != null)
                {
                    return TriggerState.toT2;
                }
                if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    operate.ByAgent = null;
                    return TriggerState.toT0;
                }
                return TriggerState.update;
            }
        }
        public class Skill_3_5_T2 : OperateStateBase_T2
        {
            public override OperateFSMBase Operate { get => operate; set => operate = (Skill_3_5)value; }
            private Skill_3_5 operate;

            protected override TriggerState Trigger()
            {
                operate.ByAgent.Rig2D.AddForce(operate.d * 70);
                return TriggerState.toT0;
            }
        }
    }
}