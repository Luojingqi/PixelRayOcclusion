using PRO.TurnBased;
using PRO.Tool;
using UnityEngine;
using static PRO.BottomBagM;

namespace PRO
{
    /// <summary>
    /// 操作状态机基类，回合制系统里最小的状态机，所有操作继承此类
    /// </summary>
    public abstract class OperateFSMBase : FSMManager<OperateStateEnum>
    {
        /// <summary>
        /// 本操作受此管理
        /// </summary>
        public TurnFSM Turn { get; set; }

        /// <summary>
        /// 快捷键，使用按键按下触发时可以设置
        /// </summary>
        public KeyCode ShortcutKey = KeyCode.None;
        /// <summary>
        /// 使用事件触发模式，从t0触发进入t1（按键触发优先于事件触发）
        /// </summary>
        public void EventTrigger()
        {
            if (Turn.State_Operate.NowOperate_T1 == null)
                T0.TrySwitchStateToT1();
        }

        public OperateStateBase_T0 T0 { get; private set; }
        public OperateStateBase_T1 T1 { get; private set; }
        public OperateStateBase_T2 T2 { get; private set; }

        public SkillConfig config { get; private set; }

        public OperateFSMBase()
        {
            config = AssetManagerEX.LoadSkillConfig(this);
            InitState();
            T0 = GetState(OperateStateEnum.t0) as OperateStateBase_T0;
            T1 = GetState(OperateStateEnum.t1) as OperateStateBase_T1;
            T2 = GetState(OperateStateEnum.t2) as OperateStateBase_T2;
            if (T0 == null || T1 == null || T2 == null)
            {
                Debug.Log("操作状态机未添加相应的三个状态");
            }
            SwitchState(OperateStateEnum.t0);
        }
        /// <summary>
        /// 为操作状态机添加三个状态
        /// </summary>
        protected abstract void InitState();

        public enum TriggerState
        {
            /// <summary>
            /// 状态还在持续触发中
            /// </summary>
            update,
            /// <summary>
            /// 状态跳转到t0
            /// </summary>
            toT0,
            /// <summary>
            /// 状态跳转到t2
            /// </summary>
            toT2,

        }
        public Toward startToward;
        public Toward lastToward;
        public CombatContext context;


        public GridObject GridUI;
        public void BuildUI(GridObject grid)
        {
            if (grid != null)
            {
                grid.OnReset();
                grid.ImageOnClick += grid => EventTrigger();
                grid.Name.text = config.Name;
                grid.Number.text = $"{config.行动点}x行动点";
                UpdateUI();
            }
            GridUI = grid;
        }
        public virtual void UpdateUI() { }
    }
}
