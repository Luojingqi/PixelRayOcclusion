using Google.FlatBuffers;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PRO.EventData
{
    public class EventData_Int32 : EventData<int>
    {
        public EventData_Int32(RoleInfo roleInfo) : base(roleInfo) { }
        protected override int Add(int value0, int value1) => value0 + value1;
        protected override int Subtract(int value0, int value1) => value0 - value1;
        protected override int Multiply(int value0, int value1) => value0 * value1;
        protected override int MultiplyValue(int value0, double value1) => (int)(value0 * value1);
        protected override int Multiplication_Double(int value0, double value1) => (int)(value0 * value1);
        public Offset<Flat.EventData_Int32Data> ToDisk(FlatBufferBuilder builder) => default;
        // Flat.EventData_Int32Data.CreateEventData_Int32Data(builder, valueSum_基础, valueSum_百分比, valueSum_额外, valueSum);

        public void ToRAM(Flat.EventData_Int32Data diskData)
        {
            //valueSum_基础 = diskData.Value0;
            //valueSum_百分比 = diskData.Value1;
            //valueSum_额外 = diskData.Value2;
            //valueSum = diskData.Value3;
        }
    }
    public class EventData_Double : EventData<double>
    {
        public EventData_Double(RoleInfo roleInfo) : base(roleInfo) { }
        protected override double Add(double value0, double value1) => value0 + value1;
        protected override double Subtract(double value0, double value1) => value0 - value1;
        protected override double Multiply(double value0, double value1) => value0 * value1;
        protected override double MultiplyValue(double value0, double value1) => value0 * value1;
        protected override double Multiplication_Double(double value0, double value1) => value0 * value1;
        public Offset<Flat.EventData_DoubleData> ToDisk(FlatBufferBuilder builder) => default;
        //Flat.EventData_DoubleData.CreateEventData_DoubleData(builder, valueSum_基础, valueSum_额外, valueSum_百分比, valueSum);
        public void ToRAM(Flat.EventData_DoubleData diskData)
        {
            //valueSum_基础 = diskData.Value0;
            //valueSum_百分比 = diskData.Value1;
            //valueSum_额外 = diskData.Value2;
            //valueSum = diskData.Value3;
        }
    }

    /// <summary>
    /// 枚举内的ID会被初始添加
    /// </summary>
    public enum DefaultValueID : long
    {
        默认 = 0,
        溢出修正 = -1,
    }
    public static class EventDataEx
    {
        public static long[] InitValueArray;
        static EventDataEx()
        {
            InitValueArray = Enum.GetValues(typeof(DefaultValueID)) as long[];
        }
    }
    [Flags]
    public enum ChangeValueType
    {
        基础 = 1 << 0,
        百分比 = 1 << 1,
        额外 = 1 << 2,
    }
    public enum EventClear
    {
        Clear,
        UnClear,
    }

    public abstract partial class EventData<T> where T : struct
    {
        public struct Data
        {
            public T Value_基础;
            public double Value_百分比;
            public T Value_额外;
            public readonly T Value;
            public Data(T value_基础, double value_百分比, T value_额外, T value)
            {
                this.Value_基础 = value_基础;
                this.Value_百分比 = value_百分比;
                this.Value_额外 = value_额外;
                this.Value = value;
            }
            public Data(EventData<T> data) : this(data.valueSum_基础, data.valueSum_百分比, data.valueSum_额外, data.valueSum) { }
        }

        #region 值
        protected Dictionary<long, Value_基础> value_基础_Dic = new();
        protected Dictionary<long, Value_百分比> value_百分比_Dic = new();
        protected Dictionary<long, Value_额外> value_额外_Dic = new();
        protected T valueSum;
        public T ValueSum => valueSum;

        private T valueSum_基础;
        public T ValueSum_基础 => valueSum_基础;
        private double valueSum_百分比;
        public double ValueSum_百分比 => valueSum_百分比;
        private T valueSum_额外;
        public T ValueSum_额外 => valueSum_额外;
        #endregion
        #region 获取Value
        protected static Dictionary<Type, Func<EventData<T>, long, ValueBase>> getValueFuncDic = new(3);
        static EventData()
        {
            getValueFuncDic.Add(typeof(Value_基础), (eventData, id) => { eventData.value_基础_Dic.TryGetValue(id, out var value); return value; });
            getValueFuncDic.Add(typeof(Value_百分比), (eventData, id) => { eventData.value_百分比_Dic.TryGetValue(id, out var value); return value; });
            getValueFuncDic.Add(typeof(Value_额外), (eventData, id) => { eventData.value_额外_Dic.TryGetValue(id, out var value); return value; });
        }
        private static Type value_基础_type = typeof(Value_基础);
        private static Type value_百分比_type = typeof(Value_百分比);
        private static Type value_额外_type = typeof(Value_额外);
        public TValue GetValue<TValue>(long id) where TValue : ValueBase => getValueFuncDic[typeof(TValue)].Invoke(this, id) as TValue;
        public ValueBase GetValue(ChangeValueType type, long id)
        {
            switch (type)
            {
                case ChangeValueType.基础: return value_基础_Dic.GetValueOrDefault(id);
                case ChangeValueType.百分比: return value_百分比_Dic.GetValueOrDefault(id);
                case ChangeValueType.额外: return value_额外_Dic.GetValueOrDefault(id);
            }
            return null;
        }
        public Value_基础 GetValue_基础(long id) => value_基础_Dic.GetValueOrDefault(id);
        public Value_百分比 GetValue_百分比(long id) => value_百分比_Dic.GetValueOrDefault(id);
        public Value_额外 GetValue_额外(long id) => value_额外_Dic.GetValueOrDefault(id);
        #endregion

        private Data CalculateValueSum(ChangeValueType changeValue)
        {
            T valueSum_基础 = default;
            if ((changeValue & ChangeValueType.基础) == ChangeValueType.基础)
                foreach (var data in value_基础_Dic.Values)
                    valueSum_基础 = Add(valueSum_基础, data.Value);
            else valueSum_基础 = this.valueSum_基础;

            double valueSum_百分比 = default;
            if ((changeValue & ChangeValueType.百分比) == ChangeValueType.百分比)
                foreach (var data in value_百分比_Dic.Values)
                    valueSum_百分比 += data.Value;
            else valueSum_百分比 = this.valueSum_百分比;

            T valueSum_额外 = default;
            if ((changeValue & ChangeValueType.额外) == ChangeValueType.额外)
                foreach (var data in value_额外_Dic.Values)
                    valueSum_额外 = Add(valueSum_额外, data.Value);
            else valueSum_额外 = this.valueSum_额外;
            T valueSum = Multiplication_Double(Add(valueSum_基础, valueSum_额外), valueSum_百分比 + 1.0);
            return new Data(valueSum_基础, valueSum_百分比, valueSum_额外, valueSum);
        }

        #region 事件
        private enum ChangeEnumItem
        {
            Set,
            Add,
            Remove,
            RemovePutIn,
        }
        private struct ChangeQueueItem
        {
            public ValueBase value;
            public ChangeEnumItem changeEnum;
            public ChangeQueueItem(ValueBase value, ChangeEnumItem changeEnum)
            {
                this.value = value;
                this.changeEnum = changeEnum;
            }
        }
        private Queue<ChangeQueueItem> changeQueue = new();
        private List<ValueChangeEventData> eventList = new List<ValueChangeEventData>(2);
        private bool isInvokeEvent = false;
        private struct ValueChangeEventData
        {
            public EventClear eventType;
            public ValueChangeEvent action;
            public ValueChangeEventData(EventClear eventType, ValueChangeEvent action)
            {
                this.eventType = eventType;
                this.action = action;
            }
        }
        public void AddValueChangeEvent(EventClear eventType, ValueChangeEvent action) => eventList.Add(new(eventType, action));
        public delegate void ValueChangeEvent(RoleInfo info, ValueBase changeValue, Data oldData, Data newData);
        private void InvokeValueChangeEvent(ValueBase value)
        {
            isInvokeEvent = true;
            Data oldData = new(this);
            Data newData = CalculateValueSum(value.ValueType);
            for (int i = 0; i < eventList.Count; i++)
                eventList[i].action.Invoke(Info, value, oldData, newData);

            this.valueSum_基础 = newData.Value_基础;
            this.valueSum_百分比 = newData.Value_百分比;
            this.valueSum_额外 = newData.Value_额外;
            this.valueSum = newData.Value;
            isInvokeEvent = false;
            if (changeQueue.Count > 0)
            {
                var item = changeQueue.Dequeue();
                switch (item.changeEnum)
                {
                    case ChangeEnumItem.Set:
                        var cloneValue = item.value;
                        switch (cloneValue.ValueType)
                        {
                            case ChangeValueType.基础: GetValue_基础(cloneValue.ID).Value = (cloneValue as Value_基础).Value; break;
                            case ChangeValueType.百分比: GetValue_百分比(cloneValue.ID).Value = (cloneValue as Value_百分比).Value; break;
                            case ChangeValueType.额外: GetValue_额外(cloneValue.ID).Value = (cloneValue as Value_额外).Value; break;
                        }
                        ValueBase.PutInRandomID(cloneValue);
                        break;
                    case ChangeEnumItem.Add:
                        item.value.AddTo(this); break;
                    case ChangeEnumItem.Remove:
                        item.value.RemoveTo(); break;
                }
            }
        }

        #endregion

        [HideInInspector]
        public readonly RoleInfo Info;
        public EventData(RoleInfo roleInfo)
        {
            Info = roleInfo;
            for (int i = 0; i < EventDataEx.InitValueArray.Length; i++)
            {
                long id = EventDataEx.InitValueArray[i];
                value_基础_Dic.Add(id, Value_基础.TakeOut(default, id));
                value_百分比_Dic.Add(id, Value_百分比.TakeOut(default, id));
                value_额外_Dic.Add(id, Value_额外.TakeOut(default, id));
            }
        }
        /// <summary>
        /// 清除所有值，不会值修改触发事件
        /// </summary>
        public void ClearValue()
        {
            valueSum_基础 = default;
            valueSum_额外 = default;
            valueSum_百分比 = default;
            valueSum = default;
            foreach (var value in value_基础_Dic.Values)
                Value_基础.PutIn(value);
            foreach (var value in value_百分比_Dic.Values)
                Value_百分比.PutIn(value);
            foreach (var value in value_额外_Dic.Values)
                Value_额外.PutIn(value);
            value_基础_Dic.Clear();
            value_百分比_Dic.Clear();
            value_额外_Dic.Clear();
        }
        /// <summary>
        /// 清除所有被标记为清理的事件
        /// </summary>
        public void ClearEvent()
        {
            for (int i = eventList.Count - 1; i >= 0; i--)
                if (eventList[i].eventType == EventClear.Clear)
                    eventList.RemoveAt(i);
        }
        /// <summary>
        /// 清理所有值与事件
        /// </summary>
        public void ClearAll()
        {
            ClearValue();
            eventList.Clear();
        }
        public static void CloneValue(EventData<T> from, EventData<T> to)
        {
            to.ClearValue();
            foreach (var value in from.value_基础_Dic.Values)
            {
                var valueClone = Value_基础.TakeOut(value.Value);
                to.value_基础_Dic.Add(valueClone.ID, valueClone);
            }
            foreach (var value in from.value_百分比_Dic.Values)
            {
                var valueClone = Value_百分比.TakeOut(value.Value);
                to.value_百分比_Dic.Add(valueClone.ID, valueClone);
            }
            foreach (var value in from.value_额外_Dic.Values)
            {
                var valueClone = Value_额外.TakeOut(value.Value);
                to.value_额外_Dic.Add(valueClone.ID, valueClone);
            }
            to.valueSum_基础 = from.valueSum_基础;
            to.valueSum_额外 = from.valueSum_额外;
            to.valueSum_百分比 = from.valueSum_百分比;
            to.valueSum = from.valueSum;
        }

        //public void Add(EventData<T> eventData)
        //{
        //    Value_基础 = Add(valueSum_基础, eventData.valueSum_基础);
        //    Value_百分比 += eventData.valueSum_百分比;
        //    Value_额外 = Add(valueSum_额外, eventData.valueSum_额外);
        //}
        //public void Subtract(EventData<T> eventData)
        //{
        //    Value_基础 = Subtract(valueSum_基础, eventData.valueSum_基础);
        //    Value_百分比 -= eventData.valueSum_百分比;
        //    Value_额外 = Subtract(valueSum_额外, eventData.valueSum_额外);
        //}
        //public void Multiply(EventData<T> eventData)
        //{
        //    Value_基础 = Multiply(valueSum_基础, eventData.valueSum_基础);
        //    Value_百分比 *= eventData.valueSum_百分比;
        //    Value_额外 = Multiply(valueSum_额外, eventData.valueSum_额外);
        //}
        //public void Multiply(double value)
        //{
        //    Value_基础 = MultiplyValue(valueSum_基础, value);
        //    Value_百分比 *= value;
        //    Value_额外 = MultiplyValue(valueSum_额外, value);
        //}
        protected abstract T Add(T value0, T value1);
        protected abstract T Subtract(T value0, T value1);
        protected abstract T Multiply(T value0, T value1);
        protected abstract T MultiplyValue(T value0, double value1);
        protected abstract T Multiplication_Double(T value0, double value1);
    }
}
