using PRO.Skill;
using PRO.Tool;
using UnityEngine;

namespace PRO.TurnBased
{
    /// <summary>
    /// 阶段0：技能释放前置条件判断
    /// </summary>
    public abstract class OperateStateBase_T0 : IFSMState<OperateStateEnum>
    {
        public abstract OperateFSMBase Operate { get; set; }
        public FSMManager<OperateStateEnum> FSM { get => Operate; set => Operate = (OperateFSMBase)value; }

        public OperateStateEnum EnumName => OperateStateEnum.t0;

        public void Enter() { }

        public void Exit() { }

        public void Update()
        {
            if (Input.GetKeyDown(Operate.ShortcutKey)) TrySwitchStateToT1(OperateFSMBase.Operator.Player);
        }


        public bool TrySwitchStateToT1(OperateFSMBase.Operator form)
        {
            if (Operate.NowState.EnumName == OperateStateEnum.t0 && CheckUp())
            {
                Operate.startToward = Operate.Agent.Toward;
                Operate.lastToward = Operate.startToward;

                Operate.context = CombatContext.TakeOut();
                Operate.context.SetAgent(Operate.Agent);
                Operate.context.LogBuilder.Append(Operate.config.Name);
                Operate.context.Calculate_战斗技能初始化(Operate.config.施法type, Operate.config.StartCombatEffectDataList);

                Operate.Agent.gameObject.layer = (int)GameLayer.UnRole;
                if (form == OperateFSMBase.Operator.Player)
                    CreatePointer(Operate.config.SkillPointerLoadPath);
                Trigger();
                Operate.SwitchState(OperateStateEnum.t1);
                Operate.form = form;
                return true;
            }
            else
            {
                return false;
            }
        }

        protected void CreatePointer(string loadPath)
        {
            if (loadPath == string.Empty || Operate is ISkillPointer i == false) return;
            SkillPointerBase skillPointer = AssetManagerEX.LoadSkillPointer<SkillPointerBase>(loadPath);
            i.SkillPointerBase = skillPointer;
            skillPointer.Open();
            Role agent = Operate.Agent;
            skillPointer.SetPosition(agent.GlobalPos + agent.nav.AgentMould.center);
        }

        /// <summary>
        /// 检查此操作是否达到了可执行条件
        /// </summary>
        /// <returns></returns>
        public virtual bool CheckUp() { return Operate.Agent.Info.行动点.Value >= Operate.config.行动点; }
        /// <summary>
        /// 触发一次
        /// </summary>
        public virtual void Trigger() { }
    }
}
