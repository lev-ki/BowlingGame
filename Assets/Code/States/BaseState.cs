using System;
using System.Collections.Generic;
using UnityEngine;

namespace Code.States
{
    public class BaseState : ScriptableObject
    {
        [SerializeField] private BaseState subState;
        [SerializeField] private BaseState defaultSubState;
        
        [Serializable]
        public class Transition
        {
            public BaseState source;
            public bool anySource;
            public EventId reason;
            public BaseState target;
        }
        
        public List<Transition> transitions;
        private bool isLeaf;

        private void OnEnable()
        {
            isLeaf = defaultSubState == null;
        }

        public virtual void OnEnter()
        {
            // Debug.Log($"{name} OnEnter");
            if (isLeaf)
            {
                return;
            }
            subState = defaultSubState;
            subState.OnEnter();
        }
        
        public virtual void OnExit()
        {
            // Debug.Log($"{name} OnEnter");
            if (isLeaf)
            {
                return;
            }
            subState.OnExit();
        }
        
        public virtual void InvokeEvent(EventId eventId)
        {
            var transition = transitions.Find(t => t.reason == eventId && (t.anySource || t.source == subState));
            if (transition != null)
            {
                TransitionToState(transition.target);
                return;
            }
            if (!isLeaf)
            {
                subState.InvokeEvent(eventId);
            }
        }

        private void TransitionToState(BaseState newState)
        {
            subState.OnExit();
            subState = newState;
            subState.OnEnter();
        }
    }
}