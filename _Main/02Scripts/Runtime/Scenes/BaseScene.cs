using System;
using Cysharp.Threading.Tasks;
using FMODUnity;
using Main.Core;
using Main.Runtime.Manager;
using Main.Shared;
using PJH.Runtime.Players;
using UnityEngine;

namespace Main.Scenes
{
    [DefaultExecutionOrder(100)]
    public class BaseScene : MonoBehaviour, IScene
    {
        protected PlayerInputSO _playerInput;
        [SerializeField] protected bool _autoPlayBGM = true;
        [SerializeField] protected EventReference _bgm;

        protected virtual void Awake()
        {
            SetPlayerInput();
            BIS.Manager.Managers.UI.PopupStackClear();
            Managers.FMODManager.SetBeforePlayerDead(false);
        }

        private async void SetPlayerInput()
        {
            try
            {
                await UniTask.WaitUntil(() => AddressableManager.IsLoaded,
                    cancellationToken: gameObject.GetCancellationTokenOnDestroy());
                _playerInput = AddressableManager.Load<PlayerInputSO>("PlayerInputSO");
                _playerInput.EnablePlayerInput(true);
                _playerInput.EnableUIInput(true);
            }
            catch (Exception e)
            {
            }
        }

        protected virtual void Start()
        {
            Managers.FMODManager.SetBeforePlayerDead(false);
            Managers.FMODManager.SetGameSoundVolume(1);
            Time.timeScale = 1;
            CursorManager.EnableCursor(false);
            if (_autoPlayBGM)
            {
                PlayBGM();
            }

            BIS.Manager.Managers.Save.LoadGame();
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
    }
}