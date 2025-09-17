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

        private void HandleEndEvasion()
        {
            if (IsEvadingWhileHitting)
            {
                IsEvadingWhileHitting = false;
                OnEvasionEndWhileHitting?.Invoke();
            }

            IsEvading = false;
            _currentEvasionDelayTime = Time.time;
            _canEvading = true;

            if (IsManualMove)
                EndManualMove();
        }

        private void HandleDeath() => CC.enabled = false;

        private void HandleMovement(Vector3 input)
        {
            if (IsRunning) OnRun?.Invoke(input != Vector3.zero);
        }

        private void HandleAnimatorMove(Vector3 deltaPosition, Quaternion deltaRotation)
        {
            if (!CanMove || !CC.enabled || _player.WarpingComponent.IsActive() ||
                _counterAttackCompo.IsCounterAttacking || _player.IsStunned) return;
            _player.ModelTrm.rotation = deltaRotation;
            if (_animatorCompo.IsEnabledInputWhileRootMotion) return;

            if (_attackCompo.IsAttacking)
            {
                Vector3 origin = transform.position + Vector3.up * 1f;
                Vector3 direction = transform.forward;

                RaycastHit hit;
                if (Physics.BoxCast(origin, _boxCastHalfExtentsWhenAttacking, direction, out hit, transform.rotation,
                        _castDistanceWhenAttacking))
                {
                    if (hit.collider.CompareTag("Enemy"))
                    {
                        ApplyRootMotion(Vector3.zero);
                        return;
                    }
                }
            }

            ApplyRootMotion(deltaPosition);
        }

        void ApplyRootMotion(Vector3 deltaPosition)
        {
            _velocity = deltaPosition * _rootMotionMultiplierCurve.Value;
            _velocity.y = _yVelocity;
            CC.Move(_velocity);
        }

        private void HandleRun(bool isRunning)
        {
            IsRunning = isRunning;
            OnRun?.Invoke(isRunning);
        }

        private void HandleBlockEnd()
        {
            _currentEvasionDelayTime = Time.time;
        }

        private void HandleEndRotatingTargetWhileAttack()
        {
            _isRotatingTargetWhileAttack = false;
        }
    }
}