using System;

namespace PRO.Buff.Base
{
    public abstract class BuffBase<T> : IBuffBase where T : BuffBase<T>
    {
        public abstract BuffTriggerType TriggerType { get; }
        public abstract string Name { get; }
        /// <summary>
        /// 活动状态
        /// </summary>
        protected bool active = false;
        public abstract void ApplyEffect(CombatContext context, int index);

        public abstract RoleBuffUpdateCheckBase<T> UpdateCheck { get; }

        public Role Agent { get; private set; }
        /// <summary>
        /// 此接口只需重写禁止调用，请使用Role.AddBuff()
        /// </summary>
        public virtual void RoleAddThis(Role role)
        {
            Agent = role;
        }
        /// <summary>
        /// 此接口只需重写禁止调用，请使用Role.RemoveBuff()
        /// </summary>
        public virtual void RoleRemoveThis()
        {
            Agent = null;
        }
        public bool GetActive() => active;
        /// <summary>
        /// 设置buff的活动状态，由于设置时可能会处理buff冲突，所以先设置buff内的各种属性字段的值，然后在调用此方法
        /// </summary>
        /// <param name="active"></param>
        public virtual void SetActive(bool active) => this.active = active;

        public void UpdateCheckAction()
        {
            if (UpdateCheck != null && UpdateCheck.active)
                UpdateCheck?.Update();
        }
    }

    public interface IBuffBase
    {
        public BuffTriggerType TriggerType { get; }
        public string Name { get; }
        public void ApplyEffect(CombatContext context, int index);
        /// <summary>
        /// 此接口只需重写禁止调用，请使用Role.AddBuff()
        /// </summary>
        public void RoleAddThis(Role role);
        /// <summary>
        /// 此接口只需重写禁止调用，请使用Role.RemoveBuff()
        /// </summary>
        public void RoleRemoveThis();
        public bool GetActive();
        public void UpdateCheckAction();
    }
}


namespace PRO.Buff.Base.BuffAux
{
    public abstract class BuffAuxBase_回合开始 : BuffBase<BuffAuxBase_回合开始>
    {
        public override BuffTriggerType TriggerType => BuffTriggerType.回合开始时;
        public override string Name => name;
        private string name = $"回合开始_{Guid.NewGuid()}";
        public override RoleBuffUpdateCheckBase<BuffAuxBase_回合开始> UpdateCheck => null;
    }
    public abstract class BuffAuxBase_回合结束 : BuffBase<BuffAuxBase_回合结束>
    {
        public override BuffTriggerType TriggerType => BuffTriggerType.回合结束时;
        public override string Name => name;
        private string name = $"回合结束_{Guid.NewGuid()}";
        public override RoleBuffUpdateCheckBase<BuffAuxBase_回合结束> UpdateCheck => null;
    }
}
