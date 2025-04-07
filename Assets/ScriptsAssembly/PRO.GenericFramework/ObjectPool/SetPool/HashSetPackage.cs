using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PRO.Tool
{
    public struct HashSetPackage<T> : ISet<T> where T : class
    {
        public HashSet<object> set;

        public int Count => set.Count;

        public bool IsReadOnly => false;

        public bool Add(T item) => set.Add(item);

        public void Clear() => set.Clear();

        public bool Contains(T item) => set.Contains(item);

        public void CopyTo(T[] array, int arrayIndex)
        {
            int i = arrayIndex;
            foreach (var item in set)
            {
                array[i++] = (T)item;
            }
        }

        public void ExceptWith(IEnumerable<T> other) =>
            set.ExceptWith(other.Cast<object>());

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var item in set)
            {
                yield return (T)item;
            }
        }

        public void IntersectWith(IEnumerable<T> other) =>
            set.IntersectWith(other.Cast<object>());

        public bool IsProperSubsetOf(IEnumerable<T> other) =>
            set.IsProperSubsetOf(other.Cast<object>());

        public bool IsProperSupersetOf(IEnumerable<T> other) =>
            set.IsProperSupersetOf(other.Cast<object>());

        public bool IsSubsetOf(IEnumerable<T> other) =>
            set.IsSubsetOf(other.Cast<object>());

        public bool IsSupersetOf(IEnumerable<T> other) =>
            set.IsSupersetOf(other.Cast<object>());

        public bool Overlaps(IEnumerable<T> other) =>
            set.Overlaps(other.Cast<object>());

        public bool Remove(T item) => set.Remove(item);

        public bool SetEquals(IEnumerable<T> other) =>
            set.SetEquals(other.Cast<object>());

        public void SymmetricExceptWith(IEnumerable<T> other) =>
            set.SymmetricExceptWith(other.Cast<object>());

        public void UnionWith(IEnumerable<T> other) =>
            set.UnionWith(other.Cast<object>());

        void ICollection<T>.Add(T item) => Add(item);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}