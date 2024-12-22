using System;

namespace Infrastructure.State
{
    public interface IStateMachine<TState,TTrigger> where TState : IState
    {
        public void Registration(TState T, TTrigger trigger);
        public void Enter(TTrigger trigger);
    }
}