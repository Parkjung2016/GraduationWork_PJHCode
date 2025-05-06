using System;
using Main.Runtime.Agents;
using UnityEngine;

namespace PJH.Runtime.Players
{
    public partial class PlayerAnimator : AgentAnimator
    {
        public override void Initialize(Agent agent)
        {
            base.Initialize(agent);
            _player = agent as Player;
            _attackCompo = _player.GetCompo<PlayerAttack>();
            _movementCompo = _player.GetCompo<PlayerMovement>();
            _blockCompo = _player.GetCompo<PlayerBlock>();
        }

        public override void AfterInitialize()
        {
            base.AfterInitialize();
            _attackSpeedStat = _player.GetCompo<PlayerStat>().GetStat(_attackSpeedStat);
            SubscribeEvents();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            UnSubscribeEvents();
        }


        private void DisableRootMotion()
        {
            EnableRootMotion(false);
        }


        private void EnableRootMotion(bool isEnabled)
        {
            IsRootMotion = isEnabled;
            if (isEnabled)
                IsEnabledInputWhileRootMotion = false;
        }


        private void Update()
        {
            if (_isPlayingTimeline) return;
            Vector3 input = _player.PlayerInput.Input;

            var movementCompo = _player.GetCompo<PlayerMovement>();
            float velocity = (movementCompo.CC.velocity.magnitude / movementCompo.Speed) * Time.timeScale;
            float offset = 0.5f * Convert.ToByte(movementCompo.IsRunning) + 0.5f;

            float value = velocity * offset;
            if (!movementCompo.enabled)
            {
                velocity = 0;
                value = 0;
            }

            SetParam(_isMovingParam, velocity > 0.1f);
            SetParam(_fadeOffLeaningParam, velocity < 0.1f);
            SetParam(_isGroundedParam, movementCompo.IsGrounded);
            SetParam(_velocityParam, value, .1f, Time.unscaledDeltaTime);
            SetParam(_horizontalParam, input.x, .3f, Time.unscaledDeltaTime);
            SetParam(_verticalParam, input.z, .3f, Time.unscaledDeltaTime);
        }

        private void OnAnimatorMove()
        {
            if (_isPlayingTimeline)
            {
                transform.position = Animancer.rootPosition;
                return;
            }

            if (!IsRootMotion) return;
            Vector3 deltaPosition = Animancer.deltaPosition;
            Quaternion targetRot = Animancer.Animator.targetRotation;
            AnimatorMoveEvent?.Invoke(deltaPosition, targetRot);
        }
    }
}