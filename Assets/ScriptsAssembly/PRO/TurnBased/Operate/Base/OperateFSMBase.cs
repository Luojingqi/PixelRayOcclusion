using Google.FlatBuffers;
using PRO.Skill.Base;
using PRO.Tool;
using PRO.TurnBased;
using PROTool;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using static PRO.BottomBagM;

namespace PRO.Skill
{
    /// <summary>
    /// 操作状态机基类，回合制系统里最小的状态机，所有操作继承此类
    /// </summary>
    public abstract class OperateFSMBase : FSMManager<OperateStateEnum>
    {
        #region 反射创建Operate实例
        private static Dictionary<string, CreateOperateInfo> OperateTypeDic;
        private class CreateOperateInfo
        {
            public Type type;
            public ConstructorInfo constructor;

            public CreateOperateInfo(Type type, ConstructorInfo constructor)
            {
                this.type = type;
                this.constructor = constructor;
            }
        }
        public static void InitOperateType()
        {
            Type[] bind = new Type[] { typeof(string) };
            var typeList = ReflectionTool.GetDerivedClasses(typeof(OperateFSMBase));
            OperateTypeDic = new(typeList.Count);
            for (int i = 0; i < typeList.Count; i++)
            {
                var type = typeList[i];
                var info = type.GetConstructor(bind);
                if (info != null)
                {
                    OperateTypeDic.Add(type.Name, new(type, info));
                }
            }
        }
        private static object[] parameter = new object[1];
        public static OperateFSMBase CreateOperate(string typeName, string guid)
        {
            OperateFSMBase ret = null;
            if (OperateTypeDic.TryGetValue(typeName, out var info))
            {
                parameter[0] = guid;
                ret = info.constructor.Invoke(parameter) as OperateFSMBase;
                parameter[0] = null;
            }
            return ret;
        }
        public static OperateFSMBase CreateOperate(PRO.Flat.OperatBasiceData diskData)
        {
            OperateFSMBase ret = CreateOperate(diskData.Type, diskData.Guid);
            ret?.ToRAM(diskData);
            return ret;
        }
        #endregion

        public Role Agent;

        public TurnFSM Turn => Agent.Turn;

        public string GUID => guid;
        private string guid;
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

        public OperateFSMBase(string GUID)
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
            SetState(OperateStateEnum.t0);
            if (GUID == null) guid = Guid.NewGuid().ToString();
            guid = GUID;
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

        public enum Operator
        {
            Player,
            AI,
        }

        public Offset<PRO.Flat.OperatBasiceData> ToDisk(FlatBufferBuilder builder)
        {
            var skillTypeOffset = builder.CreateString(GetType().Name);
            var skillGuidOffset = builder.CreateString(GUID);
            var extendBuilder = FlatBufferBuilder.TakeOut(1024);
            ExtendDataToDisk(extendBuilder);
            var extendOffset = builder.CreateVector_Builder(extendBuilder);
            FlatBufferBuilder.PutIn(extendBuilder);

            PRO.Flat.OperatBasiceData.StartOperatBasiceData(builder);
            PRO.Flat.OperatBasiceData.AddType(builder, skillTypeOffset);
            PRO.Flat.OperatBasiceData.AddGuid(builder, skillGuidOffset);
            PRO.Flat.OperatBasiceData.AddNowState(builder, (int)NowState.EnumName);
            PRO.Flat.OperatBasiceData.AddExtendData(builder, extendOffset);
            return PRO.Flat.OperatBasiceData.EndOperatBasiceData(builder);
        }
        public void ToRAM(PRO.Flat.OperatBasiceData diskData)
        {
            SetState((OperateStateEnum)diskData.NowState);

            var builder = FlatBufferBuilder.TakeOut(diskData.ExtendDataLength);
            var datas = builder.DataBuffer.ToSpan(0, diskData.ExtendDataLength);
            for (int i = datas.Length - 1; i >= 0; i--)
                datas[datas.Length - i - 1] = diskData.ExtendData(i);
            ExtendDataToRAM(builder);
            FlatBufferBuilder.PutIn(builder);
        }
        protected virtual void ExtendDataToDisk(FlatBufferBuilder builder) { }
        protected virtual void ExtendDataToRAM(FlatBufferBuilder builder) { }
    }
}
