using GamePlay.TurnBased;
using PRO;
using PRO.SkillEditor;
using System.Collections.Generic;
using UnityEngine;
using static GamePlay.CombatContext;

namespace GamePlay
{
    public class Skill_3_4 : OperateFSMBase
    {
        public Particle SkillPlayer;
        public Skill_Disk Skill;

        private Dictionary<Role, CombatContext_ByAgentData> ByRoleDic = new Dictionary<Role, CombatContext_ByAgentData>();


        public void PutIn()
        {
            ParticleManager.Inst.GetPool(SkillPlayer.loadPath).PutIn(SkillPlayer);
            SkillPlayer = null;
            Skill = null;
        }
        

        protected override void InitState()
        {
            AddState(new Skill_3_4_T0());
            AddState(new Skill_3_4_T1());
            AddState(new Skill_3_4_T2());
        }

        public class Skill_3_4_T0 : OperateStateBase_T0
        {
            public override OperateFSMBase Operate { get => operate; set => operate = (Skill_3_4)value; }
            private Skill_3_4 operate;
             

            public override void Trigger()
            {
                operate.Skill = AssetManagerEX.LoadSkillDisk(operate);
                operate.SkillPlayer = ParticleManager.Inst.GetPool("ͨ��0").TakeOut(operate.Turn.Agent.Scene);
                Role agent = operate.Turn.Agent;
                operate.SkillPlayer.transform.position = agent.transform.position + (Vector3)agent.nav.AgentMould.centerW / 2;


                operate.context.Calculate_ս�����ܳ�ʼ��(ʩ����ʽ.��ս, stackalloc StartCombatEffectData[]
                {
                    new (����.��� , 3)
                });
            }
        }
        public class Skill_3_4_T1 : OperateStateBase_T1
        {
            public override OperateFSMBase Operate { get => operate; set => operate = (Skill_3_4)value; }
            private Skill_3_4 operate;

            public override void DestroyPointer()
            {

            }
            public override TriggerState Trigger(out IOperateRecord operateRecord)
            {
                operateRecord = null;
                operate.context.ClearByAgentData();
                operate.ByRoleDic.Clear();
                operate.Turn.Agent.LookAt(MousePoint.worldPos);
                //���ü��ܳ���
                if (operate.Turn.Agent.Toward == Toward.right) operate.SkillPlayer.transform.rotation = Quaternion.identity;
                else operate.SkillPlayer.transform.rotation = Quaternion.Euler(0, 180, 0);
                //������һ֡�Ķ���
                operate.SkillPlayer.SkillPlayAgent.UpdateFrameScript(operate.Skill, ~(int)Skill_Disk.PlayTrack.AttackTestTrack2D);
                //�������еĹ�������������ҽ���⵽�ļ�����������
                for (int index = 0; index < operate.Skill.MaxFrame; index++)
                    operate.Skill.UpdateFrame(operate.SkillPlayer.SkillPlayAgent, index, (int)Skill_Disk.PlayTrack.AttackTestTrack2D);
                //var data = operate.SkillPlayer.SkillPlayAgent.GetBufferData<AttackTestSlice2DBase_Disk.BufferData>("Ŀ��ѡȡ");
                //if (data != null)
                //    foreach (var item in data.value)
                //    {
                //        Role byRole = GamePlayMain.Inst.GetRole(item.transform);
                //        if (byRole != null && operate.ByRoleDic.ContainsKey(byRole) == false && byRole != operate.Turn.Agent)
                //        {
                //            operate.context.AddByAgent(byRole, true);
                //            operate.ByRoleDic.Add(byRole, operate.context.ByAgentDataList[operate.context.ByAgentDataList.Count - 1]);
                //        }
                //    }
               // operate.SkillPlayer.SkillPlayAgent.ClearBuffer();

                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    operate.SkillPlayer.SkillPlayAgent.ClearTimeAndBuffer();
                    return TriggerState.toT2;
                }
                if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    operate.PutIn();
                    operate.ByRoleDic.Clear();
                    return TriggerState.toT0;
                }
                return TriggerState.update;
            }


            public override void ��չ�ڵ�(ref List<IOperateRecord> operateRecordList)
            {
                throw new System.NotImplementedException();
            }

            public override void �ڵ�ִ��(IOperateRecord operateRecord)
            {
                throw new System.NotImplementedException();
            }
        }
        public class Skill_3_4_T2 : OperateStateBase_T2
        {
            public override OperateFSMBase Operate { get => operate; set => operate = (Skill_3_4)value; }
            private Skill_3_4 operate;

            protected override TriggerState Trigger()
            {
                if (operate.SkillPlayer.SkillPlayAgent.UpdateFrameScript(operate.Skill, ~0, false) == false)
                {
                    //var data = operate.SkillPlayer.SkillPlayAgent.GetBufferData<AttackTestSlice2DBase_Disk.BufferData>("Ŀ��ѡȡ");
                    //if (data != null)
                    //{
                    //    foreach (var item in data.value)
                    //    {
                    //        Role role = GamePlayMain.Inst.GetRole(item.transform);
                    //        if (role != null && operate.ByRoleDic.TryGetValue(role, out var byAgentData))
                    //        {
                    //            byAgentData.PlayAffectedAnimation = false;
                    //            role.Play������Animation();
                    //            operate.ByRoleDic.Remove(role);
                    //        }
                    //    }
                    //    data.value.Clear();
                    //}
                    return TriggerState.update;
                }
                else
                {
                    operate.SkillPlayer.SkillPlayAgent.ClearTimeAndBuffer();
                    operate.PutIn();
                    return TriggerState.toT0;
                }
            }

            public override void Exit()
            {
                throw new System.NotImplementedException();
            }

            public override void Enter(IOperateRecord operateRecord)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
