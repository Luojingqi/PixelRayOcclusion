using System;
using System.Collections.Generic;
using UnityEngine;
namespace PRO.Tool
{
    public class ObjectPool<T> where T : class, new()
    {
        /// <summary>
        /// 未使用的对象
        /// </summary>
        private List<T> notUsedList = new List<T>();
        /// <summary>
        /// 使用中的对象
        /// </summary>
        private HashSet<T> usedHashSet = new HashSet<T>();

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
        public ObjectPool(int maxNuber, bool isCanExceed)
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

            usedHashSet.Remove(item);

            if (notUsedList.Count + usedHashSet.Count < MaxNumber)
            {
                notUsedList.Add(item);
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
            if (notUsedList.Count > 0)
            {
                takeOutData = notUsedList[notUsedList.Count - 1];
                notUsedList.RemoveAt(notUsedList.Count - 1);
                usedHashSet.Add(takeOutData);
                TakeOutEvent?.Invoke(takeOutData);
            }
            else
            {
                if (IsCanExceed == true)
                {
                    takeOutData = ClonePoolData();
                    usedHashSet.Add(takeOutData);
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
        /// 强制取出对象，一般用于UI中使用对象池，但仅允许存在一个实例的对象池取出
        /// </summary>
        /// <returns></returns>
        public T ForceTakeOut()
        {
            T takeOutData = null;
            if (notUsedList.Count > 0)
            {
                takeOutData = notUsedList[notUsedList.Count - 1];
                notUsedList.RemoveAt(notUsedList.Count - 1);
                TakeOutEvent?.Invoke(takeOutData);
            }
            else
            {
                takeOutData = ClonePoolData();
                usedHashSet.Add(takeOutData);
                TakeOutEvent?.Invoke(takeOutData);
            }
            return takeOutData;
        }


        /// <summary>
        /// 创建一个新对象
        /// </summary>
        /// <returns></returns>
        protected virtual T ClonePoolData()
        {
            T t = new T();
            CreateEvent?.Invoke(t);
            return t;
        }

        /// <summary>
        /// 池内添加数据
        /// </summary>
        /// <param name="num"></param>
        public void PutInCloneData(int num)
        {
            for (int i = 0; i < num; i++)
            {
                PutIn(ClonePoolData());
            }
        }

        public virtual void Remove(T item)
        {

        }

        public virtual void Clear()
        {
            usedHashSet.Clear();
            notUsedList.Clear();
        }

        protected void CreateEventInvoke(T t)
        {
            CreateEvent(t);
        }
    }
}