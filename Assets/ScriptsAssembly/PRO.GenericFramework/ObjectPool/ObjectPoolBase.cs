using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace PRO.Tool
{
    public abstract class ObjectPoolBase<T> where T : class
    {
        /// <summary>
        /// 未使用的对象
        /// </summary>
        private HashSet<T> notUsedObject = new HashSet<T>();
        /// <summary>
        /// 使用中的对象
        /// </summary>
        private HashSet<T> usedObject = new HashSet<T>();

        /// <summary>
        /// 对象池内最大存储对象数量
        /// </summary>
        public int MaxNumber { get; set; }

        /// <summary>
        /// 在取出时是否可以超出最大存储数量
        /// </summary>
        public bool IsCanExceed { get; set; }

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
        /// 初始化对象池
        /// </summary>
        /// <param 对象池内最大对象数量 ="maxNuber"></param>
        public ObjectPoolBase(int maxNuber, bool isCanExceed)
        {
            this.MaxNumber = maxNuber;
            this.IsCanExceed = isCanExceed;

        }

        /// <summary>
        /// 放入对象
        /// </summary>
        /// <param name="item"></param>
        public virtual void PutIn(T item)
        {
            if (item == null) return;

            usedObject.Remove(item);
            PutInEvent?.Invoke(item);
            if (notUsedObject.Count + usedObject.Count < MaxNumber)
            {
                if (notUsedObject.Contains(item) == false)
                    notUsedObject.Add(item);
            }
            else
            {
                Remove(item);
            }
        }


        public void PutIn(T[] items)
        {
            foreach (var item in items)
                PutIn(item);
        }

        /// <summary>
        /// 取出多个对象
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public T[] TakeOut(int num)
        {
            T[] takeOutDatas = new T[num];

            for (int i = 0; i < num; i++)
            {
                takeOutDatas[i] = TakeOut();
            }
            return takeOutDatas;
        }

        /// <summary>
        /// 取出单个对象
        /// </summary>
        /// <returns></returns>
        public virtual T TakeOut()
        {
            T takeOutData = null;
            if (notUsedObject.Count > 0)
            {
                takeOutData = notUsedObject.ElementAt(0);
                notUsedObject.Remove(takeOutData);
                usedObject.Add(takeOutData);
                TakeOutEvent?.Invoke(takeOutData);
            }
            else
            {
                if (usedObject.Count < MaxNumber || IsCanExceed == true)
                {
                    takeOutData = ClonePoolData();
                    usedObject.Add(takeOutData);
                    TakeOutEvent?.Invoke(takeOutData);
                }
                else
                {
                    Debug.Log("不允许从池中取出超过最大限制的对象");
                }
            }
            return takeOutData;
        }

        /// <summary>
        /// 强制取出对象
        /// </summary>
        /// <returns></returns>
        public T ForceTakeOut()
        {
            T takeOutData = null;
            if (notUsedObject.Count > 0)
            {
                takeOutData = notUsedObject.ElementAt(0);
                notUsedObject.Remove(takeOutData);
                TakeOutEvent?.Invoke(takeOutData);
            }
            else
            {
                takeOutData = ClonePoolData();
                usedObject.Add(takeOutData);
                TakeOutEvent?.Invoke(takeOutData);
            }
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

        public abstract void Remove(T item);

        public virtual void Clear()
        {
            usedObject.Clear();
            notUsedObject.Clear();
        }

        protected void CreateEventInvoke(T t)
        {
            CreateEvent(t);
        }
    }
}