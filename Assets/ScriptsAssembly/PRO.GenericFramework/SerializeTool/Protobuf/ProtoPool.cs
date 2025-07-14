using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PRO.Proto
{
    public static class ProtoPool
    {
        public class Pool<T> where T : IMessage<T>
        {
            public Queue<T> queue = new Queue<T>();
            public Func<T> createObjectAction;
            public T TakeOut()
            {
                lock (this)
                {
                    if (queue.Count > 0)
                    {
                        return queue.Dequeue();
                    }
                    else
                    {
                        return createObjectAction();
                    }
                }
            }
            public void PutIn(T value)
            {
                lock (this)
                {
                    queue.Enqueue(value);
                }
            }
        }

        public readonly static Dictionary<Type, object> poolDic = new Dictionary<Type, object>();
        public static T TakeOut<T>() where T : IMessage<T>
        {
            var pool = GetPool<T>();
            return pool.TakeOut();
        }
        public static void PutIn<T>(this T value) where T : IMessage<T>
        {
            var pool = GetPool<T>();
            pool.PutIn(value);
        }
        public static Pool<T> GetPool<T>() where T : IMessage<T>
        {
            if (poolDic.TryGetValue(typeof(T), out object obj))
            {
                return obj as Pool<T>;
            }
            else
            {
                RuntimeHelpers.RunClassConstructor(typeof(T).TypeHandle);
                return poolDic[typeof(T)] as Pool<T>;
            }
        }
    }
}

