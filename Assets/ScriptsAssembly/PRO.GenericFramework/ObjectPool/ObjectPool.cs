using System;

namespace PRO.Tool
{
    public class ObjectPool<T> : ObjectPoolArbitrary<T> where T : class, new()
    {
        public ObjectPool() : base(() => new T())
        {

        }
    }

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
