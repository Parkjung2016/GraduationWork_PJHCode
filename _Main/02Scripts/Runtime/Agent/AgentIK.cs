using System;
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

        public virtual void Initialize(Agent agent)
        {
            LegsAnimator = GetComponent<LegsAnimator>();
            RagdollAnimator = GetComponent<RagdollAnimator2>();
            _agent = agent;
        }

        public virtual void AfterInitialize()
        {
            AgentAnimationTrigger animationTriggerCompo = _agent.GetCompo<AgentAnimationTrigger>(true);
            animationTriggerCompo.OnTriggerRagdoll += HandleTriggerRagdoll;
        }

        protected virtual void OnDestroy()
        {
            AgentAnimationTrigger animationTriggerCompo = _agent.GetCompo<AgentAnimationTrigger>(true);
            animationTriggerCompo.OnTriggerRagdoll -= HandleTriggerRagdoll;
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

            try
            {
                Vector3 agentPos = _agent.transform.position;
                agentPos.y += .2f;
                _agent.transform.position = agentPos;
                RagdollAnimator.Settings.StoreCalibrationPose();
                RagdollAnimator.User_SwitchFallState(RagdollHandler.EAnimatingMode.Falling);
                await UniTask.WaitForSeconds(.4f, cancellationToken: gameObject.GetCancellationTokenOnDestroy());
                _agent.GetCompo<AgentAnimator>(true).Animator.enabled = false;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
    }
}