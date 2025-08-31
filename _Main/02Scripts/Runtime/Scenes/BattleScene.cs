using BIS.Manager;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using LJS.Map;
using LJS.Utils;
using Main.Core;
using Main.Runtime.Agents;
using Main.Runtime.Core;
using Main.Runtime.Core.Events;
using Main.Runtime.Manager;
using Main.Runtime.Manager.VolumeTypes;
using Main.Shared;
using PJH.Runtime.Core;
using PJH.Runtime.Core.EnemySpawnSystem;
using PJH.Runtime.Players;
using PJH.Runtime.UI;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using YTH.Boss;
using YTH.Enemies;
using Managers = Main.Runtime.Manager.Managers;
using Random = UnityEngine.Random;
using SceneManagerEx = Main.Runtime.Manager.SceneManagerEx;

namespace Main.Scenes
{
    public class BattleScene : BaseScene, IBattleScene
    {
        public event IBattleScene.ChangedBattleZoneControllerEvent OnChangedBattleZoneController;
        [SerializeField] private GameObject _gameSceneUI;
        [SerializeField] private PlayableAsset _appearBossSequence, _bossDeadSequence;
        [SerializeField] private CinemachineCamera _bossDeathTimelineBossCamera;
        [SerializeField] private Vector3 _timelineOffsetFromCenter = new Vector3(0, 0, 0.55f);
        private IBattleZoneController _currentBattleZoneController;

        public IBattleZoneController CurrentBattleZoneController
        {
            get => _currentBattleZoneController;
            set
            {
                IBattleZoneController prevBattleZoneController = _currentBattleZoneController;
                _currentBattleZoneController = value;
                OnChangedBattleZoneController?.Invoke(prevBattleZoneController, _currentBattleZoneController);
            }
        }

        private GameEventChannelSO _uiEventChannel;

        protected GameEventChannelSO _gameEventChannel;

        private PlayableDirector _playableDirector;

        protected override void Awake()
        {
            base.Awake();
            BIS.Manager.Managers.Resource.Load<ComboSynthesisPriceInfoSO>("ComboSynthesisPriceInfo")
                .ResetIncreaseLevel();
            BIS.Manager.Managers.Resource.Load<ShopItemInfoSO>("ShopItemInfo")
                .ResetInfo();
            _playableDirector = FindAnyObjectByType<PlayableDirector>();
            Application.targetFrameRate = 60;
            _gameEventChannel = AddressableManager.Load<GameEventChannelSO>("GameEventChannel");
            _uiEventChannel = AddressableManager.Load<GameEventChannelSO>("UIEventChannelSO");
            AddressableManager.Instantiate<TextDialogueCanvas>("TextDialogueCanvas");
        }

        protected override void Start()
        {
            base.Start();
            _gameEventChannel.AddListener<PlayerDeath>(HandlePlayerDeath);
            _gameEventChannel.AddListener<TimeSlowByPlayer>(HandlePlayerAvoidingAttack);
            _gameEventChannel.AddListener<PlayerStunned>(HandlePlayerStunned);
            _gameEventChannel.AddListener<FinishAllWave>(HandleEndBattle);
            _gameEventChannel.AddListener<StartWave>(HandleStartWave);
            _gameEventChannel.AddListener<BossDead>(HandleBossDead);
            _gameEventChannel.AddListener<EnterBossRoom>(HandleEnterBossRoom);
            _gameEventChannel.AddListener<GoToLobby>(GoToLobby);
            RoomManager.Instance.CurrentRoomChangeActon += HandleRoomChange;

            PlayerManager.Instance.Player.HealthCompo.OnDeath += HandlePlayerDeathEvent;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _gameEventChannel.RemoveListener<PlayerDeath>(HandlePlayerDeath);
            _gameEventChannel.RemoveListener<TimeSlowByPlayer>(HandlePlayerAvoidingAttack);
            _gameEventChannel.RemoveListener<PlayerStunned>(HandlePlayerStunned);
            _gameEventChannel.RemoveListener<FinishAllWave>(HandleEndBattle);
            _gameEventChannel.RemoveListener<StartWave>(HandleStartWave);
            _gameEventChannel.RemoveListener<BossDead>(HandleBossDead);
            _gameEventChannel.RemoveListener<EnterBossRoom>(HandleEnterBossRoom);
            _gameEventChannel.RemoveListener<GoToLobby>(GoToLobby);
            RoomManager.Instance.CurrentRoomChangeActon -= HandleRoomChange;
            Agent player = PlayerManager.Instance.Player;
            if (player != null) player.HealthCompo.OnDeath -= HandlePlayerDeathEvent;
        }

