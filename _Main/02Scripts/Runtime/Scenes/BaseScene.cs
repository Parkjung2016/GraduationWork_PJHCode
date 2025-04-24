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
        [SerializeField] protected PlayerInputSO _playerInput;
        [SerializeField] protected EventReference _bgm;

        private void Awake()
        {
            BIS.Manager.Managers.UI.PopupStackClear();
        }

        protected virtual void Start()
        {
            Time.timeScale = 1;
            _playerInput.EnablePlayerInput(true);
            CursorManager.EnableCursor(false);
            _playerInput.EnablePlayerInput(true);
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