using DG.Tweening;
using GamePlay.TurnBased;
using PRO;
using PRO.Tool;
using UnityEngine;
namespace GamePlay
{
    public class Skill_5_2 : OperateFSMBase, IOperate_射线选择
    {
        public SkillPointer_范围内射线类 SkillPointer { get  ; set  ; }
        public Vector2 StartPos { get  ; set  ; }
        public Vector2 EndPos { get  ; set  ; }

        protected override void InitState()
        {
            AddState(new Skill_5_2_T0());
            AddState(new Skill_5_2_T1());
            AddState(new Skill_5_2_T2());
        }

        public class Skill_5_2_T0 : OperateStateBase_T0
        {
            public override OperateFSMBase Operate { get => operate; set => operate = (Skill_5_2)value; }
            private Skill_5_2 operate;
             

            public override void Trigger()
            {
                operate.context.Calculate_战斗技能初始化(施法方式.远程, stackalloc StartCombatEffectData[]
                {
                    new(属性.毒 , 1)
                });
            }
        }
        public class Skill_5_2_T1 : OperateStateBase_T1
        {
            public override OperateFSMBase Operate { get => operate; set => operate = (Skill_5_2)value; }
            private Skill_5_2 operate;

            public override void DestroyPointer()
            {
                operate.StartPos = operate.SkillPointer.StartPos;
                operate.EndPos = operate.SkillPointer.EndPos;

                operate.SkillPointer.Close();
                operate.SkillPointer = null;
            }
            public CombatContext context => operate.context;
            public override TriggerState Trigger()
            {
                operate.Turn.Agent.LookAt(MousePoint.worldPos);
                context.ClearByAgentData();
                Vector2 d = MousePoint.worldPos - (Vector2)operate.SkillPointer.transform.position;
                var hit = Physics2D.Raycast(operate.SkillPointer.StartPos, d, operate.SkillPointer.NowPointer.Radius_W,
                   1 << (int)GameLayer.Role | 1 << (int)GameLayer.Block);
                if (hit.collider != null)
                {
                    if (GamePlayMain.Inst.GetRole(hit.transform, out Role byRole))
                    {
                        context.AddByAgent(byRole, true);
                    }
                    operate.SkillPointer.EndPos = hit.point;
                }
                else
                {
                    operate.SkillPointer.EndPos = operate.SkillPointer.StartPos + d.normalized * operate.SkillPointer.NowPointer.Radius_W;
                }
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
        public class Skill_5_2_T2 : OperateStateBase_T2
        {
            public override OperateFSMBase Operate { get => operate; set => operate = (Skill_5_2)value; }
            private Skill_5_2 operate;
            private float time;
            public override void Enter()
            {
                base.Enter();
                float distanceInPixels = (operate.EndPos - operate.StartPos).magnitude / Pixel.Size;
                time = distanceInPixels / (25f + distanceInPixels * 0.1f);

                particle = ParticleManager.Inst.GetPool("通用0").TakeOut(operate.Turn.Agent.Scene);
                particle.transform.rotation = Quaternion.FromToRotation(Vector3.right, operate.EndPos - operate.StartPos);
                particle.Renderer.color = BlockMaterial.GetPixelColorInfo("毒液色0").color;
                particle.RemainTime = (int)(time * 1000);
                particle.transform.position = operate.StartPos;
                particle.transform.DOMove(operate.EndPos, time).SetEase(Ease.Linear);
                particle.SkillPlayAgent.Skill = AssetManagerEX.LoadSkillDisk(operate);

                particle.RemainTimeIsZeroEvent += (p) =>
                {
                    var pool = ParticleManager.Inst.GetPool("特殊粒子/毒液");
                    Vector2Int endGlobalPos = Block.WorldToGlobal(operate.EndPos);
                    var list = SetPool.TakeOut_List_Vector2Int();
                    DrawTool.GetCircle(endGlobalPos, 2, ref list);
                    foreach (var globalPos in list)
                    {
                        Vector2 d = (Vector2)(globalPos - endGlobalPos);
                        d.Normalize();
                        var particle = pool.TakeOut(p.Scene);
                        particle.SetGlobal(globalPos);
                        particle.Rig2D.AddForce(Quaternion.Euler(0, Random.Range(-100, 100), 0) * d * Random.Range(10, 30));
                        particle.RemainTime = 5000;
                    }
                };
            }
            private Particle particle;

            protected override TriggerState Trigger()
            {
                time = particle.RemainTime / 1000f;
                if (time > 0) return TriggerState.update;
                else
                {
                    particle = null;
                    return TriggerState.toT0;
                }
            }
        }

    }
}