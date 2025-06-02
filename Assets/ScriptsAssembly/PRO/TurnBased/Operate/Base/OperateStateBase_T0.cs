using PRO;
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
            if (Input.GetKeyDown(Operate.ShortcutKey)) TrySwitchStateToT1();
        }


        public void TrySwitchStateToT1()
        {
            if (Operate.NowState.EnumName == OperateStateEnum.t0 && CheckUp())
            {
                Operate.startToward = Operate.Turn.Agent.Toward;
                Operate.lastToward = Operate.startToward;

                Operate.context = CombatContext.TakeOut();
                Operate.context.SetAgent(Operate.Turn.Agent, Operate.Turn.RoundFSM, Operate.Turn);
                Operate.context.LogBuilder.Append(Operate.config.Name);
                Operate.context.Calculate_战斗技能初始化(Operate.config.施法type, Operate.config.StartCombatEffectDataList);

                Operate.Turn.Agent.gameObject.layer = (int)GameLayer.UnRole;

                switch (Operate)
                {
                    case IOperate_范围选择 i: { i.SkillPointer = (SkillPointer_范围内选择类)CreatePointer(@"范围内选择类\通用"); i.SkillPointer.SetPointer(Operate.config.Radius_G); break; }
                    case IOperate_射线选择 i: { i.SkillPointer = (SkillPointer_范围内射线类)CreatePointer(@"范围内射线类\白色虚线"); i.SkillPointer.SetPointer(Operate.config.Radius_G); break; }
                }

                Trigger();
                Operate.SwitchState(OperateStateEnum.t1);
            }
        }

        protected SkillPointerBase CreatePointer(string loadPath)
        {
            SkillPointerBase skillPointer;
            skillPointer = AssetManagerEX.LoadSkillPointer<SkillPointerBase>(loadPath);
            skillPointer.Open();
            Role agent = Operate.Turn.Agent;
            skillPointer.SetPosition(agent.GlobalPos + agent.nav.AgentMould.center);

            if (skillPointer is SkillPointer_范围内射线类 skillPointer_射线)
            {
                skillPointer_射线.StartPos = skillPointer_射线.transform.position;
            }
            return skillPointer;
        }

        /// <summary>
        /// 检查此操作是否达到了可执行条件
        /// </summary>
        /// <returns></returns>
        public virtual bool CheckUp() { return Operate.Turn.Agent.Info.行动点.Value >= Operate.config.行动点; }
        /// <summary>
        /// 触发一次
        /// </summary>
        public virtual void Trigger() { }
    }
}
