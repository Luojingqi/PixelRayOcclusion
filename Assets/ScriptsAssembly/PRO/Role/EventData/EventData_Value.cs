using System;
using System.Collections.Generic;

namespace PRO.EventData
{
    public partial class EventData<T>
    {
        public abstract class ValueBase
        {
            public EventData<T> EventData { get; protected set; }
            public long ID { get; protected set; }
            public abstract ChangeValueType ValueType { get; }
            protected ValueBase() { ID = RandomID(); }
            protected static long RandomID() => ((long)UnityEngine.Random.Range(int.MinValue, int.MaxValue) << 32) | (long)UnityEngine.Random.Range(int.MinValue, int.MaxValue);

            protected static Dictionary<ChangeValueType, Action<ValueBase>> putInActionDic = new(3);
            /// <summary>
            /// 放入并重置ID
            /// </summary>
            /// <param name="value"></param>
            public static void PutInRandomID(ValueBase value) { value.ID = RandomID(); putInActionDic[value.ValueType].Invoke(value); }

            public virtual void AddTo(EventData<T> eventData)
            {
                if (this.EventData != null)
                    throw new Exception("禁止将单个值重复添加");
            }
            /// <summary>
            /// 移除后自动放入池中
            /// </summary>
            public virtual void RemoveTo()
            {
                if (this.EventData == null)
                    throw new Exception("当前值未被添加");
            }
        }

        public class Value_基础 : ValueBase
        {
            #region 对象池
            private static Queue<Value_基础> pool = new();
            public static Value_基础 TakeOut(T value, long id) { var ret = TakeOut(value); ret.ID = id; return ret; }
            public static Value_基础 TakeOut(T value)
            {
                Value_基础 ret = null;
                if (pool.Count > 0)
                    ret = pool.Dequeue();
                else
                    ret = new();
                ret.value = value;
                return ret;
            }
            public static void PutIn(Value_基础 value)
            {
                if (value.EventData != null)
                {
                    new Exception("值未被移除就被放入了池中");
                    return;
                }
                value.value = default;
                pool.Enqueue(value);
            }
            static Value_基础()
            {
                putInActionDic.Add(ChangeValueType.基础, (value) => PutIn(value as Value_基础));
            }
            #endregion
            private Value_基础() { }
            public override ChangeValueType ValueType => ChangeValueType.基础;
            private T value;
            public T Value
            {
                get => value;
                set
                {
                    if (EventData.isInvokeEvent == false)
                    {
                        this.value = value;
                        EventData.InvokeValueChangeEvent(this);
                    }
                    else
                    {
                        var cloneValue = TakeOut(value);
                        cloneValue.ID = ID;
                        EventData.changeQueue.Enqueue(new(cloneValue, ChangeEnumItem.Set));
                    }
                }
            }

            public override void AddTo(EventData<T> eventData)
            {
                base.AddTo(eventData);
                if (eventData.isInvokeEvent == false)
                {
                    eventData.value_基础_Dic.Add(ID, this);
                    EventData = eventData;
                    EventData.InvokeValueChangeEvent(this);
                }
                else
                    eventData.changeQueue.Enqueue(new(this, ChangeEnumItem.Add));
            }
            public override void RemoveTo()
            {
                base.RemoveTo();
                if (EventData.isInvokeEvent == false)
                {
                    EventData.value_基础_Dic.Remove(ID);
                    EventData = null;
                    EventData.InvokeValueChangeEvent(this);
                    pool.Enqueue(this);
                }
                else
                    EventData.changeQueue.Enqueue(new(this, ChangeEnumItem.Remove));
            }
        }

