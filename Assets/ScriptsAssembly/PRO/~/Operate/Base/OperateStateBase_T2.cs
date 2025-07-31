using Google.FlatBuffers;
using Google.Protobuf;
using PRO.Skill;
using PRO.Tool;
using UnityEngine;

namespace PRO.TurnBased
{
    /// <summary>
    /// T2：技能执行
    /// </summary>
    public abstract class OperateStateBase_T2 : IFSMState<OperateStateEnum>
    {
        public abstract OperateFSMBase Operate { get; set; }
        public FSMManager<OperateStateEnum> FSM { get => Operate; set => Operate = (OperateFSMBase)value; }

        public OperateStateEnum EnumName => OperateStateEnum.t2;

        public void Enter()
        {
            Operate.Agent.Info.行动点.Value -= Operate.config.行动点;
        }

        public abstract void Exit();

        public void Update()
        {
            OperateFSMBase.TriggerState state = Trigger();
            if (state == OperateFSMBase.TriggerState.toT0)
            {
                Operate.context.Calculate_最终结算();
                Operate.Agent.ForEachBuffApplyEffect(BuffTriggerType.技能释放后, Operate.context, -1);
                LogPanelC.Inst.AddLog(Operate.context, true);
                Operate.SwitchState(OperateStateEnum.t0);
                CombatContext.PutIn(Operate.context);
                Operate.context = null;
                Operate.form = OperateFSMBase.Operator.Not;
            }
        }
        private float v = Pixel.Size * 25f;
        protected void ParticleDoMove(Particle particle, Vector2 startWorld, Vector2 endWorld)
        {
            Vector2 d = endWorld - startWorld;
            float m = d.magnitude;
            Vector2 n = d / m;
            particle.transform.rotation = Quaternion.FromToRotation(Vector3.right, d);
            particle.RemainTime = (int)(m / v * 1000);
            particle.transform.position = startWorld;
            particle.Rig2D.velocity = n * v;
        }

        public abstract void Enter(FlatBufferBuilder operateRecord);

        /// <summary>
        /// 操作的实际执行过程，只能返回toT0或者update，返回toT1无效
        /// </summary>
        /// <returns></returns>
        protected abstract OperateFSMBase.TriggerState Trigger();

        public abstract void ToDisk(FlatBufferBuilder builder);
        public abstract void ToRAM(FlatBufferBuilder builder);
    }
}
