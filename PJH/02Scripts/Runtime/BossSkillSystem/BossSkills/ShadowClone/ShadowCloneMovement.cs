using Main.Runtime.Agents;
using Main.Runtime.Core;
using Pathfinding;
using Pathfinding.RVO;
using UnityEngine;

namespace PJH.Runtime.BossSkill.BossSkills.ShadowClones
{
    public class ShadowCloneMovement : MonoBehaviour, IAgentComponent, IAfterInitable
    {
        private ShadowClone _shadowClone;
        public AIPath AIPathCompo { get; private set; }
        public RVOController RvoControllerCompo { get; private set; }

        public bool IsArrived => AIPathCompo.reachedDestination;


        public void Initialize(Agent agent)
        {
            _shadowClone = agent as ShadowClone;
            AIPathCompo = _shadowClone.GetComponent<AIPath>();
            RvoControllerCompo = _shadowClone.GetComponent<RVOController>();
        }

        public void AfterInitialize()
        {
            _shadowClone.GetCompo<ShadowCloneAnimator>().AnimatorMoveEvent += HandleAnimatorMove;
        }

        private void OnDestroy()
        {
            _shadowClone.GetCompo<ShadowCloneAnimator>().AnimatorMoveEvent -= HandleAnimatorMove;
        }

        private void HandleAnimatorMove(Animator animator)
        {
            if (!animator.applyRootMotion) return;
            if (!AIPathCompo || !AIPathCompo.enabled) return;
            if (AIPathCompo.hasPath) AIPathCompo.SetPath(null);
            Vector3 nextPosition;
            Quaternion nextRotation;
            AIPathCompo.MovementUpdate(Time.deltaTime, out nextPosition, out nextRotation);
            nextPosition = animator.rootPosition;
            AIPathCompo.FinalizeMovement(nextPosition, nextRotation);
        }

        public void SetDestination(Vector3 targetPosition)
        {
            AIPathCompo.destination = targetPosition;
        }

        public void SetCanMove(bool canMove) => AIPathCompo.canMove=canMove;
        public void SetRVOControllerLocked(bool locked) => RvoControllerCompo.locked=locked;
    }
}