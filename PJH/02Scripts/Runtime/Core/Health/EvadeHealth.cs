using Animancer;
using Main.Runtime.Agents;
using Main.Runtime.Combat.Core;
using Main.Runtime.Core.StatSystem;
using Main.Shared;
using UnityEngine;

namespace PJH.Runtime.Core.Health
{
    public class EvadeHealth : Main.Runtime.Combat.Health
    {
        [SerializeField] private TransitionAsset _leftEvade, _rightEvade;

        private Agent _agent;

        public override void Init(IAgent agent, StatSO maxHealthStat, StatSO maxShieldStat)
        {
            base.Init(agent, maxHealthStat, maxShieldStat);
            _agent = (base._agent as Agent);
        }

        protected override bool CanApplyDamage(GetDamagedInfo getDamagedInfo)
        {
            var animator = _agent.GetCompo<AgentAnimator>(true);
            Vector3 hitPoint = getDamagedInfo.hitPoint;
            Vector3 toHit = (hitPoint - _agent.transform.position).normalized;

            Vector3 right = _agent.transform.right;

            float dot = Vector3.Dot(toHit, right);

            animator.PlayAnimationClip(dot > 0f ? _rightEvade : _leftEvade);

            return false;
        }
    }
}