        public class Value_百分比 : ValueBase
        {
            #region 对象池
            private static Queue<Value_百分比> pool = new();
            public static Value_百分比 TakeOut(double value, long id) { var ret = TakeOut(value); ret.ID = id; return ret; }
            public static Value_百分比 TakeOut(double value)
            {
                Value_百分比 ret = null;
                if (pool.Count > 0)
                    ret = pool.Dequeue();
                else
                    ret = new();
                ret.value = value;
                return ret;
            }
            public static void PutIn(Value_百分比 value)
            {
                if (value.EventData != null)
                {
                    new Exception("值未被移除就被放入了池中");
                    return;
                }
                value.value = default;
                pool.Enqueue(value);
            }
            static Value_百分比()
            {
                putInActionDic.Add(ChangeValueType.百分比, (value) => PutIn(value as Value_百分比));
            }
            #endregion
            private Value_百分比() { }
            public override ChangeValueType ValueType => ChangeValueType.百分比;
            private double value;
            public double Value
            {
                get => value;
                set
                {
                    if (EventData.isInvokeEvent == false)
                    {
                        this.value = value;
                        EventData.InvokeValueChangeEvent(this);
                    }
                    else
                    {
                        var cloneValue = TakeOut(value);
                        cloneValue.ID = ID;
                        EventData.changeQueue.Enqueue(new(cloneValue, ChangeEnumItem.Set));
                    }
                }
            }
            public override void AddTo(EventData<T> eventData)
            {
                base.AddTo(eventData);
                if (eventData.isInvokeEvent == false)
                {
                    eventData.value_百分比_Dic.Add(ID, this);
                    EventData = eventData;
                    EventData.InvokeValueChangeEvent(this);
                }
                else
                    eventData.changeQueue.Enqueue(new(this, ChangeEnumItem.Add));
            }
            public override void RemoveTo()
            {
                base.RemoveTo();
                if (EventData.isInvokeEvent == false)
                {
                    EventData.value_百分比_Dic.Remove(ID);
                    EventData = null;
                    EventData.InvokeValueChangeEvent(this);
                    pool.Enqueue(this);
                }
                else
                    EventData.changeQueue.Enqueue(new(this, ChangeEnumItem.Remove));
            }
        }

        public class Value_额外 : ValueBase
        {
            #region 对象池
            private static Queue<Value_额外> pool = new();
            public static Value_额外 TakeOut(T value, long id) { var ret = TakeOut(value); ret.ID = id; return ret; }
            public static Value_额外 TakeOut(T value)
            {
                Value_额外 ret = null;
                if (pool.Count > 0)
                    ret = pool.Dequeue();
                else
                    ret = new();
                ret.value = value;
                return ret;
            }
            public static void PutIn(Value_额外 value)
            {
                if (value.EventData != null)
                {
                    new Exception("值未被移除就被放入了池中");
                    return;
                }
                value.value = default;
                pool.Enqueue(value);
            }
            static Value_额外()
            {
                putInActionDic.Add(ChangeValueType.额外, (value) => PutIn(value as Value_额外));
            }
            #endregion
            private Value_额外() { }
            public override ChangeValueType ValueType => ChangeValueType.额外;
            private T value;
            public T Value
            {
                get => value;
                set
                {
                    if (EventData.isInvokeEvent == false)
                    {
                        this.value = value;
                        EventData.InvokeValueChangeEvent(this);
                    }
                    else
                    {
                        var cloneValue = TakeOut(value);
                        cloneValue.ID = ID;
                        EventData.changeQueue.Enqueue(new(cloneValue, ChangeEnumItem.Set));
                    }
                }
            }
            public override void AddTo(EventData<T> eventData)
            {
                base.AddTo(eventData);
                if (eventData.isInvokeEvent == false)
                {
                    eventData.value_额外_Dic.Add(ID, this);
                    EventData = eventData;
                    EventData.InvokeValueChangeEvent(this);
                }
                else
                    eventData.changeQueue.Enqueue(new(this, ChangeEnumItem.Add));
            }
            public override void RemoveTo()
            {
                base.RemoveTo();
                if (EventData.isInvokeEvent == false)
                {
                    EventData.value_额外_Dic.Remove(ID);
                    EventData = null;
                    EventData.InvokeValueChangeEvent(this);
                    pool.Enqueue(this);
                }
                else
                    EventData.changeQueue.Enqueue(new(this, ChangeEnumItem.Remove));
            }
        }
    }
}
