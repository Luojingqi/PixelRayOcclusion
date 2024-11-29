using System;

namespace PRO.GenericFramework
{
    public interface IFSMState<T> where T : Enum
    {
        /// <summary>
        /// 状态的管理器
        /// </summary>
        public FSMManager<T> FSM { get; set; }
        /// <summary>
        /// 状态的枚举
        /// </summary>
        public T EnumName { get; }
        public void Enter();
        public void Update();
        public void Exit();
    }
}
