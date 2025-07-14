using DG.Tweening;
using GamePlay.TurnBased;
using PRO;
using PRO.Disk;
using UnityEngine;
namespace GamePlay
{
    public class Skill_3_6 : OperateFSMBase, IOperate_射线选择
    {

        public PixelTypeInfo TypeInfo;
        public PixelColorInfo ColorInfo;

        public SkillPointer_范围内射线类 SkillPointer { get  ; set  ; }
        public Vector2 StartPos { get  ; set  ; }
        public Vector2 EndPos { get  ; set  ; }

        protected override void InitState()
        {
            AddState(new Skill_3_6_T0());
            AddState(new Skill_3_6_T1());
            AddState(new Skill_3_6_T2());
            TypeInfo = Pixel.GetPixelTypeInfo("照明弹");
            ColorInfo = BlockMaterial.GetPixelColorInfo(TypeInfo.availableColors[0]);
        }

        public class Skill_3_6_T0 : OperateStateBase_T0
        {
            public override OperateFSMBase Operate { get => operate; set => operate = (Skill_3_6)value; }
            private Skill_3_6 operate;
             

            public override void Trigger()
            {
            }
        }
        public class Skill_3_6_T1 : OperateStateBase_T1
        {
            public override OperateFSMBase Operate { get => operate; set => operate = (Skill_3_6)value; }
            private Skill_3_6 operate;

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
                var hit = Physics2D.Raycast(operate.SkillPointer.transform.position, 
                    MousePoint.worldPos - (Vector2)operate.SkillPointer.transform.position,
                    operate.SkillPointer.NowPointer.Radius_W,
                    1 << (int)GameLayer.Block);
                if (hit.collider != null)
                {
                    operate.SkillPointer.EndPos = hit.point;

                    if (Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        return TriggerState.toT2;
                    }
                }
                else
                {
                    operate.SkillPointer.EndPos = operate.SkillPointer.StartPos;
                }
                if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    return TriggerState.toT0;
                }
                return TriggerState.update;
            }
        }
        public class Skill_3_6_T2 : OperateStateBase_T2
        {
            public override OperateFSMBase Operate { get => operate; set => operate = (Skill_3_6)value; }
            private Skill_3_6 operate;
            private float time;
            public override void Enter()
            {
                base.Enter();
                float distanceInPixels = (operate.EndPos - operate.StartPos).magnitude / Pixel.Size;
                time = distanceInPixels / (25f + distanceInPixels * 0.1f);

                particle = ParticleManager.Inst.GetPool("单像素").TakeOut(operate.Turn.Agent.Scene);
                particle.transform.rotation = Quaternion.FromToRotation(Vector3.right, operate.EndPos - operate.StartPos);
                particle.Renderer.color = operate.ColorInfo.color;
                particle.RemainTime = (int)(time * 1000);
                particle.transform.position = operate.StartPos;
                particle.transform.DOMove(operate.EndPos, time).SetEase(Ease.Linear);
                source = FreelyLightSource.New(operate.Turn.Agent.Scene, operate.ColorInfo);
            }
            private FreelyLightSource source;
            private Particle particle;
            protected override TriggerState Trigger()
            {
                time = particle.RemainTime / 1000f;
                source.GloabPos = Block.WorldToGlobal(particle.transform.position);
                if (time > 0) return TriggerState.update;
                else
                {
                    source.GloabPos = null;
                    source = null;
                    particle = null;
                    operate.Turn.Agent.Scene.GetBlock(Block.WorldToBlock(operate.EndPos))?.SetPixel(Pixel.TakeOut(operate.TypeInfo, operate.ColorInfo, Block.WorldToPixel(operate.EndPos)));
                    return TriggerState.toT0;
                }
            }
        }
    }
}