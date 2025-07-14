using GamePlay.TurnBased;
using PRO;
using UnityEngine;
namespace GamePlay
{
    public class Skill_7_0 : OperateFSMBase, IOperate_范围选择
    {
        public Vector2 endPos;

        public SkillPointer_范围内选择类 SkillPointer { get  ; set  ; }

        protected override void InitState()
        {
            AddState(new Skill_7_0_T0());
            AddState(new Skill_7_0_T1());
            AddState(new Skill_7_0_T2());
        }

        public class Skill_7_0_T0 : OperateStateBase_T0
        {
            public override OperateFSMBase Operate { get => operate; set => operate = (Skill_7_0)value; }
            private Skill_7_0 operate;


            public override void Trigger()
            {
                operate.SkillPointer = (SkillPointer_范围内选择类)CreatePointer(@"范围内选择类\通用");
                operate.SkillPointer.SetPointer(30);
                operate.context.LogBuilder.Append($"起始坐标：{operate.Turn.Agent.GlobalPos}");
            }
        }
        public class Skill_7_0_T1 : OperateStateBase_T1
        {
            public override OperateFSMBase Operate { get => operate; set => operate = (Skill_7_0)value; }
            private Skill_7_0 operate;

            public override void DestroyPointer()
            {
                operate.SkillPointer.Close(); operate.SkillPointer = null;
            }
            public CombatContext context => operate.context;
            public override TriggerState Trigger()
            {
                operate.Turn.Agent.SpriteRenderer.color = Color.yellow;
                operate.Turn.Agent.LookAt(MousePoint.worldPos);
                if (operate.SkillPointer.Chack(MousePoint.worldPos))
                {
                    Pixel pixel = operate.Turn.Agent.Scene.GetPixel(BlockBase.BlockType.Block, MousePoint.globalPos);
                    if (pixel != null && pixel.typeInfo.typeName == "水")
                    {
                        operate.endPos = Block.GlobalToWorld(pixel.posG);
                        operate.Turn.Agent.SpriteRenderer.color = Color.green;

                        if (Input.GetKeyDown(KeyCode.Mouse0))
                        {
                            operate.Turn.Agent.SpriteRenderer.color = Color.white;
                            return TriggerState.toT2;
                        }
                    }
                }
                if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    operate.Turn.Agent.SpriteRenderer.color = Color.white;
                    return TriggerState.toT0;
                }
                return TriggerState.update;
            }
        }
        public class Skill_7_0_T2 : OperateStateBase_T2
        {
            public override OperateFSMBase Operate { get => operate; set => operate = (Skill_7_0)value; }
            private Skill_7_0 operate;
            private float time;
            public override void Enter()
            {
                base.Enter();
                time = 0;
            }

            protected override TriggerState Trigger()
            {
                time += TimeManager.deltaTime;
                if (time > 0.05f)
                {
                    operate.Turn.Agent.transform.position = operate.endPos;
                    return TriggerState.toT0;
                }
                return TriggerState.update;
            }
        }
    }
}