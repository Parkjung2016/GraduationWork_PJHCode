using Main.Core;
using Main.Runtime.Core.Events;
using Main.Runtime.Manager;
using Main.Shared;
using PJH.Runtime.Players;
using UnityEngine;

namespace PJH.Runtime.UI
{
    public class LockOnUI : MonoBehaviour
    {
        private GameEventChannelSO _uiEventChannel;

        private ILockOnAble _lockOnTarget;


        private void Awake()
        {
            _uiEventChannel = AddressableManager.Load<GameEventChannelSO>("UIEventChannelSO");
        }

        private void Start()
        {
            Player player = PlayerManager.Instance.Player as Player;
            player.OnLockOn += HandleLockOn;
            _uiEventChannel.AddListener<ShowLockOnUI>(HandleShowLockOnUI);
        }

        private void OnDestroy()
        {
            if (PlayerManager.Instance)
            {
                Player player = PlayerManager.Instance.Player as Player;
                player.OnLockOn -= HandleLockOn;
            }

            _uiEventChannel.RemoveListener<ShowLockOnUI>(HandleShowLockOnUI);
        }

        private void HandleLockOn(bool isLockOn)
        {
            gameObject.SetActive(isLockOn);
        }

        private void HandleShowLockOnUI(ShowLockOnUI evt)
        {
            gameObject.SetActive(evt.isShowUI);
            _lockOnTarget = evt.lockOnTarget;
        }

        private void LateUpdate()
        {
            transform.GetChild(0).LookAt(Camera.main.transform);
            if (_lockOnTarget == null) return;
            transform.position = _lockOnTarget.GameObject.transform.position + _lockOnTarget.AdditionalUIDisplayPos;
        }
    }
}