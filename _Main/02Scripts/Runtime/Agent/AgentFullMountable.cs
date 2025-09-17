using System;
using Animancer;
using Main.Runtime.Core;
using UnityEngine;

namespace Main.Runtime.Agents
{
    public class AgentFullMountable : MonoBehaviour, IAgentComponent, IAfterInitable
    {
        public bool IsFullMounted { get; private set; }
        public event Action OnFullMounted; //시작
        public event Action OnEndFullMounted; //끝
        private Agent _agent;

        public void Initialize(Agent agent)
        {
            _agent = agent;
        }

        public void AfterInitialize()
        {
            _agent.GetCompo<AgentAnimationTrigger>(true).OnGetUp += HandleGetUp;
            _agent.HealthCompo.OnApplyDamaged += HandleApplyDamaged;
        }

        private void OnDestroy()
        {
            _agent.GetCompo<AgentAnimationTrigger>(true).OnGetUp -= HandleGetUp;
            _agent.HealthCompo.OnApplyDamaged -= HandleApplyDamaged;
        }

        private void HandleApplyDamaged(float value)
        {
            if (IsFullMounted)
            {
                OnEndFullMounted?.Invoke();
                IsFullMounted = false;
            }
        }

        private void HandleGetUp()
        {
            OnEndFullMounted?.Invoke();
            IsFullMounted = false;
        }

        public void FullMounted()
        {
            OnFullMounted?.Invoke();
            _agent.IsKnockDown = false;
            IsFullMounted = true;
        }
    }
}