        private void HandlePlayerDeathEvent()
        {
            if (_foundCanvases == null || _foundCanvases.Length <= 0)
                _foundCanvases = FindObjectsByType<Canvas>(FindObjectsSortMode.None);
            for (int i = 0; i < _foundCanvases.Length; i++)
            {
                if (_foundCanvases[i].gameObject.TryGetComponent(out CanvasGroup canvasGroup))
                    canvasGroup.DOFade(0, .5f);
            }
        }

        public void StartBossBattle()
        {
            Sequence seq = DOTween.Sequence();
            seq.Append(Managers.VolumeManager.GetVolumeType<BrightnessVolumeType>().SetValue(0, .5f));
            seq.AppendInterval(.3f);
            seq.AppendCallback(() =>
            {
                var evt = GameEvents.StartBossBattle;
                _gameEventChannel.RaiseEvent(evt);
                _playableDirector.Stop();
                _gameSceneUI.SetActive(true);

                Managers.FMODManager.MusicEventInstance.setParameterByNameWithLabel("MusicType", "InBoss");
                AddressableManager.Instantiate<BossUI>("BossUI");

                SettingForEndTimeline();
                VolumeForEndTimeline();
            });
        }

        private void HandleEnterBossRoom(EnterBossRoom evt)
        {
            SettingForTimeline();
            Sequence seq = DOTween.Sequence();
            seq.Append(Managers.VolumeManager.GetVolumeType<BrightnessVolumeType>().SetValue(0, .5f));
            seq.AppendInterval(1f);
            seq.AppendCallback(() =>
            {
                Managers.VolumeManager.GetVolumeType<BrightnessVolumeType>().SetValue(1, .5f);
                VolumeForTimeline();
                _playableDirector.Play(_appearBossSequence);
            });
        }


        private void HandleRoomChange(RoomComponent room)
        {
            if (room == null)
            {
                Managers.FMODManager.MusicEventInstance.setParameterByNameWithLabel("MusicType", "Default");
            }
            else if (room is SpecialRoomComponent)
            {
                switch (room.SpecialRoomType)
                {
                    case SpecialRoomType.Shop:
                        Managers.FMODManager.MusicEventInstance.setParameterByNameWithLabel("MusicType", "InShop");
                        break;
                }
            }
        }

