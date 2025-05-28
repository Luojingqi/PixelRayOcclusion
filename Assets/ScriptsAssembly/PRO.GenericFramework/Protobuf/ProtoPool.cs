using Google.Protobuf;
using System;
using System.Collections.Generic;

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
        }

        public readonly static Dictionary<Type, object> poolDic = new Dictionary<Type, object>();
        public static T TakeOut<T>() where T : IMessage<T>
        {
            var pool = poolDic[typeof(T)] as Pool<T>;
            return pool.TakeOut();
        }
        public static void PutIn<T>(this T value) where T : IMessage<T>
        {
            var pool = poolDic[typeof(T)] as Pool<T>;
            lock (pool)
            {
                pool.queue.Enqueue(value);
            }
        }
    }
}

