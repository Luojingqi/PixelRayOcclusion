using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : class, new()
{
    /// <summary>
    /// 对象字典
    /// </summary>
    private Dictionary<int, T> DataDic = new Dictionary<int, T>();

    /// <summary>
    /// 对象字典当前使用状态
    /// </summary>
    private Dictionary<int, bool> DataStateDic = new Dictionary<int, bool>();

    /// <summary>
    /// 对象池内最大对象数量
    /// </summary>
    public int MaxNumber { get; set; }

    /// <summary>
    /// 在取出时是否可以超出
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

        int hash = item.GetHashCode();


        if (DataDic.Count <= MaxNumber)
        {
            if (DataDic.ContainsKey(hash))
            {
                //如果对象在池内
                DataStateDic[hash] = false;
                PutInEvent?.Invoke(item);
            }
            else
            {
                //对象不在池内 

                DataDic.Add(hash, item);
                DataStateDic.Add(hash, false);
                PutInEvent?.Invoke(item);

            }
        }
        else if (DataDic.ContainsKey(hash))
        {
            //超出的部分在池内 
            RemoveData(item);
            return;
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
        foreach (var kv in DataStateDic)
        {
            if (!kv.Value)
            {
                DataDic.TryGetValue(kv.Key, out takeOutData);

                if (takeOutData != null)
                {
                    DataStateDic[kv.Key] = true;
                    TakeOutEvent?.Invoke(takeOutData);
                    break;
                }
            }
        }
        //所有对象均已占用,且可以取出超过最大限制的对象，则暂时为池中添加对象
        if (takeOutData == null)
        {
            if (IsCanExceed == true)
            {
                takeOutData = ClonePoolData();
                int takeOutDataHash = takeOutData.GetHashCode();
                DataDic.Add(takeOutDataHash, takeOutData);
                DataStateDic.Add(takeOutDataHash, true);
                TakeOutEvent?.Invoke(takeOutData);
            }
            else if (DataDic.Count < MaxNumber)
            {
                takeOutData = ClonePoolData();
                int takeOutDataHash = takeOutData.GetHashCode();
                DataDic.Add(takeOutDataHash, takeOutData);
                DataStateDic.Add(takeOutDataHash, true);
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
        foreach (var kv in DataStateDic)
        {
            DataDic.TryGetValue(kv.Key, out takeOutData);
            DataStateDic[kv.Key] = true;
            TakeOutEvent?.Invoke(takeOutData);
            break;
        }
        return takeOutData;
    }


    /// <summary>
    /// 创建一个新对象
    /// </summary>
    /// <returns></returns>
    protected virtual T ClonePoolData()
    {
        T t =new T();
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

    public virtual void RemoveData(T item)
    {
        int itemHash = item.GetHashCode();
        DataDic.Remove(itemHash);
        DataStateDic.Remove(itemHash);
    }

    public virtual void Clear()
    {
        foreach (var data in DataDic)
        {
            RemoveData(data.Value);
        }
    }

    protected void CreateEventInvoke(T t)
    {
        CreateEvent(t);
    }
}
