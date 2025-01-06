using System;
using System.Collections.Generic;
namespace PRO.Tool
{
    public abstract class ObjectPoolBase<T> where T : class
    {
        /// <summary>
        /// 未使用的对象
        /// </summary>
        public Queue<T> notUsedObject = new Queue<T>();

        /// <summary>
        /// 创建对象事件
        /// </summary>
        public event Action<T> CreateEvent;
        /// <summary>
        /// 放入之后事件
        /// </summary>
        public event Action<T> PutInEvent;
        /// <summary>
        /// 取出之后事件
        /// </summary>
        public event Action<T> TakeOutEvent;


        /// <summary>
        /// 放入对象
        /// </summary>
        /// <param name="item"></param>
        public virtual void PutIn(T item)
        {
            if (item == null) return;
            PutInEvent?.Invoke(item);
            notUsedObject.Enqueue(item);
        }


        public void PutIn(T[] items)
        {
            foreach (var item in items)
                PutIn(item);
        }

        /// <summary>
        /// 取出单个对象
        /// </summary>
        /// <returns></returns>
        public virtual T TakeOut()
        {
            T takeOutData = null;
            if (notUsedObject.Count > 0)
                takeOutData = notUsedObject.Dequeue();
            else
                takeOutData = ClonePoolData();
            TakeOutEvent?.Invoke(takeOutData);
            return takeOutData;
        }



        /// <summary>
        /// 创建一个新对象
        /// </summary>
        /// <returns></returns>
        private T ClonePoolData()
        {
            T t = NewObject();
            CreateEvent?.Invoke(t);
            return t;
        }

        protected abstract T NewObject();


        public virtual void Clear()
        {
            notUsedObject.Clear();
        }

        protected void CreateEventInvoke(T t)
        {
            CreateEvent(t);
        }
    }
}