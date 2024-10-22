using System.Collections.Generic;
using UnityEngine;
namespace PRO.DataStructure
{
    /// <summary>
    /// 平面直角坐标系
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CrossList<T>
    {

        public int Count { get { return XAxis.Count; } }
        public int AllCount
        {
            get
            {
                int ret = 0;
                for (int i = 0; i < XAxis.Count; i++)
                    ret += XAxis[i].Count;
                return ret;

            }
        }

        private NumberAxis<NumberAxis<T>> XAxis = new NumberAxis<NumberAxis<T>>();

        public NumberAxis<T> this[int x]
        {
            get
            {
                return XAxis[x];
            }
        }
        public T this[Vector2Int pos]
        {
            get
            {
                NumberAxis<T> yAxis = XAxis[pos.x];
                if (yAxis == null) return default(T);
                return yAxis[pos.y];
            }
            set
            {
                if (XAxis[pos.x] == null) XAxis[pos.x] = new NumberAxis<T>();
                XAxis[pos.x][pos.y] = value;
            }
        }
        public T this[Vector2Byte pos]
        {
            get
            {
                NumberAxis<T> yAxis = XAxis[pos.x];
                if (yAxis == null) return default(T);
                return yAxis[pos.y];
            }
            set
            {
                if (XAxis[pos.x] == null) XAxis[pos.x] = new NumberAxis<T>();
                XAxis[pos.x][pos.y] = value;
            }
        }

        /// <summary>
        /// 在索引x处添加一个新的数轴，XAxis.PositiveCount——x中间添加为Null
        /// </summary>
        /// <param name="x"></param>
        private void New(int x)
        {
            if (x >= 0)
            {
                if (x < XAxis.NonNegativeCount && XAxis[x] != null) return;
            }
            else
            {
                if (-x - 1 < XAxis.NegativeCount && XAxis[x] != null) return;
            }
            XAxis[x] = new NumberAxis<T>();
        }

    }


    public class NumberAxis<T>
    {
        private List<T> NonNegative = new List<T>();
        private List<T> Negative = new List<T>();

        /// <summary>
        /// get获取索引处元素
        /// set设置索引处元素，Positive.Count——x处设置为null
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public T this[int x]
        {
            get
            {
                if (x >= 0)
                {
                    if (x >= NonNegative.Count)
                        return default(T);
                    return NonNegative[x];
                }
                else
                {
                    if (-x - 1 >= Negative.Count)
                        return default(T);
                    return Negative[-x - 1];
                }
            }
            set
            {
                if (x >= 0)
                {
                    if (x >= NonNegative.Count)
                        for (int i = NonNegative.Count; i < x + 1; i++)
                            NonNegative.Add(default(T));
                    NonNegative[x] = value;
                }
                else
                {
                    x = -x - 1;
                    if (x >= Negative.Count)
                        for (int i = Negative.Count; i < x + 1; i++)
                            Negative.Add(default(T));
                    Negative[x] = value;
                }
            }
        }

        public int Count { get { return NonNegative.Count + Negative.Count; } }
        public int NonNegativeCount { get { return NonNegative.Count; } }
        public int NegativeCount { get { return Negative.Count; } }

    }
}