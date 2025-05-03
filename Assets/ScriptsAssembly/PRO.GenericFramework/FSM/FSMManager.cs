using System;
using System.Collections.Generic;

namespace PRO.Tool
{
    public abstract class FSMManager<T> where T : Enum
    {
        public IFSMState<T> NowState { get; protected set; }
        protected Dictionary<T, IFSMState<T>> stateDic = new Dictionary<T, IFSMState<T>>();
        protected Dictionary<Type, IFSMState<T>> stateTypeDic = new Dictionary<Type, IFSMState<T>>();
        protected void AddState(IFSMState<T> IState)
        {
            IState.FSM = this;
            stateDic.Add(IState.EnumName, IState);
            stateTypeDic.Add(IState.GetType(), IState);
        }

        public StateType GetState<StateType>() where StateType : IFSMState<T>
        {
            if (stateTypeDic.TryGetValue(typeof(StateType), out IFSMState<T> value)) return (StateType)value;
            else return default;
        }
        public IFSMState<T> GetState(T stateType)
        {
            if (stateDic.TryGetValue(stateType, out IFSMState<T> value)) return value;
            else return null;
        }

        public void SwitchState(T nextState)
        {
            if (NowState == null)
            {
                NowState = stateDic[nextState]; NowState.Enter(); return;
            }
            if (NowState.EnumName.GetHashCode() == nextState.GetHashCode()) return;
            if (stateDic.TryGetValue(nextState, out IFSMState<T> value))
            {
                NowState.Exit();
                NowState = value;
                NowState.Enter();
            }
            else return;
        }

        public void SwitchState(int nextState)
        {

        }

        public virtual void Update()
        {
            NowState.Update();
        }
    }

}
