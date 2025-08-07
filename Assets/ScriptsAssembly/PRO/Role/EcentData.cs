using Google.FlatBuffers;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PRO
{
    public class EventData_Int32 : EventData<int>
    {
        public EventData_Int32(RoleInfo roleInfo) : base(roleInfo) { }
        protected override int Add(int value0, int value1) => value0 + value1;
        protected override int Multiplication_Double(int value0, double value1) => (int)(value0 * value1);
        public Offset<Flat.EventData_Int32Data> ToDisk(FlatBufferBuilder builder) =>
             Flat.EventData_Int32Data.CreateEventData_Int32Data(builder, value_基础, value_百分比, value_额外, value);

        public void ToRAM(Flat.EventData_Int32Data diskData)
        {
            value_基础 = diskData.Value0;
            value_百分比 = diskData.Value1;
            value_额外 = diskData.Value2;
            value = diskData.Value3;
        }
    }
    public class EventData_Double : EventData<double>
    {
        public EventData_Double(RoleInfo roleInfo) : base(roleInfo) { }
        protected override double Add(double value0, double value1) => value0 + value1;
        protected override double Multiplication_Double(double value0, double value1) => value0 * value1;
        public Offset<Flat.EventData_DoubleData> ToDisk(FlatBufferBuilder builder) =>
            Flat.EventData_DoubleData.CreateEventData_DoubleData(builder, value_基础, value_额外, value_百分比, value);
        public void ToRAM(Flat.EventData_DoubleData diskData)
        {
            value_基础 = diskData.Value0;
            value_百分比 = diskData.Value1;
            value_额外 = diskData.Value2;
            value = diskData.Value3;
        }
    }

    public abstract class EventData<T> where T : struct
    {
        public enum ChangeValueType
        {
            基础,
            百分比,
            额外,
        }
        public struct Data
        {
            public T value_基础;
            public double value_百分比;
            public T value_额外;
            public readonly T value;
            public Data(T value_基础, double value_百分比, T value_额外, T value)
            {
                this.value_基础 = value_基础;
                this.value_百分比 = value_百分比;
                this.value_额外 = value_额外;
                this.value = value;
            }
            public Data(EventData<T> data) : this(data.value_基础, data.value_百分比, data.value_额外, data.value) { }
        }
        protected T value_基础;
        protected double value_百分比;
        protected T value_额外;
        protected T value;

        [ShowInInspector]
        public T Value_基础
        {
            get => value_基础;
            set
            {
                var oldData = new Data(this);
                value_基础 = value;
                UpdateValue();
                var nowData = new Data(this);
                InvokeValueChangeEvent(ChangeValueType.基础, nowData, oldData);
            }
        }
        [ShowInInspector]
        public double Value_百分比
        {
            get => value_百分比;
            set
            {
                var oldData = new Data(this);
                value_百分比 = value;
                UpdateValue();
                var nowData = new Data(this);
                InvokeValueChangeEvent(ChangeValueType.百分比, nowData, oldData);
            }
        }
        [ShowInInspector]
        public T Value_额外
        {
            get => value_额外;
            set
            {
                var oldData = new Data(this);
                value_额外 = value;
                UpdateValue();
                var nowData = new Data(this);
                InvokeValueChangeEvent(ChangeValueType.额外, nowData, oldData);
            }
        }
        [ShowInInspector]
        public T Value => value;

        #region 事件
        private List<ValueChangeEvent> eventList = new List<ValueChangeEvent>(2);

        public enum EventType
        {
            Clear,
            UnClear
        }
        private struct ValueChangeEvent
        {
            public EventType eventType;
            public Func<RoleInfo, ChangeValueType, Data, Data, Data> func;
            public ValueChangeEvent(EventType eventType, Func<RoleInfo, ChangeValueType, Data, Data, Data> func)
            {
                this.eventType = eventType;
                this.func = func;
            }
        }
        public void AddValueChangeEvent(EventType eventType, Func<RoleInfo, ChangeValueType, Data, Data, Data> func) => eventList.Add(new ValueChangeEvent(eventType, func));

        public void InvokeValueChangeEvent(ChangeValueType type, Data nowData, Data oldData)
        {
            for (int i = 0; i < eventList.Count; i++)
            {
                nowData = eventList[i].func.Invoke(Info, type, nowData, oldData);
                value_基础 = nowData.value_基础;
                value_百分比 = nowData.value_百分比;
                value_额外 = nowData.value_额外;
                UpdateValue();
            }
        }
        #endregion
        [HideInInspector]
        public readonly RoleInfo Info;
        public EventData(RoleInfo roleInfo)
        {
            Info = roleInfo;
        }
        public void Clear()
        {
            value_基础 = default;
            value_额外 = default;
            value_百分比 = default;
            value = default;
            ClearEvent();
        }
        public void ClearEvent()
        {
            for (int i = eventList.Count - 1; i >= 0; i--)
                if (eventList[i].eventType == EventType.Clear)
                    eventList.RemoveAt(i);
        }
        public void ClearAll()
        {
            value_基础 = default;
            value_额外 = default;
            value_百分比 = default;
            value = default;
            eventList.Clear();
        }
        public static void CloneValue(EventData<T> from, EventData<T> to)
        {
            to.value_基础 = from.value_基础;
            to.value_额外 = from.value_额外;
            to.value_百分比 = from.value_百分比;
            to.value = from.value;
        }
        public void UpdateValue()
        {
            value = Multiplication_Double(Add(value_基础, value_额外), value_百分比 + 1.0);
        }

        protected abstract T Add(T value0, T value1);
        protected abstract T Multiplication_Double(T value0, double value1);
    }
}
