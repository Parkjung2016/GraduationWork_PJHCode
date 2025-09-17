using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using FIMSpace.FProceduralAnimation;
using Main.Runtime.Core;
using UnityEngine;

namespace Main.Runtime.Agents
{
    public class AgentIK : MonoBehaviour, IAgentComponent, IAfterInitable
    {
        public RagdollAnimator2 RagdollAnimator { get; private set; }
        public LegsAnimator LegsAnimator { get; private set; }

        private Agent _agent;
        private CancellationTokenSource _triggerRagdollTokenSource;

        public virtual void Initialize(Agent agent)
        {
            LegsAnimator = GetComponent<LegsAnimator>();
            if (LegsAnimator != null && !LegsAnimator.enabled) LegsAnimator = null;
            RagdollAnimator = GetComponent<RagdollAnimator2>();
            _agent = agent;
        }

        public virtual void AfterInitialize()
        {
            AgentAnimationTrigger animationTriggerCompo = _agent.GetCompo<AgentAnimationTrigger>(true);
            animationTriggerCompo.OnTriggerRagdoll += HandleTriggerRagdoll;

            AgentAnimator agentAnimatorCompo = _agent.GetCompo<AgentAnimator>(true);
            _agent.HealthCompo.OnApplyDamaged += HandleApplyDamaged;
            agentAnimatorCompo.OnEndHitAnimation += HandleEndHitAnimation;
            if (_agent.TryGetCompo(out AgentFinisherable finisherableCompo))
            {
                finisherableCompo.OnSetToFinisherTarget += HandleSetToFinisherTarget;
            }
        }

        protected virtual void OnDestroy()
        {
            AgentAnimationTrigger animationTriggerCompo = _agent.GetCompo<AgentAnimationTrigger>(true);
            animationTriggerCompo.OnTriggerRagdoll -= HandleTriggerRagdoll;
            AgentAnimator agentAnimatorCompo = _agent.GetCompo<AgentAnimator>(true);
            _agent.HealthCompo.OnApplyDamaged -= HandleApplyDamaged;
            agentAnimatorCompo.OnEndHitAnimation -= HandleEndHitAnimation;
            if (_agent.TryGetCompo(out AgentFinisherable finisherableCompo))
            {
                finisherableCompo.OnSetToFinisherTarget -= HandleSetToFinisherTarget;
            }
        }

        private void HandleEndHitAnimation()
        {
            if (LegsAnimator)
            {
                LegsAnimator.enabled = false;
            }
        }

        private void HandleApplyDamaged(float obj)
        {
            if (LegsAnimator)
            {
                LegsAnimator.enabled = true;
            }
        }

        private void HandleSetToFinisherTarget()
        {
            if (_triggerRagdollTokenSource is { IsCancellationRequested: false })
            {
                _triggerRagdollTokenSource.Cancel();
                _triggerRagdollTokenSource.Dispose();
            }

            RagdollAnimator.User_SwitchFallState(RagdollHandler.EAnimatingMode.Off);
            _agent.GetCompo<AgentAnimator>(true).Animator.enabled = true;
        }

        protected virtual async void HandleTriggerRagdoll()
        {
            if (LegsAnimator)
                LegsAnimator.enabled = false;
            if (!RagdollAnimator)
            {
                _agent.GetCompo<AgentAnimator>(true).Animator.enabled = false;
                return;
            }

            if (_triggerRagdollTokenSource is { IsCancellationRequested: false })
            {
                _triggerRagdollTokenSource.Cancel();
                _triggerRagdollTokenSource.Dispose();
            }

            _triggerRagdollTokenSource = new();
            _triggerRagdollTokenSource.RegisterRaiseCancelOnDestroy(gameObject);
            try
            {
                Vector3 agentPos = _agent.transform.position;
                agentPos.y += .15f;
                _agent.transform.position = agentPos;
                AgentAnimator agentAnimatorCompo = _agent.GetCompo<AgentAnimator>(true);
                agentAnimatorCompo.Animator.enabled = true;
                RagdollAnimator.User_Teleport();
                await UniTask.Yield(cancellationToken: _triggerRagdollTokenSource.Token);
                RagdollAnimator.Settings.StoreCalibrationPose();
                RagdollAnimator.User_SwitchFallState(RagdollHandler.EAnimatingMode.Falling);
                agentAnimatorCompo.Animancer.Speed = 0;
            }
            catch (Exception e)
            {
            }
        }
    }
}