namespace PRO.Tool
{
    public class ObjectPool<T> : ObjectPoolBase<T> where T : class, new()
    {
        public ObjectPool(int maxNuber, bool isCanExceed) : base(maxNuber, isCanExceed)
        {
        }

        public override void Destroy(T item)
        {

        }

        protected override T NewObject()
        {
            return new T();
        }
    }
}