        private void HandleBossDead(BossDead obj)
        {
            SettingForTimeline();

            Managers.FMODManager.StopMusicSound();
            Sequence seq = DOTween.Sequence();
            seq.Append(Managers.VolumeManager.GetVolumeType<BrightnessVolumeType>().SetValue(0, 1f));
            seq.AppendInterval(1f);
            seq.AppendCallback(async () =>
            {
                var destroyDeadEnemyEvt = GameEvents.DestroyDeadEnemy;
                destroyDeadEnemyEvt.isPlayingBossDeathTimeline = true;
                _gameEventChannel.RaiseEvent(destroyDeadEnemyEvt);
                BossRoomComponent bossRoomComponent = (RoomManager.Instance.CurrentRoom as BossRoomComponent);
                BossBattleZone bossBattleZone = (SceneManagerEx.Instance.CurrentScene as IBattleScene)
                    .CurrentBattleZoneController?.CurrentBattleZone as BossBattleZone;
                Player player = PlayerManager.Instance.Player as Player;
                player.GetComponentInChildren<DistanceFade>().Locked = true;
                player.WarpingComponent.enabled = false;
                PlayerAnimator playerAnimatorCompo = player.GetCompo<PlayerAnimator>();
                playerAnimatorCompo.Animancer.enabled = false;
                PlayerIK playerIKCompo = player.GetCompo<PlayerIK>();
                playerIKCompo.LegsAnimator.enabled = false;
                playerIKCompo.LeaningAnimator.enabled = false;
                playerIKCompo.LookAnimator.enabled = false;
                player.ComponentManager.EnableComponents(false);
                Boss boss = null;
                if (bossRoomComponent)
                {
                    boss = bossRoomComponent.BossObject;
                }
                else
                {
                    boss = bossBattleZone.GetBoss();
                }

                _bossDeathTimelineBossCamera.Target.TrackingTarget = boss.transform.Find("Visual");

                boss.GetComponentInChildren<DistanceFade>().Locked = true;

                boss.WarpingComponent.enabled = false;
                AgentAnimator bossAnimatorCompo = boss.GetCompo<AgentAnimator>(true);
                bossAnimatorCompo.Animancer.enabled = false;
                Animator bossAnimator = bossAnimatorCompo.Animator;
                bossAnimator.enabled = true;
                boss.GetCompo<EnemyMovement>().AIPath.enabled = false;
                boss.BehaviorTreeCompo.enabled = false;
                AgentIK bossIK = boss.GetCompo<AgentIK>();
                if (bossIK.LegsAnimator) bossIK.LegsAnimator.enabled = false;
                if (bossIK.RagdollAnimator) bossIK.RagdollAnimator.enabled = false;
                _playableDirector.playableAsset = _bossDeadSequence;
                var timeline = _playableDirector.playableAsset as TimelineAsset;
                foreach (var track in timeline.GetOutputTracks())
                {
                    if (track.name == "Boss Animation Track")
                    {
                        _playableDirector.SetGenericBinding(track, bossAnimator);
                    }
                }

                boss.ComponentManager.EnableComponents(false);
                Transform centerPoint;
                if (bossRoomComponent)
                {
                    centerPoint = bossRoomComponent.BossDeathTimelinePoint;
                }
                else
                {
                    centerPoint = bossBattleZone.GetBossDeathTimelinePoint();
                }

                player.transform.position = centerPoint.position + _timelineOffsetFromCenter;
                boss.transform.position = centerPoint.position - _timelineOffsetFromCenter;
                await UniTask.Yield();
                player.transform.DOLookAt(boss.transform.position, 0f, AxisConstraint.Y);
                boss.transform.DOLookAt(centerPoint.position, 0f, AxisConstraint.Y);
                VolumeForTimeline();
                _playableDirector.Play();
            });

            seq.Append(Managers.VolumeManager.GetVolumeType<BrightnessVolumeType>().SetValue(1, 1f));
        }

        private void HandleStartWave(StartWave obj)
        {
            if (RoomManager.Instance.CurrentRoom is not BossRoomComponent)
            {
                Managers.FMODManager.MusicEventInstance.setParameterByName("InBattleMusicNumber", Random.Range(0, 2));
                Managers.FMODManager.MusicEventInstance.setParameterByNameWithLabel("MusicType", "InBattle");
            }
        }

        private void HandleEndBattle(FinishAllWave evt)
        {
            Managers.FMODManager.MusicEventInstance.setParameterByNameWithLabel("MusicType", "Default");
        }

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
            _playerInput.EnablePlayerInput(false);
            _playerInput.EnableUIInput(false);
            Sequence seq = DOTween.Sequence();
            seq.AppendCallback(() =>
            {
                var destroyDeadEnemyEvt = GameEvents.DestroyDeadEnemy;
                destroyDeadEnemyEvt.isPlayingBossDeathTimeline = false;
                _gameEventChannel.RaiseEvent(destroyDeadEnemyEvt);
                var showDeathUIEvt = UIEvents.ShowDeathUI;
                showDeathUIEvt.isShowUI = true;
                _uiEventChannel.RaiseEvent(showDeathUIEvt);
            });
            seq.AppendInterval(5);
            seq.AppendCallback(() => { SceneManagerEx.LoadScene("Lobby", true); });
        }

        public void GoToLobby(GoToLobby evt)
        {
            var destroyDeadEnemyEvt = GameEvents.DestroyDeadEnemy;
            destroyDeadEnemyEvt.isPlayingBossDeathTimeline = false;
            _gameEventChannel.RaiseEvent(destroyDeadEnemyEvt);
            Managers.clearedThemeCount++;
            if (Managers.clearedThemeCount >= Managers.maxThemeCount)
            {
                SceneControlManager.FadeOut(async () =>
                {
                    await UniTask.WaitForSeconds(1f, cancellationToken: gameObject.GetCancellationTokenOnDestroy(),
                        ignoreTimeScale: true);
                    AddressableManager.Instantiate("EndingCanvas");
                });
            }
            else
            {
                SceneManagerEx.LoadScene("Lobby", true);
            }
        }

        public void OpenComboPieceLoadoutUI()
        {
            AddressableManager.Instantiate("ComboPieceLoadoutCanvas");
        }
    }
}