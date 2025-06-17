using BIS.UI.Popup;
using DG.Tweening;
using Main.Core;
using Main.Runtime.Core.Events;
using Main.Runtime.Manager;
using Main.Runtime.Manager.VolumeTypes;
using PJH.Runtime.Players;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Managers = Main.Runtime.Manager.Managers;

namespace Main.Scenes
{
    public class BattleScene : BaseScene
    {
        [SerializeField] private PoolTypeSO _rewardChestPoolType;
        private PoolManagerSO _poolManager;
        private GameEventChannelSO _uiEventChannel;
        private GameEventChannelSO _gameEventChannel;
        [SerializeField] private PlayableDirector _playableDirector;
        [SerializeField] private Transform _rewardCehstPointTrm;

        private void Awake()
        {
            Application.targetFrameRate = 60;

            _poolManager = AddressableManager.Load<PoolManagerSO>("PoolManager");
            _gameEventChannel = AddressableManager.Load<GameEventChannelSO>("GameEventChannel");
            _uiEventChannel = AddressableManager.Load<GameEventChannelSO>("UIEventChannelSO");
        }

        protected override void Start()
        {
            base.Start();
            _gameEventChannel.AddListener<EnemyFinisherSequence>(HandleEnemyFinisherSequence);
            _gameEventChannel.AddListener<PlayerDeath>(HandlePlayerDeath);
            _gameEventChannel.AddListener<TimeSlowByPlayer>(HandlePlayerAvoidingAttack);
            _gameEventChannel.AddListener<PlayerStunned>(HandlePlayerStunned);
            // _gameEventChannel.AddListener<FinishAllWave>(HandleFinishAllWave);
            _gameEventChannel.AddListener<EndBattle>(HandleEndBattle);
        }

        private void OnDestroy()
        {
            _gameEventChannel.RemoveListener<EnemyFinisherSequence>(HandleEnemyFinisherSequence);
            _gameEventChannel.RemoveListener<PlayerDeath>(HandlePlayerDeath);
            _gameEventChannel.RemoveListener<TimeSlowByPlayer>(HandlePlayerAvoidingAttack);
            _gameEventChannel.RemoveListener<PlayerStunned>(HandlePlayerStunned);
            // _gameEventChannel.RemoveListener<FinishAllWave>(HandleFinishAllWave);
            _gameEventChannel.RemoveListener<EndBattle>(HandleEndBattle);
        }

        private void HandleEndBattle(EndBattle evt)
        {
            BIS.Manager.Managers.UI.ShowPopup<RankingUpPopupUI>();
        }

        // private void HandleFinishAllWave(FinishAllWave evt)
        // {
        //     Managers.FMODManager.MusicEventInstance.setParameterByNameWithLabel("MusicType", "EndBattle");
        //
        //     Transform _rewardChest = _poolManager.Pop(_rewardChestPoolType).GameObject.transform;
        //     _rewardChest.position = _rewardCehstPointTrm.position;
        // }

        private void HandlePlayerStunned(PlayerStunned evt)
        {
            if (evt.isStunned)
            {
                float duration = .4f;
                Managers.VolumeManager.GetVolumeType<SaturateVolumeType>().SetValue(-2, duration);
                Managers.VolumeManager.GetVolumeType<BrightnessVolumeType>().SetValue(.3f, duration);
                Managers.VolumeManager.GetVolumeType<VignettingFadeVolumeType>().SetValue(.4f, duration);
            }
            else
            {
                float duration = .2f;
                Managers.VolumeManager.GetVolumeType<SaturateVolumeType>().ResetValue(duration);
                Managers.VolumeManager.GetVolumeType<BrightnessVolumeType>().ResetValue(duration);
                Managers.VolumeManager.GetVolumeType<VignettingFadeVolumeType>().ResetValue(duration);
            }
        }

        private void HandlePlayerAvoidingAttack(TimeSlowByPlayer evt)
        {
            if (evt.isEnabledEffect)
            {
                float duration = .2f;
                Managers.VolumeManager.GetVolumeType<SaturateVolumeType>().SetValue(-2, 2f);
                Managers.VolumeManager.GetVolumeType<SepiaVolumeType>().SetValue(-2, 2f);
            }
            else
            {
                float duration = .2f;
                Managers.VolumeManager.GetVolumeType<SaturateVolumeType>().ResetValue(duration);
                Managers.VolumeManager.GetVolumeType<SepiaVolumeType>().ResetValue(duration);
            }
        }

        private void HandlePlayerDeath(PlayerDeath evt)
        {
            Sequence seq = DOTween.Sequence();
            seq.AppendCallback(() =>
            {
                var destroyDeadEnemyEvt = GameEvents.DestroyDeadEnemy;
                _gameEventChannel.RaiseEvent(destroyDeadEnemyEvt);
                var showDeathUIEvt = UIEvents.ShowDeathUI;
                showDeathUIEvt.isShowUI = true;
                _uiEventChannel.RaiseEvent(showDeathUIEvt);
            });
            seq.AppendInterval(5);
            seq.AppendCallback(() => { SceneManagerEx.LoadScene("Lobby", true); });
        }

        private void HandleEnemyFinisherSequence(EnemyFinisherSequence evt)
        {
            PlayableAsset playableAsset = evt.sequenceAsset;
            _playableDirector.playableAsset = playableAsset;
            var timeline = playableAsset as TimelineAsset;
            foreach (var track in timeline.GetOutputTracks())
            {
                if (track.name == "Enemy Animation Track")
                {
                    _playableDirector.SetGenericBinding(track, evt.enemyAnimator);
                    break;
                }
            }

            _playableDirector.Play();
        }

        public void FinishTimelineSignal()
        {
            _playableDirector.Stop();
            FinishTimeline evt = GameEvents.FinishTimeline;
            _gameEventChannel.RaiseEvent(evt);
        }

        public void DeadFinisherTargetSignal()
        {
            DeadFinisherTarget evt = GameEvents.DeadFinisherTarget;
            _gameEventChannel.RaiseEvent(evt);
        }

        public void ReOffsetPlayerSignal()
        {
            ReOffsetPlayer evt = GameEvents.ReOffsetPlayer;
            _gameEventChannel.RaiseEvent(evt);
        }
    }
}