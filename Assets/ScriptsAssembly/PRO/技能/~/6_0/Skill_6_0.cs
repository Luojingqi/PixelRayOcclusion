using GamePlay.TurnBased;
using PRO;
using PRO.SkillEditor;
using UnityEngine;
namespace GamePlay
{
    public class Skill_6_0 : OperateFSMBase, IOperate_范围选择
    {

        public Particle SkillPlayer;
        public Skill_Disk Skill;
        public readonly Vector2Int Offset = new Vector2Int(13, -1);

        public SkillPointer_范围内选择类 SkillPointer { get  ; set  ; }

        protected override void InitState()
        {
            AddState(new Skill_6_0_T0());
            AddState(new Skill_6_0_T1());
            AddState(new Skill_6_0_T2());
        }

        public class Skill_6_0_T0 : OperateStateBase_T0
        {
            public override OperateFSMBase Operate { get => operate; set => operate = (Skill_6_0)value; }
            private Skill_6_0 operate;


            public override void Trigger()
            {
                operate.Skill = AssetManagerEX.LoadSkillDisk(operate);
                operate.SkillPlayer = ParticleManager.Inst.GetPool("通用0").TakeOut(operate.Turn.Agent.Scene);
                operate.Skill.UpdateFrame(operate.SkillPlayer.SkillPlayAgent, operate.Skill.MaxFrame - 1, (int)Skill_Disk.PlayTrack.AnimationTrack2D);
            }
        }
        public class Skill_6_0_T1 : OperateStateBase_T1
        {
            public override OperateFSMBase Operate { get => operate; set => operate = (Skill_6_0)value; }
            private Skill_6_0 operate;

            public override void DestroyPointer()
            {
                operate.SkillPointer.Close();
                operate.SkillPointer = null;
            }
            public override TriggerState Trigger()
            {
                operate.Turn.Agent.LookAt(MousePoint.worldPos);
                if (operate.SkillPointer.Chack(MousePoint.worldPos))
                {
                    operate.SkillPlayer.gameObject.SetActive(true);
                    operate.SkillPlayer.transform.position = Block.GlobalToWorld(MousePoint.globalPos - operate.Offset);
                    if (Chack())
                    {
                        operate.SkillPlayer.Renderer.color = Color.white;
                        operate.SkillPlayer.SkillPlayAgent.UpdateFrameScript(operate.Skill, (int)Skill_Disk.PlayTrack.AnimationTrack2D);

                        if (Input.GetKeyDown(KeyCode.Mouse0))
                        {
                            operate.SkillPlayer.SkillPlayAgent.ClearTimeAndBuffer();
                            return TriggerState.toT2;
                        }
                    }
                    else
                    {
                        operate.SkillPlayer.Renderer.color = Color.red;
                    }
                }
                else
                {
                    operate.SkillPlayer.gameObject.SetActive(false);
                }
                if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    ParticleManager.Inst.GetPoolPutIn(operate.SkillPlayer);
                    operate.SkillPlayer = null;
                    operate.Skill = null;
                    return TriggerState.toT0;
                }
                return TriggerState.update;
            }

            private bool Chack()
            {
                return true;
            }
        }
        public class Skill_6_0_T2 : OperateStateBase_T2
        {
            public override OperateFSMBase Operate { get => operate; set => operate = (Skill_6_0)value; }
            private Skill_6_0 operate;
            private int num = 0;
            public override void Enter()
            {
                base.Enter();
                num = 0;
            }
            protected override TriggerState Trigger()
            {
                if (operate.SkillPlayer.SkillPlayAgent.UpdateFrameScript(operate.Skill, ~(int)Skill_Disk.PlayTrack.EventTrack))
                    num++;
                if (num <= 3) return TriggerState.update;
                else
                {
                    ParticleManager.Inst.GetPoolPutIn(operate.SkillPlayer);
                    operate.SkillPlayer = null;
                    operate.Skill = null;
                    return TriggerState.toT0;
                }
            }
        }
    }
}