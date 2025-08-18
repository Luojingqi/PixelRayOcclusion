using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Google.FlatBuffers
{
    public partial class FlatBufferBuilder
    {
        /// <summary>
        /// 创建指针数组的指针
        /// </summary>
        /// <param name="offsets">全是指针的数组</param>
        /// <returns></returns>
        public VectorOffset CreateVector_Offset(Span<int> offsets)
        {
            NotNested();
            int oneObjectSize = sizeof(int);
            int allObjectSize = oneObjectSize * offsets.Length;
            StartVector(oneObjectSize, offsets.Length, oneObjectSize);
            Prep(oneObjectSize, allObjectSize);
            for (int i = 0; i < offsets.Length; i++)
            {
                int off = offsets[i];
                if (off != 0)
                    off = Offset - off + oneObjectSize;
                PutInt(off);
            }
            return EndVector();
        }
        /// <summary>
        /// 创建基本数据数组的指针
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="datas">全是基础类型的数组</param>
        /// <returns></returns>
        public unsafe VectorOffset CreateVector_Data<T>(Span<T> datas) where T : unmanaged
        {
            NotNested();
            int oneObjectSize = sizeof(T);
            int allObjectSize = oneObjectSize * datas.Length;
            StartVector(oneObjectSize, datas.Length, oneObjectSize);
            Prep(oneObjectSize, allObjectSize);
            Span<T> copyDatas = stackalloc T[datas.Length];
            fixed (T* src = datas, dest = copyDatas)
                Buffer.MemoryCopy(src, dest, allObjectSize, allObjectSize);
            copyDatas.Reverse();
            ToLittleEndian(copyDatas, oneObjectSize);
            _space -= allObjectSize;
            fixed (T* prt = copyDatas)
            {
                Span<byte> datas_byte = new Span<byte>(prt, allObjectSize);
                datas_byte.CopyTo(_bb.ToSpan(_space, allObjectSize));
            }
            return EndVector();
        }
        public VectorOffset CreateVector_Builder(FlatBufferBuilder datasForm) => CreateVector_Data(datasForm.ToSpan());

        private static void ToLittleEndian<T>(Span<T> array, int oneObjectSize) where T : unmanaged
        {
            Span<byte> bytes = MemoryMarshal.AsBytes(array);
            // 如果当前系统是大端序，则反转字节
            if (oneObjectSize > 1 && !BitConverter.IsLittleEndian)
            {
                for (int i = 0; i < array.Length; i++)
                {
                    var elementSpan = bytes.Slice(i * oneObjectSize, oneObjectSize);
                    elementSpan.Reverse(); // 反转当前元素的字节序
                }
            }
        }

        public override string ToString()
        {
            var span = ToSpan();
            StringBuilder sb = new StringBuilder(span.Length);
            for (int i = 0; i < span.Length; i++)
                sb.Append(span[i]);
            return sb.ToString();
        }

        private static List<FlatBufferBuilder> pool = new List<FlatBufferBuilder>(26);
        public static FlatBufferBuilder TakeOut(int size)
        {
            if (size <= 0) size = 1024;
            int minGap = int.MaxValue;
            int minGapIndex = -1;

            int maxSize = size;
            int maxSizeIndex = -1;
            lock (pool)
            {
                for (int i = pool.Count - 1; i >= 0; i--)
                {
                    var builder = pool[i];
                    int gap = builder.DataBuffer.Length - size;
                    if (gap >= 0 && gap < minGap)
                    {
                        //差距在一个size以内，直接返回
                        if (gap < size)
                        {
                            minGapIndex = i;
                            break;
                        }
                        minGap = gap;
                        minGapIndex = i;
                    }
                    if (builder.DataBuffer.Length > maxSize)
                    {
                        maxSize = builder.DataBuffer.Length;
                        maxSizeIndex = i;
                    }
                }
                FlatBufferBuilder ret = null;
                if (minGapIndex != -1)
                {
                    ret = pool[minGapIndex];
                    pool.RemoveAt(minGapIndex);
                }
                else if (maxSizeIndex != -1)
                {
                    ret = pool[maxSizeIndex];
                    pool.RemoveAt(maxSizeIndex);
                }
                else
                {
                    ret = new FlatBufferBuilder(size * 2);
                }
                return ret;
            }
        }
        public static void PutIn(FlatBufferBuilder builder)
        {
            builder.Clear();
            lock (pool)
            {
                pool.Add(builder);
            }
        }

        public Span<byte> ToSpan() => DataBuffer.ToSpan(DataBuffer.Position, Offset);
    }
}