using Animancer;
using UnityEngine;

namespace PJH.Runtime.Players
{
    public partial class PlayerMovement
    {
        private void HandleEndKnockBack()
        {
            IsKnockBack = false;
        }

        private void HandleStartKnockBack(Vector3 knockBackDir, float knockBackPower)
        {
            _knockBackPower = knockBackPower;
            _knockBackDir = knockBackDir;
            IsKnockBack = true;
        }

        private void HandleFullMount(ITransition obj)
        {
            CC.enabled = false;
        }

        private void HandleEndFullMount()
        {
            CC.enabled = true;
        }

        private void HandleEndEvasion()
        {
            IsEvading = false;
            _currentEvasionDelayTime = Time.time;
            _isEvadingCoolTime = true;
            if (IsManualMove)
                EndManualMove();
        }

        private void HandleDeath() => CC.enabled = false;

        private void HandleFinisherTimeline(bool isFinishering)
        {
            enabled = !isFinishering;
            CC.enabled = !isFinishering;
        }

        private void HandleMovement(Vector3 input)
        {
            if (IsRunning) OnRun?.Invoke(input != Vector3.zero);
        }

        private void HandleAnimatorMove(Vector3 deltaPosition, Quaternion deltaRotation)
        {
            if (!CanMove || !CC.enabled) return;
            _player.ModelTrm.rotation = deltaRotation;
            if (_animatorCompo.IsEnabledInputWhileRootMotion) return;
            _velocity = deltaPosition * _rootMotionMultiplierCurve.Value;
            _velocity.y = _yVelocity;
            CC.Move(_velocity);
        }

        private void HandleRun(bool isRunning)
        {
            if (!IsRunning)
            {
                Evasion();
            }

            IsRunning = isRunning;
            OnRun?.Invoke(isRunning);
        }

        private void HandleBlockEnd()
        {
            _currentEvasionDelayTime = Time.time;
        }
    }
}