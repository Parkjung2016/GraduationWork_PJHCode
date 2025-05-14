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

        protected virtual void HandleTriggerRagdoll()
        {
            LegsAnimator.enabled = false;
            RagdollAnimator.User_SwitchFallState(RagdollHandler.EAnimatingMode.Falling);
        }
    }
}