using System;
using BIS.Data;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using FMODUnity;
using Main.Runtime.Core.Events;
using Main.Runtime.Manager;
using Main.Runtime.Manager.VolumeTypes;
using Main.Shared;
using PJH.Runtime.Core;
using PJH.Runtime.Core.Cheat;
using PJH.Runtime.Players;
using PJH.Utility.Managers;
using UnityEngine;

namespace Main.Scenes
{
    [DefaultExecutionOrder(100)]
    public class BaseScene : MonoBehaviour, IScene
    {
        [SerializeField] protected bool _autoPlayBGM = true;
        [SerializeField] protected EventReference _bgm;

        protected PlayerInputSO _playerInput;
        protected Canvas[] _foundCanvases;
        protected CheatSO _cheatSO;

        protected virtual void Awake()
        {
            SetData();
            BIS.Manager.Managers.UI.CloseAllPopupUIWithoutCloseMethod();
            Managers.FMODManager.SetBeforePlayerDead(false);
        }

        private async void SetData()
        {
            try
            {
                await UniTask.WaitUntil(() => AddressableManager.isLoaded,
                    cancellationToken: gameObject.GetCancellationTokenOnDestroy());
                _cheatSO = AddressableManager.Load<CheatSO>("CheatSO");
                _cheatSO.money = AddressableManager.Load<CurrencySO>("Money");
                _playerInput = AddressableManager.Load<PlayerInputSO>("PlayerInputSO");
                _playerInput.ResetPreventVariable();
                _playerInput.EnablePlayerInput(true);
                _playerInput.EnableUIInput(true);
                Managers.VolumeManager.FindVolumeComponent();
                GameEvents.ChangeCurrentEnemy.enemyCount = 99;
                GameEventChannelSO gameEventChannel = AddressableManager.Load<GameEventChannelSO>("GameEventChannel");
                gameEventChannel.AddListener<TimeSlowByPlayer>(HandlePlayerAvoidingAttack);
                gameEventChannel.AddListener<PlayerStunned>(HandlePlayerStunned);
            }
            catch (Exception e)
            {
                // ignored
            }
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

        protected virtual void Start()
        {
            Managers.FMODManager.SetBeforePlayerDead(false);
            Managers.FMODManager.ResumeMainSound();
            Managers.FMODManager.SetGameSoundVolume(1);
            Time.timeScale = 1;
            CursorManager.SetCursorLockMode(CursorLockMode.Locked);
            if (_autoPlayBGM)
            {
                PlayBGM();
            }

            BIS.Manager.Managers.Save.LoadGame();
        }

        protected virtual void Update()
        {
            _cheatSO?.Update();
        }

        protected void PlayBGM()
        {
            if (!_bgm.IsNull)
            {
                Managers.FMODManager.PlayMusicSound(_bgm);
            }
        }

        protected virtual void OnDestroy()
        {
            GameEventChannelSO gameEventChannel = AddressableManager.Load<GameEventChannelSO>("GameEventChannel");
            if (gameEventChannel)
            {
                gameEventChannel.RemoveListener<TimeSlowByPlayer>(HandlePlayerAvoidingAttack);
                gameEventChannel.RemoveListener<PlayerStunned>(HandlePlayerStunned);
            }

            BIS.Manager.Managers.Save.SaveGame();
        }

        public void SettingForTimeline()
        {
            Managers.VolumeManager.GetVolumeType<SepiaVolumeType>().SetValue(0f, .5f);
            Managers.VolumeManager.GetVolumeType<ChromaticAberrationVolumeType>().SetValue(0f, 0.5f);
            FrameVolumeType frameVolumeType = Managers.VolumeManager.GetVolumeType<FrameVolumeType>();
            frameVolumeType.ChangeToCinematicBands();
            frameVolumeType.SetValue(.1f, 1f);
            Managers.FMODManager.PauseMainSound();
            _playerInput.EnablePlayerInput(false);
            _playerInput.EnableUIInput(false);

            _foundCanvases = FindObjectsByType<Canvas>(FindObjectsSortMode.None);
            for (int i = 0; i < _foundCanvases.Length; i++)
            {
                if (_foundCanvases[i].gameObject.CompareTag("IgnoreTimelineUI")) continue;
                _foundCanvases[i].gameObject.SetActive(false);
            }
        }

        public void SettingForEndTimeline()
        {
            Managers.FMODManager.ResumeMainSound();
            _playerInput.EnablePlayerInput(true);
            _playerInput.EnableUIInput(true);

            bool isBeforeDead = (PlayerManager.Instance.Player.HealthCompo as PlayerHealth).IsBeforeDead;
            FrameVolumeType frameVolumeType = Managers.VolumeManager.GetVolumeType<FrameVolumeType>();
            frameVolumeType.ChangeToCinematicBands();
            frameVolumeType.SetValue(.1f);
            frameVolumeType.SetValue(0f, 1f).OnComplete(() => { frameVolumeType.ChangeToBorder(); });
            Managers.VolumeManager.GetVolumeType<SepiaVolumeType>().SetValue(isBeforeDead ? .7f : 0f, .5f);
            Managers.VolumeManager.GetVolumeType<BrightnessVolumeType>().ResetValue();

            for (int i = 0; i < _foundCanvases.Length; i++)
            {
                if (_foundCanvases[i].gameObject.CompareTag("PersistentUI"))
                    _foundCanvases[i].gameObject.SetActive(true);
            }
        }
    }
}