using GamePlay.TurnBased;
using UnityEngine;

namespace GamePlay
{
    public class Skill_1_0 : OperateFSMBase
    {
        protected override void InitState()
        {
            AddState(new Skill_1_0_T0());
            AddState(new Skill_1_0_T1());
            AddState(new Skill_1_0_T2());
            ShortcutKey = KeyCode.Escape;
        }


        private class Skill_1_0_T0 : OperateStateBase_T0
        {
            public override OperateFSMBase Operate { get => operate; set => operate = (Skill_1_0)value; }
            private Skill_1_0 operate;

            public override bool CheckUp()
            {
                return operate.Turn.State_Operate.NowOperateList_T2.Count == 0;
            }

            public override void Trigger()
            {

            }
        }


        private class Skill_1_0_T1 : OperateStateBase_T1
        {
            public override OperateFSMBase Operate { get => operate; set => operate = (Skill_1_0)value; }
            private Skill_1_0 operate;


            public override TriggerState Trigger()
            {
                return TriggerState.toT2;
            }

            public override void DestroyPointer()
            {

            }
        }


        private class Skill_1_0_T2 : OperateStateBase_T2
        {
            public override OperateFSMBase Operate { get => operate; set => operate = (Skill_1_0)value; }

            private Skill_1_0 operate;

            protected override TriggerState Trigger()
            {
                operate.Turn.SwitchState(TurnStateEnum.end);
                operate.context.LogBuilder.Clear();
                return TriggerState.toT0;
            }
        }
    }
}
