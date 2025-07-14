using PRO.Tool;

namespace PRO.Buff.Base
{
    public abstract class BuffBase
    {
        /// <summary>
        /// 应用buff的效果到战斗上下文中，与buff的活跃状态有关
        /// </summary>
        /// <param name="context"></param>
        /// <param name="index"></param>
        public abstract void ApplyEffect(CombatContext context, int index);

        public Role Agent { get; private set; }

        public BuffConfig Config => config;
        private BuffConfig config;
        public string Name => config.Name;
        public abstract BuffTriggerType TriggerType { get; }
        public string guid;
        public BuffBase()
        {
            config = AssetManagerEX.LoadBuffConfig(this);
            guid = System.Guid.NewGuid().ToString();
        }
        /// <summary>
        /// 此接口只需重写禁止调用，请使用Role.AddBuff()
        /// </summary>
        public virtual bool RoleAddThis(Role role)
        {
            bool ret = true;
            for (int i = 0; i < childBuffList.Count; i++)
                ret = ret && childBuffList[i].RoleAddThis(role);
            var type = this.GetType();
            if (config.独有)
            {
                if (role.BuffTypeDic.ContainsKey(type))
                    return false;
            }
            if (role.BuffTypeDic.TryGetValue(type, out var list))
            {
                list.Add(this);
                role.BuffTypeDic[type] = list;
            }
            else
            {
                list = new ReusableList<BuffBase>(1);
                list.Add(this);
                role.BuffTypeDic.Add(type, list);
            }
            role.AllBuff[(int)TriggerType].Add(this, config.Priority);
            Agent = role;
            return true && ret;
        }
        /// <summary>
        /// 此接口只需重写禁止调用，请使用Role.RemoveBuff()
        /// </summary>
        public virtual bool RoleRemoveThis()
        {
            bool ret = true;
            for (int i = 0; i < childBuffList.Count; i++)
                ret = ret && childBuffList[i].RoleRemoveThis();
            if (Agent == null) return false;
            var type = this.GetType();
            ret = ret && Agent.AllBuff[(int)TriggerType].Remove(this, config.Priority);
            var list = Agent.BuffTypeDic[type];
            if (list.Count > 1)
            {
                var newList = new ReusableList<BuffBase>(list.Count);
                for (int i = 0; i < list.Count; i++)
                    if (list[i] != this)
                        newList.Add(list[i]);
                Agent.BuffTypeDic[type] = newList;
            }
            else
            {
                Agent.BuffTypeDic.Remove(type);
            }
            list.Dispose();
            Agent = null;
            return ret;
        }

        /// <summary>
        /// 更新buff的帧更新检查
        /// </summary>
        public virtual void Update() { }

        protected ReusableList<IChildBuff> childBuffList;
        protected void AddChildBuuff(IChildBuff childBuff)
        {
            if (childBuffList.Length <= 0)
                childBuffList = new ReusableList<IChildBuff>(1);
            childBuffList.Add(childBuff);
        }
    }

    public interface IChildBuff
    {
        public bool RoleAddThis(Role role);
        public bool RoleRemoveThis();
        public Role Agent { get; }
    }
}
