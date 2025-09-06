using Main.Runtime.Agents;
using Main.Runtime.Core.Events;
using Main.Runtime.Manager;
using Main.Shared;
using PJH.Runtime.Players;
using PJH.Utility.Managers;
using UnityEngine;

namespace PJH.Runtime.UI
{
    public class LockOnUI : MonoBehaviour
    {
        private GameEventChannelSO _uiEventChannel;

        private ILockOnAble _lockOnTarget;


        private bool _isLockOn;
        private GameObject _groupTrm;

        private void Awake()
        {
            _groupTrm = transform.GetChild(0).gameObject;
            _uiEventChannel = AddressableManager.Load<GameEventChannelSO>("UIEventChannelSO");
        }

        private void Start()
        {
            Player player = PlayerManager.Instance.Player as Player;
            player.OnLockOn += HandleLockOn;
            PlayerEnemyFinisher finisherCompo = player.GetCompo<PlayerEnemyFinisher>();
            finisherCompo.OnFinisher += HandleFinisher;
            finisherCompo.OnFinisherEnd += HandleFinisherEnd;
            _isLockOn = player.IsLockOn;
            _uiEventChannel.AddListener<ShowLockOnUI>(HandleShowLockOnUI);
            _groupTrm.SetActive(false);
        }

        private void OnDestroy()
        {
            if (PlayerManager.Instance)
            {
                Agent player = PlayerManager.Instance.Player;
                if (player)
                {
                    (player as Player).OnLockOn -= HandleLockOn;
                    PlayerEnemyFinisher finisherCompo = player.GetCompo<PlayerEnemyFinisher>();
                    finisherCompo.OnFinisher -= HandleFinisher;
                    finisherCompo.OnFinisherEnd -= HandleFinisherEnd;
                }
            }

            _uiEventChannel.RemoveListener<ShowLockOnUI>(HandleShowLockOnUI);
        }

        private void HandleFinisherEnd()
        {
            gameObject.SetActive(true);
        }

        private void HandleFinisher()
        {
            gameObject.SetActive(false);
        }

        private void HandleLockOn(bool isLockOn)
        {
            _isLockOn = isLockOn;
            if (_lockOnTarget != null)
                _groupTrm.SetActive(isLockOn);
            else
                _groupTrm.SetActive(false);
        }

        private void HandleShowLockOnUI(ShowLockOnUI evt)
        {
            _lockOnTarget = evt.lockOnTarget;
            if (!_isLockOn) return;
            _groupTrm.gameObject.SetActive(evt.isShowUI);
        }

        private void LateUpdate()
        {
            if (!_groupTrm.gameObject.activeSelf) return;
            _groupTrm.transform.LookAt(Camera.main.transform);
            if (_lockOnTarget == null) return;
            transform.position = _lockOnTarget.LockOnUIDisplayTargetTrm.position;
        }
    }
}