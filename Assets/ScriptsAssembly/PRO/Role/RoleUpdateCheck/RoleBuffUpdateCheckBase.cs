namespace PRO.Buff.Base
{
    /// <summary>
    /// 角色的buff帧更新检查基类
    /// </summary>
    public abstract class RoleBuffUpdateCheckBase<T> where T : BuffBase<T>
    {
        public bool active = true;
        public T buff { get; private set; }
        public RoleBuffUpdateCheckBase(T buff)
        {
            this.buff = buff;
        }
        public abstract void Update();
    }
}
