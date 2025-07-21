using Google.FlatBuffers;
using PRO.Flat.Ex;
using PRO.Tool;
using PRO.TurnBased;
using UnityEngine;
namespace PRO.Skill
{
    public class Skill_3_3 : OperateFSMBase, ISkillPointer_����ѡ��
    {
        public SkillPointerBase SkillPointerBase { get => skillPointer; set => skillPointer = (SkillPointer_��Χ��������)value; }
        public SkillPointer_��Χ�������� SkillPointer => skillPointer;
        private SkillPointer_��Χ�������� skillPointer;

        public Skill_3_3(string GUID) : base(GUID) { }

        protected override void InitState()
        {
            AddState(new Skill_3_3_T0());
            AddState(new Skill_3_3_T1());
            AddState(new Skill_3_3_T2());
        }

        public class Skill_3_3_T0 : OperateStateBase_T0
        {
            public override OperateFSMBase Operate { get => operate; set => operate = (Skill_3_3)value; }
            private Skill_3_3 operate;
        }
        public class Skill_3_3_T1 : OperateStateBase_T1
        {
            public override OperateFSMBase Operate { get => operate; set => operate = (Skill_3_3)value; }
            private Skill_3_3 operate;

            public CombatContext context => operate.context;
            public override TriggerState Trigger(FlatBufferBuilder record)
            {
                return ISkillPointer_����ѡ��.Trigger(operate, record);
            }

            public override void �ڵ���չ(ref ReusableList<FlatBufferBuilder> recordList)
            {
                ISkillPointer_����ѡ��.�ڵ���չ(operate, ref recordList);
            }

            public override void �ڵ�ִ��(FlatBufferBuilder record, Operator form)
            {
                ISkillPointer_����ѡ��.�ڵ�ִ��(operate, record, form);
            }
        }
        public class Skill_3_3_T2 : OperateStateBase_T2
        {
            public override OperateFSMBase Operate { get => operate; set => operate = (Skill_3_3)value; }
            private Skill_3_3 operate;
            public override void Enter(FlatBufferBuilder record)
            {
                var recordData = Flat.OperateRecord_RaySelect.GetRootAsOperateRecord_RaySelect(record.DataBuffer);
                particle = ParticleManager.Inst.GetPool("���ܲ���").TakeOut(operate.Agent.Scene);
                ParticleDoMove(particle, recordData.StartPos.Value.ToRAM(), recordData.EndPos.Value.ToRAM());
                particle.SkillPlayAgent.Skill = AssetManagerEX.LoadSkillDisk(operate);
                endPos = recordData.EndPos.Value.ToRAM();
            }
            private Particle particle;
            private Vector2 endPos;

            protected override TriggerState Trigger()
            {
                float time = particle.RemainTime / 1000f;
                if (time > 0) return TriggerState.update;
                else return TriggerState.toT0;
            }

            public override void Exit()
            {
                if (Vector3.Distance(particle.transform.position, endPos) > Pixel.Size)
                {
                    operate.context.ClearByAgentData();
                }
                particle = null;
            }

            public override void ToDisk(FlatBufferBuilder builder)
            {
                var particleOffset = particle.ToDisk(builder);
                Flat.Skill_3_3_Disk.StartSkill_3_3_Disk(builder);
                Flat.Skill_3_3_Disk.AddParticle(builder, particleOffset);
                Flat.Skill_3_3_Disk.AddEndPos(builder, endPos.ToDisk(builder));
                builder.Finish(Flat.Skill_3_3_Disk.EndSkill_3_3_Disk(builder).Value);
            }

            public override void ToRAM(FlatBufferBuilder builder)
            {
                var diskData = Flat.Skill_3_3_Disk.GetRootAsSkill_3_3_Disk(builder.DataBuffer);
                particle = ParticleManager.Inst.ToRAM(operate.Agent.Scene, diskData.Particle.Value);
                endPos = diskData.EndPos.Value.ToRAM();
            }
        }
    }
}