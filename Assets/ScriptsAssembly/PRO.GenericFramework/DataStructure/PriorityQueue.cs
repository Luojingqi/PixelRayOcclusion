using System;
using System.Collections;
using System.Collections.Generic;

namespace PRO.DataStructure
{
    /// <summary>
    /// 优先队列，优先取出代价小的
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PriorityQueue<T> : IEnumerable<T>
    {
        private List<T> elements;
        private List<float> costs;

        public PriorityQueue() { elements = new List<T>(); costs = new List<float>(); }
        public PriorityQueue(int count) { elements = new List<T>(count); costs = new List<float>(count); }

        public int Count
        {
            get { return elements.Count; }
        }
        public T this[int index] => elements[index];

        public void Enqueue(T item, float cost)
        {
            elements.Add(item);
            costs.Add(cost);
            int n = elements.Count - 1;
            while (n > 0)
            {
                int p = (n - 1) / 2;
                if (costs[n] >= costs[p])
                    break;

                Exchange(p, n);
                n = p;
            }
        }

        public T Dequeue()
        {
            if (elements.Count == 0)
                throw new InvalidOperationException("The shipNodeQueue is empty.");

            T ret = elements[0];
            int last = elements.Count - 1;
            T lastItem = elements[last];
            float lastCost = costs[last];
            elements.RemoveAt(last);
            costs.RemoveAt(last);

            if (last > 0)
            {
                elements[0] = lastItem;
                int parentIndex = 0;

                while (true)
                {
                    int l = 2 * parentIndex + 1;
                    if (l >= last)
                        break;
                    int r = l + 1;

                    //找出左右两个哪个优先级更高
                    int c = (r >= last || costs[l] < costs[r]) ? l : r;

                    if (lastCost <= costs[c])
                        break;

                    elements[parentIndex] = elements[c];
                    costs[parentIndex] = costs[c];
                    parentIndex = c;
                }
                elements[parentIndex] = lastItem;
                costs[parentIndex] = lastCost;
            }

            return ret;
        }

        public void Exchange(int a, int b)
        {
            T temp = elements[a];
            elements[a] = elements[b];
            elements[b] = temp;
            costs[a] += costs[b];
            costs[b] = costs[a] - costs[b];
            costs[a] -= costs[b];
        }

        public T Peek()
        {
            if (elements.Count == 0)
                throw new InvalidOperationException("The shipNodeQueue is empty.");
            return elements[0];
        }
        public bool Contains(T data)
        {
            return elements.Contains(data);
        }

        public void Clear()
        {
            elements.Clear();
            costs.Clear();
        }

        IEnumerator<T> GetEnumerator()
        {
            return elements.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator();
        }

    }
}