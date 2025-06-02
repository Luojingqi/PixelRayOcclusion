using System;

namespace PRO.Buff.Base
{
    public abstract class BuffBase<T> : IBuffBase where T : BuffBase<T>
    {
        public abstract BuffTriggerType TriggerType { get; }
        public abstract string Name { get; }
        /// <summary>
        /// �״̬
        /// </summary>
        protected bool active = false;
        public abstract void ApplyEffect(CombatContext context, int index);

        public abstract RoleBuffUpdateCheckBase<T> UpdateCheck { get; }

        public Role Agent { get; private set; }
        /// <summary>
        /// �˽ӿ�ֻ����д��ֹ���ã���ʹ��Role.AddBuff()
        /// </summary>
        public virtual void RoleAddThis(Role role)
        {
            Agent = role;
        }
        /// <summary>
        /// �˽ӿ�ֻ����д��ֹ���ã���ʹ��Role.RemoveBuff()
        /// </summary>
        public virtual void RoleRemoveThis()
        {
            Agent = null;
        }
        public bool GetActive() => active;
        /// <summary>
        /// ����buff�Ļ״̬����������ʱ���ܻᴦ��buff��ͻ������������buff�ڵĸ��������ֶε�ֵ��Ȼ���ڵ��ô˷���
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
        /// �˽ӿ�ֻ����д��ֹ���ã���ʹ��Role.AddBuff()
        /// </summary>
        public void RoleAddThis(Role role);
        /// <summary>
        /// �˽ӿ�ֻ����д��ֹ���ã���ʹ��Role.RemoveBuff()
        /// </summary>
        public void RoleRemoveThis();
        public bool GetActive();
        public void UpdateCheckAction();
    }
}


namespace PRO.Buff.Base.BuffAux
{
    public abstract class BuffAuxBase_�غϿ�ʼ : BuffBase<BuffAuxBase_�غϿ�ʼ>
    {
        public override BuffTriggerType TriggerType => BuffTriggerType.�غϿ�ʼʱ;
        public override string Name => name;
        private string name = $"�غϿ�ʼ_{Guid.NewGuid()}";
        public override RoleBuffUpdateCheckBase<BuffAuxBase_�غϿ�ʼ> UpdateCheck => null;
    }
    public abstract class BuffAuxBase_�غϽ��� : BuffBase<BuffAuxBase_�غϽ���>
    {
        public override BuffTriggerType TriggerType => BuffTriggerType.�غϽ���ʱ;
        public override string Name => name;
        private string name = $"�غϽ���_{Guid.NewGuid()}";
        public override RoleBuffUpdateCheckBase<BuffAuxBase_�غϽ���> UpdateCheck => null;
    }
}
