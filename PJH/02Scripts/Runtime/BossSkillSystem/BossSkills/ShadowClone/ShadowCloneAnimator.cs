using System;
using Main.Runtime.Agents;
using Main.Runtime.Animators;
using UnityEngine;

namespace PJH.Runtime.BossSkill.BossSkills.ShadowClones
{
    public class ShadowCloneAnimator : AgentAnimator
    {
        public event Action<Animator> AnimatorMoveEvent;
        [SerializeField] private AnimParamSO _velocityParamSO, _isMovingParamSO, _horizontalParamSO, _verticalParamSO;

        private ShadowCloneMovement _movementCompo;
        private ShadowClone _shadowClone;

        public override void Initialize(Agent agent)
        {
            base.Initialize(agent);
            _shadowClone = agent as ShadowClone;
            _movementCompo = _shadowClone.GetCompo<ShadowCloneMovement>();
        }

        private void Start()
        {
            Animator.applyRootMotion = false;
        }

        public void SetRootMotion(bool isRootMotion)
        {
            Animator.applyRootMotion = isRootMotion;
        }

        private void OnAnimatorMove()
        {
            if (Time.deltaTime <= 0) return;
            AnimatorMoveEvent?.Invoke(Animator);
        }

        private void Update()
        {
            Vector3 targetDirection = _movementCompo.AIPathCompo.steeringTarget - transform.position;
            targetDirection.y = 0;

            float horizontal = Vector3.Dot(transform.right, targetDirection.normalized);
            float vertical = Vector3.Dot(transform.forward, targetDirection.normalized);

            bool isRunning = _movementCompo.AIPathCompo.maxSpeed == _agent.RunSpeedStat.Value;
            float offset = 0.25f * Convert.ToByte(isRunning) + 0.5f;
            float velocity = _movementCompo.AIPathCompo.velocity.sqrMagnitude;
            bool isMoving = velocity > 0.1f;
            SetParam(_isMovingParamSO, isMoving);
            SetParam(_velocityParamSO, velocity, .1f, Time.deltaTime);
            SetParam(_horizontalParamSO, horizontal * offset, 0.1f, Time.deltaTime);
            SetParam(_verticalParamSO, vertical * offset, 0.1f, Time.deltaTime);
        }
    }
}