using PRO.Proto.Ex;
using PRO.Skill.Proto;
using PRO.Tool;
using PRO.TurnBased;
using UnityEngine;
namespace PRO.Skill
{
    public class Skill_3_3 : OperateFSMBase, IOperate_射线选择
    {
        public SkillPointer_范围内射线类 SkillPointer { get; set; }

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
            public override TriggerState Trigger(out IOperateRecord operateRecord)
            {
                return IOperate_射线选择.Trigger(operate, out operateRecord);
            }

            public override void 扩展节点(ref ReusableList<IOperateRecord> operateRecordList)
            {
                IOperate_射线选择.扩展节点(operate, ref operateRecordList);
            }

            public override void 节点执行(IOperateRecord operateRecord)
            {
                IOperate_射线选择.节点执行(operate, operateRecord);
            }
        }
        public class Skill_3_3_T2 : OperateStateBase_T2
        {
            public override OperateFSMBase Operate { get => operate; set => operate = (Skill_3_3)value; }
            private Skill_3_3 operate;
            public override void Enter(IOperateRecord operateRecord)
            {
                var record = operateRecord as OperateRecordRaySelect;
                particle = ParticleManager.Inst.GetPool("技能播放").TakeOut(operate.Turn.Agent.Scene);
                ParticleDoMove(particle, record.StartPos.ToRAM(), record.EndPos.ToRAM());
                particle.SkillPlayAgent.Skill = AssetManagerEX.LoadSkillDisk(operate);
                endPos = record.EndPos.ToRAM();
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

            //public override StringBuilder ToDisk()
            //{
            //    var sb = SetPool.TakeOut_SB();
            //    sb.AddStart();
            //    sb.Add_PutIn(particle.ToDisk());
            //    sb.AddStart();
            //    sb.Add(endPos);
            //    sb.AddEnd();
            //    sb.AddEnd();
            //    return sb;
            //}

            //public override int ToRAM(string text, int offset)
            //{
            //    int ret = JsonTool.Deserialize_嵌套(text, (pointer) =>
            //    {
            //        switch (pointer.tag)
            //        {
            //            case nameof(Particle):
            //                return ParticleManager.Inst.ToRAM(operate.Turn.Agent.Scene, text, pointer.offset, ref particle);
            //            default:
            //                return JsonTool.Deserialize_平行(text, (list) =>
            //                {
            //                    int index = 0;
            //                    endPos = new Vector2(list[index++].ToFloat(), list[index++].ToFloat());
            //                },
            //                pointer.offset);
            //        }
            //    },
            //    offset);
            //    Debug.Log(ToDisk().PutInReturn());
            //    operate.NowState = operate.T2;
            //    operate.Turn.State_Operate.NowOperateList_T2.Add(operate);
            //    return ret;
            //}
        }
    }
}