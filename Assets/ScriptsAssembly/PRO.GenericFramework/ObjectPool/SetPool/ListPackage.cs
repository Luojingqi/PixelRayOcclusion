using System.Collections;
using System.Collections.Generic;

namespace PRO.Tool
{
    public struct ListPackage<T> : IList<T> where T : class
    {
        public List<object> set;

        public int Count => set.Count;

        public bool IsReadOnly => false;

        public T this[int index]
        {
            get => set[index] as T;
            set => set[index] = value;
        }

        public void Add(T item)
        {
            set.Add(item);
        }

        public bool Remove(T item)
        {
            return set.Remove(item);
        }

        bool ICollection<T>.Remove(T item)
        {
            return set.Remove(item);
        }

        public void RemoveAt(int index)
        {
            set.RemoveAt(index);
        }

        public bool Contains(T item)
        {
            return set.Contains(item);
        }

        public void Clear()
        {
            set.Clear();
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var item in set)
            {
                yield return item as T;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return set.GetEnumerator();
        }

        public int IndexOf(T item)
        {
            return set.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            set.Insert(index, item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            set.CopyTo(array, arrayIndex);
        }
    }
}
