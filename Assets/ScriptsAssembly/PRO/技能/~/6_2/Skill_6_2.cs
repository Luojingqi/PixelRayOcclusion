using GamePlay.Buff.Base;
using GamePlay.Buff.Base.BuffAux;
using GamePlay.Buff.Base.IBuff;
using GamePlay.TurnBased;
using PRO;
using PRO.SkillEditor;
using UnityEngine;

namespace GamePlay
{
    public class Skill_6_2 : OperateFSMBase, IOperate_范围选择
    {
        public Particle SkillPlayer;
        public Skill_Disk Skill;

        public SkillPointer_范围内选择类 SkillPointer { get  ; set  ; }

        protected override void InitState()
        {
            AddState(new Skill_6_2_T0());
            AddState(new Skill_6_2_T1());
            AddState(new Skill_6_2_T2());
        }

        public class Skill_6_2_T0 : OperateStateBase_T0
        {
            public override OperateFSMBase Operate { get => operate; set => operate = (Skill_6_2)value; }
            private Skill_6_2 operate;
             

            public override void Trigger()
            {
                operate.Skill = AssetManagerEX.LoadSkillDisk(operate);
                operate.SkillPlayer = ParticleManager.Inst.GetPool("通用0").TakeOut(operate.Turn.Agent.Scene);
                operate.Skill.UpdateFrame(operate.SkillPlayer.SkillPlayAgent, operate.Skill.MaxFrame - 1, (int)Skill_Disk.PlayTrack.AnimationTrack2D);
            }
        }
        public class Skill_6_2_T1 : OperateStateBase_T1
        {
            public override OperateFSMBase Operate { get => operate; set => operate = (Skill_6_2)value; }
            private Skill_6_2 operate;

            public override void DestroyPointer()
            {
                operate.SkillPointer.Close(); operate.SkillPointer = null;
            }

            public override void Enter()
            {
                base.Enter();
                offset = (operate.Skill.EventTrackList[0].SlickList[0] as EventDisk_存储切片_Vector2Int).pos;
            }
            private Vector2Int offset;

            public override TriggerState Trigger()
            {
                operate.SkillPlayer.transform.position = Block.GlobalToWorld(MousePoint.globalPos);
                operate.Turn.Agent.LookAt(MousePoint.worldPos);
                if (operate.SkillPointer.Chack(MousePoint.worldPos))
                {
                    operate.SkillPlayer.gameObject.SetActive(true);

                    var posRotate = UpdatePixelPosRotate();
                    posRotate.RotateTransform(operate.SkillPlayer.transform, offset);

                    operate.SkillPlayer.Renderer.color = Color.white;
                    operate.SkillPlayer.SkillPlayAgent.UpdateFrameScript(operate.Skill, (int)Skill_Disk.PlayTrack.AnimationTrack2D);

                    if (Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        operate.SkillPlayer.Renderer.sprite = null;
                        operate.SkillPlayer.SkillPlayAgent.ClearTimeAndBuffer();
                        return TriggerState.toT2;
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
        }
        public class Skill_6_2_T2 : OperateStateBase_T2
        {
            public override OperateFSMBase Operate { get => operate; set => operate = (Skill_6_2)value; }
            private Skill_6_2 operate;

            protected override TriggerState Trigger()
            {
                operate.Turn.Agent.AddBuff(new Buff(operate.SkillPlayer, 2));
                operate.Turn.Agent.UpdateBuffUI();
                operate.SkillPlayer.SkillPlayAgent.Skill = operate.Skill;
                operate.SkillPlayer = null;
                operate.Skill = null;
                return TriggerState.toT0;
            }
        }

        private class Buff : BuffAuxBase_回合开始, IBuff_回合, IBuff_UI
        {
            public int Round { get; set; }

            public string Info => "小风墙";

            public Particle SkillPlayer;

            public Buff(Particle SkillPlayer, int Round)
            {
                active = true;
                this.SkillPlayer = SkillPlayer;
                this.Round = Round;
            }

            public override void ApplyEffect(CombatContext context, int index)
            {
                Round--;
                if (Round <= 0)
                {
                    ParticleManager.Inst.GetPoolPutIn(SkillPlayer);
                    Agent.RemoveBuff(this);
                }
            }
        }
    }
}
