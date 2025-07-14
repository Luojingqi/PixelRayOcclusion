using System.Buffers;
using System;
namespace PRO.Tool
{
    public struct ReusableList<T> : IDisposable
    {
        private T[] array;
        /// <summary>
        /// 返回数组最大长度，-1代表没有数组实例
        /// </summary>
        public int Length => array != null ? array.Length : -1;
        private int count;
        public int Count => count;
        public T this[int index] { get => array[index]; }
        public void Add(T value)
        {
            if (count < array.Length)
                array[count++] = value;
            else
            {
                T[] arrayNew = ArrayPool<T>.Shared.Rent(array.Length << 1);
                Array.Copy(array, arrayNew, count);
                Array.Clear(array, 0, count);
                ArrayPool<T>.Shared.Return(array);
                array = arrayNew;
                Add(value);
            }
        }

        public void Dispose()
        {
            Array.Clear(array, 0, count);
            ArrayPool<T>.Shared.Return(array);
            array = null;
        }

        public ReusableList(int size)
        {
            array = ArrayPool<T>.Shared.Rent(size);
            count = 0;
        }
    }
}