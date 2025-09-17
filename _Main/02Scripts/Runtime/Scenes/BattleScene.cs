using BIS.Data;
using BIS.Events;
using BIS.Manager;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using LJS.Map;
using LJS.Utils;
using Main.Runtime.Agents;
using Main.Runtime.Core.Events;
using Main.Runtime.Manager;
using Main.Runtime.Manager.VolumeTypes;
using Main.Shared;
using PJH.Runtime.Core;
using PJH.Runtime.Players;
using PJH.Runtime.UI;
using PJH.Utility.Managers;
using UnityEngine;
using UnityEngine.Playables;
using ZLinq;
using Managers = Main.Runtime.Manager.Managers;
using Random = UnityEngine.Random;
using SceneManagerEx = Main.Runtime.Manager.SceneManagerEx;

namespace Main.Scenes
{
    public class BattleScene : BaseScene, IBattleScene
    {
        public bool IsInBattle { get; private set; }
        public event IBattleScene.ChangedBattleZoneControllerEvent OnChangedBattleZoneController;
        [SerializeField] private GameObject _gameSceneUI;
        [SerializeField] private PlayableAsset _appearBossSequence, _bossDeadSequence;
        private IBattleZoneController _currentBattleZoneController;

        private bool _deathBoss;

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
        private InventorySO _inventory;

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
            _inventory = AddressableManager.Load<InventorySO>("InventorySO");
            _uiEventChannel = AddressableManager.Load<GameEventChannelSO>("UIEventChannelSO");
            AddressableManager.Instantiate<TextDialogueCanvas>("TextDialogueCanvas");
        }

        protected override void Start()
        {
            base.Start();
            _gameEventChannel.AddListener<PlayerDeath>(HandlePlayerDeath);
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
                PlayerCommandActionManager commandActionManagerCompo =
                    PlayerManager.Instance.Player.GetCompo<PlayerCommandActionManager>();
                commandActionManagerCompo.ClearAllBuffStat();
                commandActionManagerCompo.ClearAllCooldown();
                Managers.VolumeManager.GetVolumeType<BrightnessVolumeType>().SetValue(1, .5f);
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

                switch (room.SpecialRoomType)
                {
                    case SpecialRoomType.Choice:
                        Managers.FMODManager.MusicEventInstance.setParameterByNameWithLabel("MusicType", "InHealing");
                        break;
                }
            }
        }

        private void HandleBossDead(BossDead obj)
        {
            SettingForTimeline();

            _deathBoss = true;
            Managers.FMODManager.StopMusicSound();
            Sequence seq = DOTween.Sequence();
            seq.Append(Managers.VolumeManager.GetVolumeType<BrightnessVolumeType>().SetValue(0, 1f));
            seq.AppendInterval(1f);
            seq.AppendCallback(() =>
            {
                var destroyDeadEnemyEvt = GameEvents.DestroyDeadEnemy;
                destroyDeadEnemyEvt.isPlayingBossDeathTimeline = true;
                _gameEventChannel.RaiseEvent(destroyDeadEnemyEvt);
                _playableDirector.playableAsset = _bossDeadSequence;
                _playableDirector.Play();
            });

            seq.Append(Managers.VolumeManager.GetVolumeType<BrightnessVolumeType>().SetValue(1, 1f));
        }

        private void HandleStartWave(StartWave obj)
        {
            IsInBattle = true;
            if (RoomManager.Instance.CurrentRoom is not BossRoomComponent)
            {
                Managers.FMODManager.MusicEventInstance.setParameterByName("InBattleMusicNumber", Random.Range(0, 2));
                Managers.FMODManager.MusicEventInstance.setParameterByNameWithLabel("MusicType", "InBattle");
            }
        }

        private void HandleEndBattle(FinishAllWave evt)
        {
            IsInBattle = false;
            Managers.FMODManager.MusicEventInstance.setParameterByNameWithLabel("MusicType", "Default");
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
            seq.AppendCallback(() => { OpenComboPieceLoadoutUI(); });
        }

        public void GoToLobby(GoToLobby evt)
        {
            var destroyDeadEnemyEvt = GameEvents.DestroyDeadEnemy;
            destroyDeadEnemyEvt.isPlayingBossDeathTimeline = false;
            _gameEventChannel.RaiseEvent(destroyDeadEnemyEvt);
            string themeName = UIEvent.ThemePopupChoiceEvent.themeSO.ThemeName;
            ThemeSettingSO themeSetting = AddressableManager.Load<ThemeSettingSO>("ThemeSetting");

            if (_deathBoss)
                themeSetting.clearedTheme[themeName] = true;
            if (!themeSetting.isShowedEnding && themeSetting.clearedTheme.Count >= themeSetting.maxThemeCount &&
                themeSetting.clearedTheme.Values.AsValueEnumerable().All(x => x == true))
            {
                SceneControlManager.FadeOut(async () =>
                {
                    await UniTask.WaitForSeconds(1f, cancellationToken: gameObject.GetCancellationTokenOnDestroy(),
                        ignoreTimeScale: true);
                    themeSetting.isShowedEnding = true;
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