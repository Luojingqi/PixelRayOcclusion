using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public struct DictionaryPackage<K, V> : IDictionary<K, V>
    where K : class
    where V : class
{
    public Dictionary<object, object> set;

    public V this[K key]
    {
        get => (V)set[key];
        set => set[key] = value;
    }

    public ICollection<K> Keys => set.Keys.Cast<K>().ToList();

    public ICollection<V> Values => set.Values.Cast<V>().ToList();

    public int Count => set.Count;

    public bool IsReadOnly => false;

    public void Add(K key, V value)
    {
        set.Add(key, value);
    }

    public void Add(KeyValuePair<K, V> item)
    {
        set.Add(item.Key, item.Value);
    }

    public void Clear()
    {
        set.Clear();
    }

    public bool Contains(KeyValuePair<K, V> item)
    {
        return set.TryGetValue(item.Key, out object val)
            && (V)val == item.Value;
    }

    public bool ContainsKey(K key)
    {
        return set.ContainsKey(key);
    }

    public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
    {
        if (array == null) throw new ArgumentNullException(nameof(array));
        if (arrayIndex < 0) throw new ArgumentOutOfRangeException(nameof(arrayIndex));
        if (array.Length - arrayIndex < set.Count) throw new ArgumentException("索引越界");

        foreach (var kvp in set)
        {
            array[arrayIndex++] = new KeyValuePair<K, V>(
                (K)kvp.Key,
                (V)kvp.Value
            );
        }
    }

    public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
    {
        foreach (var kvp in set)
        {
            yield return new KeyValuePair<K, V>(
                (K)kvp.Key,
                (V)kvp.Value
            );
        }
    }

    public bool Remove(K key)
    {
        return set.Remove(key);
    }

    public bool Remove(KeyValuePair<K, V> item)
    {
        if (Contains(item))
        {
            return set.Remove(item.Key);
        }
        return false;
    }

    public bool TryGetValue(K key, out V value)
    {
        if (set.TryGetValue(key, out object objValue))
        {
            value = (V)objValue;
            return true;
        }
        value = default;
        return false;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


    private struct KeyCollectionWrapper : ICollection<K>
    {
        private readonly Dictionary<object, object> _dict;

        public KeyCollectionWrapper(Dictionary<object, object> dict) => _dict = dict;

        public int Count => _dict.Count;
        public bool IsReadOnly => true;

        public IEnumerator<K> GetEnumerator()
        {
            foreach (var key in _dict.Keys)
            {
                yield return (K)key;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(K item) => throw new NotSupportedException();
        public void Clear() => throw new NotSupportedException();
        public bool Contains(K item) => _dict.ContainsKey(item);
        public void CopyTo(K[] array, int arrayIndex)
        {
            var keys = _dict.Keys;
            var i = arrayIndex;
            foreach (var key in keys)
            {
                if (i >= array.Length) break;
                array[i++] = (K)key;
            }
        }
        public bool Remove(K item) => throw new NotSupportedException();
    }

    private struct ValueCollectionWrapper : ICollection<V>
    {
        private readonly Dictionary<object, object> _dict;

        public ValueCollectionWrapper(Dictionary<object, object> dict) => _dict = dict;

        public int Count => _dict.Count;
        public bool IsReadOnly => true;

        public IEnumerator<V> GetEnumerator()
        {
            foreach (var value in _dict.Values)
            {
                yield return (V)value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        
        public void Add(V item) => throw new NotSupportedException();
        public void Clear() => throw new NotSupportedException();
        public bool Contains(V item) => _dict.Values.Contains(item);
        public void CopyTo(V[] array, int arrayIndex)
        {
            var values = _dict.Values;
            var i = arrayIndex;
            foreach (var value in values)
            {
                if (i >= array.Length) break;
                array[i++] = (V)value;
            }
        }
        public bool Remove(V item) => throw new NotSupportedException();
    }
}