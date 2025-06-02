using GamePlay.TurnBased;
using PRO;
using PRO.SkillEditor;
using UnityEngine;

namespace GamePlay
{
    public class Skill_5_0 : OperateFSMBase, IOperate_·¶Î§Ñ¡Ôñ
    {

        public Particle SkillPlayer;
        public Skill_Disk Skill;

        public SkillPointer_·¶Î§ÄÚÑ¡ÔñÀà SkillPointer { get  ; set  ; }

        protected override void InitState()
        {
            AddState(new Skill_5_0_T0());
            AddState(new Skill_5_0_T1());
            AddState(new Skill_5_0_T2());
        }

        public class Skill_5_0_T0 : OperateStateBase_T0
        {
            public override OperateFSMBase Operate { get => operate; set => operate = (Skill_5_0)value; }
            private Skill_5_0 operate;


            public override void Trigger()
            {

                operate.Skill = AssetManagerEX.LoadSkillDisk(operate);
                operate.SkillPlayer = ParticleManager.Inst.GetPool("Í¨ÓÃ0").TakeOut(operate.Turn.Agent.Scene);
                operate.Skill.UpdateFrame(operate.SkillPlayer.SkillPlayAgent, operate.Skill.MaxFrame - 1, (int)Skill_Disk.PlayTrack.AnimationTrack2D);
            }
        }
        public class Skill_5_0_T1 : OperateStateBase_T1
        {
            public override OperateFSMBase Operate { get => operate; set => operate = (Skill_5_0)value; }
            private Skill_5_0 operate;

            public override void DestroyPointer()
            {
                operate.SkillPointer.Close(); operate.SkillPointer = null;
            }

            public override void Enter()
            {
                base.Enter();
                offset = (operate.Skill.EventTrackList[0].SlickList[0] as EventDisk_´æ´¢ÇÐÆ¬_Vector2Int).pos;
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

                    if (Chack(posRotate))
                    {
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

            private bool Chack(PixelPosRotate posRotate)
            {
                Vector2Int start = Block.WorldToGlobal(operate.SkillPlayer.transform.position);
                int num = 0;
                for (int x = 0; x < 5; x++)
                {
                    Pixel pixel = operate.Turn.Agent.Scene.GetPixel(BlockBase.BlockType.Block, start + posRotate.RotatePos(new Vector2Int(x, offset.y)));
                    if (pixel.typeInfo.typeName == "ÑÒÊ¯")
                        num++;
                }
                if (num >= 3) return true;
                else return false;
            }
        }
        public class Skill_5_0_T2 : OperateStateBase_T2
        {
            public override OperateFSMBase Operate { get => operate; set => operate = (Skill_5_0)value; }
            private Skill_5_0 operate;

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
