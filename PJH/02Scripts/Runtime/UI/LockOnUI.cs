using Main.Core;
using Main.Runtime.Agents;
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


        private bool _isLockOn;

        private void Awake()
        {
            _uiEventChannel = AddressableManager.Load<GameEventChannelSO>("UIEventChannelSO");
        }

        private void Start()
        {
            Player player = PlayerManager.Instance.Player as Player;
            player.OnLockOn += HandleLockOn;
            HandleLockOn(player.IsLockOn);
            _uiEventChannel.AddListener<ShowLockOnUI>(HandleShowLockOnUI);
        }

        private void OnDestroy()
        {
            if (PlayerManager.Instance)
            {
                Agent player = PlayerManager.Instance.Player;
                if (player != null)
                    (player as Player).OnLockOn -= HandleLockOn;
            }

            _uiEventChannel.RemoveListener<ShowLockOnUI>(HandleShowLockOnUI);
        }

        private void HandleLockOn(bool isLockOn)
        {
            _isLockOn = isLockOn;
            if (_lockOnTarget != null)
                gameObject.SetActive(isLockOn);
        }

        private void HandleShowLockOnUI(ShowLockOnUI evt)
        {
            if (!_isLockOn) return;
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