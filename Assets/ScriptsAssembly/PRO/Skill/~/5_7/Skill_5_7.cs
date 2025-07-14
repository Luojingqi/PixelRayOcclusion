using GamePlay.TurnBased;
using PRO;
using PRO.SkillEditor;
using UnityEngine;

namespace GamePlay
{
    public class Skill_5_7 : OperateFSMBase
    {

        public Particle SkillPlayer;
        public Skill_Disk Skill;
        protected override void InitState()
        {
            AddState(new Skill_5_7_T0());
            AddState(new Skill_5_7_T1());
            AddState(new Skill_5_7_T2());
        }

        public class Skill_5_7_T0 : OperateStateBase_T0
        {
            public override OperateFSMBase Operate { get => operate; set => operate = (Skill_5_7)value; }
            private Skill_5_7 operate;


            public override void Trigger()
            {
                operate.Skill = AssetManagerEX.LoadSkillDisk(operate);
                operate.SkillPlayer = ParticleManager.Inst.GetPool("通用0").TakeOut(operate.Turn.Agent.Scene);
                operate.Skill.UpdateFrame(operate.SkillPlayer.SkillPlayAgent, operate.Skill.MaxFrame - 1, (int)Skill_Disk.PlayTrack.AnimationTrack2D);
            }
        }
        public class Skill_5_7_T1 : OperateStateBase_T1
        {
            public override OperateFSMBase Operate { get => operate; set => operate = (Skill_5_7)value; }
            private Skill_5_7 operate;

            public override void DestroyPointer()
            {
            }

            public override void Enter()
            {
                base.Enter();
                offset = (operate.Skill.EventTrackList[0].SlickList[0] as EventDisk_存储切片_Vector2Int).pos;
            }
            private Vector2Int offset;

            public override TriggerState Trigger()
            {
                operate.Turn.Agent.LookAt(MousePoint.worldPos);
                RotateToMouse(operate.SkillPlayer.transform, operate.Turn.Agent.CenterPos, offset);
                operate.SkillPlayer.SkillPlayAgent.UpdateFrameScript(operate.Skill, (int)Skill_Disk.PlayTrack.AnimationTrack2D);

                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    operate.SkillPlayer.SkillPlayAgent.ClearTimeAndBuffer();
                    return TriggerState.toT2;
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
        public class Skill_5_7_T2 : OperateStateBase_T2
        {
            public override OperateFSMBase Operate { get => operate; set => operate = (Skill_5_7)value; }
            private Skill_5_7 operate;

            protected override TriggerState Trigger()
            {
                if (operate.SkillPlayer.SkillPlayAgent.UpdateFrameScript(operate.Skill, ~(int)Skill_Disk.PlayTrack.AnimationTrack2D) == false)
                {
                    return TriggerState.update;
                }
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
