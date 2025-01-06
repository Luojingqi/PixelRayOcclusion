namespace PRO.Tool
{
    public class ObjectPool<T> : ObjectPoolBase<T> where T : class, new()
    {
        protected override T NewObject()
        {
            return new T();
        }
    }
}
