using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Main.Core;
using Main.Runtime.Core.Events;
using Main.Runtime.Manager;
using Main.Runtime.Manager.VolumeTypes;
using Main.Shared;
using PJH.Runtime.Players;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using Debug = Main.Core.Debug;

namespace PJH.Runtime.PlayerPassive.Passives
{
    [CreateAssetMenu(menuName = "SO/Passive/Active/WarpStrikePassive")]
    public class WarpStrikePassiveSO : PassiveSO, IActivePassive, ICooldownPassive
    {
        [field: SerializeField, OdinSerialize] public CooldownPassiveInfo CooldownPassiveInfo { get; set; }

        [VerticalGroup("Top/Right")] [LabelText("🌀 워프 스트라이크 공격 정보")] [OdinSerialize]
        public List<WarpStrikeAttackInfo> warpStrikeAttackInfos = new List<WarpStrikeAttackInfo>();

        [VerticalGroup("Top/Right")] [LabelText("🎥 타겟 선택 중 카메라 FOV")] [Range(30f, 100f)]
        public float cameraFOVWhenSelectingTargeting = 45f;

        [VerticalGroup("Top/Right")] [LabelText("⏱️FOV 전환 시간 (초)")] [SuffixLabel("sec", true)]
        public float changeCameraFOVDuration = 0.3f;

        [VerticalGroup("Top/Right")] [LabelText("⏱️ 타임스케일 변화 시간 (초)")] [SuffixLabel("sec", true)]
        public float changeTimeScaleDuration = 0.5f;

        [VerticalGroup("Top/Right")] [LabelText("⏱️ 타겟 선택 시간(초)")] [SuffixLabel("sec", true)]
        public float targetSelectionTime = 2f;

        [SerializeField] [VerticalGroup("Top/Right")] [LabelText("💥 공격력")]
        private float _power;

        private CancellationTokenSource _targetSelectionTokenSource;

        public void ActivePassive()
        {
            _player.GetCompo<PlayerAnimationTrigger>().OnTriggerPassiveAfterAttack += HandleTriggerPassiveAfterAttack;
        }

        public void DeActivePassive()
        {
            _player.GetCompo<PlayerAnimationTrigger>().OnTriggerPassiveAfterAttack -= HandleTriggerPassiveAfterAttack;
        }

        private async void HandleTriggerPassiveAfterAttack()
        {
            try
            {
                CooldownPassiveInfo.StartCooldownEvent?.Invoke();
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
                await seq.ToUniTask();
                
                _player.GetCompo<PlayerWarpStrike>().EnableWarpStrike(_power, warpStrikeAttackInfos.GetRandom(),
                    () =>
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

                    await UniTask.WaitUntil(() => !Managers.TimeManager.isPaused,
                        cancellationToken: _targetSelectionTokenSource.Token);

                    elapsed += Time.unscaledDeltaTime;
                    await UniTask.Yield(PlayerLoopTiming.Update, _targetSelectionTokenSource.Token);
                }

                EndTargetSelection();
                _player.GetCompo<PlayerWarpStrike>().DisableWarpStrike();
            }
            catch (Exception e)
            {
                Debug.Log($"{e.Message} , {e.StackTrace}");
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