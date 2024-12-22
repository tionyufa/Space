using System;
using System.Collections.Generic;
using UnityEngine;

namespace Infrastructure.State
{
    public class BaseStateMachine<TState,TTrigger> : IStateMachine<TState,TTrigger> where TState : IState
    {
        private Dictionary<TTrigger, TState> _states = new Dictionary<TTrigger, TState>();
        private IState _activeState;
        public Action<TState> OnChangeState { get; set; }

        public void Registration(TState T, TTrigger trigger)
        {
            _states.Add(trigger,T);
        }

        public void Enter(TTrigger trigger)
        {
            var nextState = _states[trigger];
            _activeState?.Exit();
            _activeState = nextState;
            _activeState.Enter();
            
            OnChangeState?.Invoke(nextState);
        }
    }
}