using GamePlay.TurnBased;
using PRO;
using PRO.Tool;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
namespace GamePlay
{
    public class Skill_3_3 : OperateFSMBase, IOperate_����ѡ��
    {
        public SkillPointer_��Χ�������� SkillPointer { get; set; }

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
                operateRecord = null;
                return TriggerState.toT0;
                //return OperateEx_����ѡ��.Trigger(operate, out operateRecord);
            }

            public override void ��չ�ڵ�(ref List<IOperateRecord> operateRecordList)
            {
                //OperateEx_����ѡ��.��չ�ڵ�_��Ŀ��(operate, ref operateRecordList);
            }

            public override void �ڵ�ִ��(IOperateRecord operateRecord)
            {
                //OperateEx_����ѡ��.�ڵ�ִ��(operate, operateRecord);
            }
        }
        public class Skill_3_3_T2 : OperateStateBase_T2
        {
            public override OperateFSMBase Operate { get => operate; set => operate = (Skill_3_3)value; }
            private Skill_3_3 operate;
            public override void Enter(IOperateRecord operateRecord)
            {
                //var record = operateRecord as OperateRecord_����ѡ��;
                //particle = ParticleManager.Inst.GetPool("���ܲ���").TakeOut(operate.Turn.Agent.Scene);
                //ParticleDoMove(particle, record.startPos, record.endPos);
                //particle.SkillPlayAgent.Skill = AssetManagerEX.LoadSkillDisk(operate);
                //endPos = record.endPos;
            }
            private Particle particle;
            private Vector2 endPos;
            bool a = true;
            protected override TriggerState Trigger()
            {
                float time = particle.RemainTime / 1000f;

                if (a && time < 1)
                {

                    a = false;
                  //  aa = ToDisk().PutInReturn();
                    Debug.Log(particle.Rig2D.velocity.x + "|" + particle.Rig2D.velocity.y);
                    Debug.Log(aa);
                }

                if (time > 0) return TriggerState.update;
                else return TriggerState.toT0;
            }
            private string aa;
            public override void Exit()
            {
                if (Vector3.Distance(particle.transform.position, endPos) > Pixel.Size)
                {
                    operate.context.ClearByAgentData();
                }
                particle = null;
                if (aa != null)
                {
                   // ToRAM(aa, 0);


                    // Debug.Log(ToDisk());
                }
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
            //    int ret = JsonTool.Deserialize_Ƕ��(text, (pointer) =>
            //    {
            //        switch (pointer.tag)
            //        {
            //            case nameof(Particle):
            //                return ParticleManager.Inst.ToRAM(operate.Turn.Agent.Scene, text, pointer.offset, ref particle);
            //            default:
            //                return JsonTool.Deserialize_ƽ��(text, (list) =>
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