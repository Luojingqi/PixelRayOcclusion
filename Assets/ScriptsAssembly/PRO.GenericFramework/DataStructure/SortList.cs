using System.Collections;
using System.Collections.Generic;

namespace PRO.Tool
{
    /// <summary>
    /// 排序list
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SortList<T> : IEnumerable<HashSet<T>>
    {
        public SortList(int size)
        {
            costList = new List<int>(size);
            valueList = new List<HashSet<T>>(size);
        }

        private readonly List<int> costList;
        private readonly List<HashSet<T>> valueList;

        public void Add(T value, int cost)
        {
            // 使用二分查找确定插入位置
            int index = costList.BinarySearch(cost);
            if (index < 0)
            {
                //未找到时返回大于目标值的下一个值的补码
                // 计算实际插入位置（取补码）
                index = ~index;
                valueList.Insert(index, new HashSet<T>());
                costList.Insert(index, cost);
            }
            valueList[index].Add(value);
            allCount++;
        }
        public bool Remove(T value, int cost)
        {
            int index = costList.BinarySearch(cost);
            if (index < 0) return false;
            var set = valueList[index];
            bool ret = set.Remove(value);
            if (set.Count == 0)
                costList.RemoveAt(index);
            allCount--;
            return ret;
        }
        public int AllCount => allCount;
        private int allCount;
        public int Count => valueList.Count;

        public HashSet<T> FormIndex(int index) => valueList[index];
        public HashSet<T> FormCost(int cost)
        {
            int index = costList.BinarySearch(cost);
            if (index < 0) return null;
            return valueList[index];
        }

        public IEnumerator<HashSet<T>> GetEnumerator() => valueList.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    }
}
