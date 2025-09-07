using System;
using Cysharp.Threading.Tasks;
using Main.Runtime.Core.Events;
using Main.Runtime.Manager;
using UnityEngine;

namespace PJH.Runtime.Players
{
    public partial class Player
    {
        private void HandleEndFullMount()
        {
            PlayerInput.EnablePlayerInput(true);
        }

        private void HandleFullMount()
        {
            PlayerInput.EnablePlayerInput(false);
        }

        private async void HandleDeath()
        {
            try
            {
                _componentManager.EnableComponents(false);
                Managers.FMODManager.SetGameSoundVolume(0, 1f);
                await UniTask.WaitForSeconds(1.6f, cancellationToken: gameObject.GetCancellationTokenOnDestroy());
                var evt = GameEvents.PlayerDeath;
                _gameEventChannel.RaiseEvent(evt);
            }
            catch (Exception e)
            {
            }
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
            IsHitting = false;
        }

        private void HandleEndGrabbed()
        {
            IsHitting = false;
        }
    }
}