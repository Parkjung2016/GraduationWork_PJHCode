using System;
using System.Threading;
using Animancer;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Main.Core;
using Main.Runtime.Core.Events;
using Main.Runtime.Manager;
using Main.Runtime.Manager.VolumeTypes;
using Main.Shared;
using PJH.Runtime.Players;
using UnityEngine;
using Debug = Main.Core.Debug;

namespace PJH.Runtime.PlayerPassive.Passives
{
    [CreateAssetMenu(menuName = "SO/Passive/WarpStrikePassive")]
    public class WarpStrikePassiveSO : PassiveSO
    {
        public ClipTransition getDamagedAnimationClip;
        public float cameraFOVWhenSelectingTargeting = 45f;
        public float changeCameraFOVDuration = .3f;
        public float changeTimeScaleDuration = .5f;
        public float targetSelectionTime = 2f;

        [SerializeField] private float _power;
        private GameEventChannelSO _gameEventChannel;
        private CancellationTokenSource _targetSelectionTokenSource;
        private Player _player;

        public override void Init(IPlayer player)
        {
            _player = player as Player;
            _gameEventChannel = AddressableManager.Load<GameEventChannelSO>("GameEventChannel");
        }

        public override void ActivePassive()
        {
            _player.GetCompo<PlayerAnimationTrigger>().OnTriggerPassiveAfterAttack += HandleTriggerPassiveAfterAttack;
        }

        public override void DeactivePassive()
        {
            _player.GetCompo<PlayerAnimationTrigger>().OnTriggerPassiveAfterAttack -= HandleTriggerPassiveAfterAttack;
        }

        private async void HandleTriggerPassiveAfterAttack()
        {
            try
            {
                if (_targetSelectionTokenSource is { IsCancellationRequested: false })
                {
                    _targetSelectionTokenSource.Cancel();
                    _targetSelectionTokenSource.Dispose();
                }

                _targetSelectionTokenSource = new();
                var changeCameraFOVEvent = GameEvents.ChangeCameraFOV;
                changeCameraFOVEvent.ignoreTimeScale = true;
                changeCameraFOVEvent.resetFOV = false;
                changeCameraFOVEvent.fovValue = cameraFOVWhenSelectingTargeting;
                changeCameraFOVEvent.changeDuration = changeCameraFOVDuration;
                _gameEventChannel.RaiseEvent(changeCameraFOVEvent);
                var changeCameraUpdateEvent = GameEvents.ChangeCameraUpdate;
                changeCameraUpdateEvent.updateIgnoreTimeScale = true;
                _gameEventChannel.RaiseEvent(changeCameraUpdateEvent);
                Sequence seq = DOTween.Sequence();
                seq.Append(DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 0, changeTimeScaleDuration));
                seq.SetUpdate(true);
                Managers.VolumeManager.GetVolumeType<RadialBlurVolumeType>().SetValue(.5f, .2f);
                _player.HealthCompo.IsInvincibility = true;
                await seq;
                _player.GetCompo<PlayerWarpStrike>().EnableWarpStrike(_power, getDamagedAnimationClip, () =>
                {
                    EndTargetSelection(true);
                    if (_targetSelectionTokenSource is { IsCancellationRequested: false })
                    {
                        _targetSelectionTokenSource.Cancel();
                        _targetSelectionTokenSource.Dispose();
                    }
                });
                float elapsed = 0f;

                while (elapsed < targetSelectionTime)
                {
                    if (_targetSelectionTokenSource is { IsCancellationRequested: true })
                    {
                        EndTargetSelection(true);
                        return;
                    }
                    // TODO °ÔÀÓ ¸ØÃèÀ» ¶§ Á¶°Ç Ãß°¡
                    // if (TimeManager)
                    // {
                    //     await UniTask.Yield(PlayerLoopTiming.Update, _targetSelectionTokenSource.Token);
                    //     continue;
                    // }

                    elapsed += Time.unscaledDeltaTime;
                    await UniTask.Yield(PlayerLoopTiming.Update, _targetSelectionTokenSource.Token);
                }

                EndTargetSelection();
                _player.GetCompo<PlayerWarpStrike>().DisableWarpStrike();
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }

        private void EndTargetSelection(bool immediatelyEnd = false)
        {
            var changeCameraFOVEvent = GameEvents.ChangeCameraFOV;
            _player.HealthCompo.IsInvincibility = false;
            if (immediatelyEnd)
                Time.timeScale = 1;
            else
                DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 1, changeTimeScaleDuration).SetUpdate(true);
            Managers.VolumeManager.GetVolumeType<RadialBlurVolumeType>().SetValue(0, .2f);
            changeCameraFOVEvent.ignoreTimeScale = true;
            changeCameraFOVEvent.resetFOV = true;
            changeCameraFOVEvent.changeDuration = changeCameraFOVDuration;
            _gameEventChannel.RaiseEvent(changeCameraFOVEvent);
            var changeCameraUpdateEvent = GameEvents.ChangeCameraUpdate;
            changeCameraUpdateEvent.updateIgnoreTimeScale = false;
            _gameEventChannel.RaiseEvent(changeCameraUpdateEvent);
        }
    }
}