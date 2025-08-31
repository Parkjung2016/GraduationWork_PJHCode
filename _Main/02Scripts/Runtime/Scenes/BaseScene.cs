using System;
using BIS.Data;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using FMODUnity;
using Main.Core;
using Main.Runtime.Manager;
using Main.Runtime.Manager.VolumeTypes;
using Main.Shared;
using PJH.Runtime.Core;
using PJH.Runtime.Players;
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
                await UniTask.WaitUntil(() => AddressableManager.IsLoaded,
                    cancellationToken: gameObject.GetCancellationTokenOnDestroy());
                _cheatSO = AddressableManager.Load<CheatSO>("CheatSO");
                _cheatSO.money = AddressableManager.Load<CurrencySO>("Money");
                _playerInput = AddressableManager.Load<PlayerInputSO>("PlayerInputSO");
                _playerInput.EnablePlayerInput(true);
                _playerInput.EnableUIInput(true);
            }
            catch (Exception e)
            {
                // ignored
            }
        }

        protected virtual void Start()
        {
            Managers.FMODManager.SetBeforePlayerDead(false);
            Managers.FMODManager.ResumeMainSound();
            Managers.FMODManager.SetGameSoundVolume(1);
            Time.timeScale = 1;
            CursorManager.EnableCursor(false);
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
            BIS.Manager.Managers.Save.SaveGame();
        }

        public void SettingForTimeline()
        {
            Managers.VolumeManager.GetVolumeType<SepiaVolumeType>().SetValue(0f, .5f);
            Managers.VolumeManager.GetVolumeType<ChromaticAberrationVolumeType>().SetValue(0f, 0.5f);
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

        public void VolumeForTimeline()
        {
            FrameVolumeType frameVolumeType = Managers.VolumeManager.GetVolumeType<FrameVolumeType>();
            frameVolumeType.ChangeToCinematicBands();
            frameVolumeType.SetValue(.1f, 1f);
        }

        public void VolumeForEndTimeline()
        {
            FrameVolumeType frameVolumeType = Managers.VolumeManager.GetVolumeType<FrameVolumeType>();
            frameVolumeType.SetValue(0f, 1f).OnComplete(() => { frameVolumeType.ChangeToBorder(); });
        }

        public void SettingForEndTimeline()
        {
            Managers.FMODManager.ResumeMainSound();
            _playerInput.EnablePlayerInput(true);
            _playerInput.EnableUIInput(true);

            bool isBeforeDead = (PlayerManager.Instance.Player.HealthCompo as PlayerHealth).IsBeforeDead;
            FrameVolumeType frameVolumeType = Managers.VolumeManager.GetVolumeType<FrameVolumeType>();
            frameVolumeType.SetValue(0f, 1f).OnComplete(() => { frameVolumeType.ChangeToBorder(); });
            Managers.VolumeManager.GetVolumeType<SepiaVolumeType>().SetValue(isBeforeDead ? .7f : 0f, .5f);
            Managers.VolumeManager.GetVolumeType<BrightnessVolumeType>().SetValue(1, .5f);

            for (int i = 0; i < _foundCanvases.Length; i++)
            {
                if (_foundCanvases[i].gameObject.CompareTag("PersistentUI"))
                    _foundCanvases[i].gameObject.SetActive(true);
            }
        }
    }
}