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
        [SerializeField] protected EventReference _bgm;

        protected virtual void Awake()
        {
            SetPlayerInput();
            BIS.Manager.Managers.UI.PopupStackClear();
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
            Time.timeScale = 1;
            CursorManager.EnableCursor(false);
            if (!_bgm.IsNull)
            {
                Managers.FMODManager.PlayMusicSound(_bgm);
            }

            BIS.Manager.Managers.Save.LoadGame();
        }

        protected virtual void OnDestroy()
        {
            BIS.Manager.Managers.Save.SaveGame();
        }
    }
}