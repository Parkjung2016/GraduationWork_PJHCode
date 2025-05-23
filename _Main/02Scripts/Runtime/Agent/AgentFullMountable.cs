﻿using System;
using Animancer;
using Main.Runtime.Core;
using UnityEngine;

namespace Main.Runtime.Agents
{
    public class AgentFullMountable : MonoBehaviour, IAgentComponent, IAfterInitable
    {
        public bool IsFullMounted { get; private set; }
        public event Action<ITransition> OnFullMounted;//시작
        public event Action OnEndFullMounted;//끝
        private Agent _agent;

        public void Initialize(Agent agent)
        {
            _agent = agent;
        }

        public void AfterInitialize()
        {
            _agent.GetCompo<AgentAnimationTrigger>(true).OnGetUp += HandleGetUp;
        }

        private void HandleGetUp()
        {
            OnEndFullMounted?.Invoke();
            IsFullMounted = false;
        }

        public void FullMounted(ITransition fullMountedAnimationClip)
        {
            OnFullMounted?.Invoke(fullMountedAnimationClip);
            IsFullMounted = true;
        }
    }
}