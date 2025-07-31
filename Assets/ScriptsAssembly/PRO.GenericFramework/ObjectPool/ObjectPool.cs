using System;

namespace PRO.Tool
{
    /// <summary>
    /// 简易对象池
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObjectPool<T> : ObjectPoolArbitrary<T> where T : class, new()
    {
        public ObjectPool() : base(() => new T())
        {

        }
    }
    /// <summary>
    /// 任意对象池
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObjectPoolArbitrary<T> : ObjectPoolBase<T> where T : class
    {
        private Func<T> ConstructorFunction;
        public ObjectPoolArbitrary(Func<T> ConstructorFunction)
        {
            this.ConstructorFunction = ConstructorFunction;
        }
        protected override T NewObject()
        {
            return ConstructorFunction.Invoke();
        }
    }
}
