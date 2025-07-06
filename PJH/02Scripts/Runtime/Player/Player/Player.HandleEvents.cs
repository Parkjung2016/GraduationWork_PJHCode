using Animancer;
using DG.Tweening;
using Main.Runtime.Core.Events;
using UnityEngine;

namespace PJH.Runtime.Players
{
    public partial class Player
    {
        private void HandleAdjustTimelineModelPosition(Transform targetTrm, float distanceFromEnemy)
        {
            Vector3 startPosition =
                targetTrm.position + targetTrm.forward * distanceFromEnemy;
            Vector3 lookAtDir = (targetTrm.position - startPosition).normalized;
            lookAtDir.y = 0;
            Quaternion lookAtRotation = Quaternion.LookRotation(lookAtDir);
            ModelTrm.rotation = lookAtRotation;
            transform.position = startPosition;
        }


        private void HandleEndFullMount()
        {
            PlayerInput.EnablePlayerInput(true);
        }

        private void HandleFullMount(ITransition obj)
        {
            PlayerInput.EnablePlayerInput(false);
        }

        private void HandleDeath()
        {
            DOVirtual.DelayedCall(1.6f, () =>
            {
                var evt = GameEvents.PlayerDeath;
                _gameEventChannel.RaiseEvent(evt);
                PlayerInput.EnablePlayerInput(false);
                _componentManager.EnableComponents(false);
            });
        }

        // private void HandleFinisherTimeline(bool isFinishering)
        // {
        //     PlayerInput.EnablePlayerInput(!isFinishering);
        // }

        private void HandleRun(bool isRunning)
        {
            if (_attackCompo.IsInBattle) return;
            bool isChangeConfig = isRunning;
            if (PlayerInput.Input.sqrMagnitude < .1f)
            {
                isChangeConfig = false;
            }

            var evt = GameEvents.CameraViewConfig;
            evt.isChangeConfig = isChangeConfig;
            _gameEventChannel.RaiseEvent(evt);
        }

        private void HandleLockOnToggle()
        {
            IsLockOn = !IsLockOn;
        }

        private void HandleEvasionWhileHitting()
        {
            IsHitting = false;
        }

        private void HandleFinisher()
        {
            PlayerInput.EnablePlayerInput(false);
            HealthCompo.IsInvincibility = true;
        }

        private void HandleFinisherEnd()
        {
            PlayerInput.EnablePlayerInput(true);
            HealthCompo.IsInvincibility = false;
        }
    }
}