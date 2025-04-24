using Animancer;
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

        private void HandleAvoidingAttack()
        {
            _avoidingAttackFeedback?.PlayFeedbacks();
        }


        private void HandleEndFullMount()
        {
            PlayerInput.EnablePlayerInput(true);
        }

        private void HandleFullMount(ITransition obj)
        {
            PlayerInput.EnablePlayerInput(false);
        }

        private void HandleFinishTimeline(FinishTimeline evt)
        {
            ModelTrm.localPosition = Vector3.zero;
        }


        private void HandleReOffsetPlayer(ReOffsetPlayer evt)
        {
            Vector3 realPosition = ModelTrm.position;
            transform.position = realPosition;
            ModelTrm.localPosition = Vector3.zero;
        }

        private void HandleDeath()
        {
            var evt = GameEvents.PlayerDeath;
            _deathCamera.Priority = 2;
            _gameEventChannel.RaiseEvent(evt);
            PlayerInput.EnablePlayerInput(false);
            _componentManager.EnableComponents(false);
        }

        private void HandleFinisherTimeline(bool isFinishering)
        {
            PlayerInput.EnablePlayerInput(!isFinishering);
        }

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

            OnLockOn?.Invoke(IsLockOn);
        }
    }
}