using System;
using Main.Runtime.Core.Events;
using Main.Runtime.Manager;
using PJH.Runtime.Core.InputKeyIcon;
using PJH.Utility.Managers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace PJH.Runtime.UI
{
    public class FinisherTargetUI : MonoBehaviour
    {
        [SerializeField] private InputKeyIconListSO _inputKeyIconList;
        [SerializeField] private InputActionReference _finisherKeyActionReference;
        private GameEventChannelSO _showFinisherTargetUIEventChannel;
        [SerializeField] private Image _inputKeyImage;
        [SerializeField] private GameObject _farKeyIcon;
        [SerializeField] private float _showFarKeyIconDistance = 4;
        private Transform _playerTrm;
        private Transform _finisherTargetTrm;

        private void Awake()
        {
            _showFinisherTargetUIEventChannel = AddressableManager.Load<GameEventChannelSO>("UIEventChannelSO");
            gameObject.SetActive(false);
            _showFinisherTargetUIEventChannel.AddListener<ShowFinisherTargetUI>(HandleShowFinisherTargetUI);
        }

        private void Start()
        {
            _playerTrm = PlayerManager.Instance.Player.GameObject.transform;
        }

        private void OnDestroy()
        {
            _showFinisherTargetUIEventChannel.RemoveListener<ShowFinisherTargetUI>(HandleShowFinisherTargetUI);
        }

        private void HandleShowFinisherTargetUI(ShowFinisherTargetUI evt)
        {
            try
            {
                _finisherTargetTrm = evt.finisherTargetTrm;
                if (evt.isShowUI)
                {
                    string inputKey = _finisherKeyActionReference.action.GetBindingDisplayString();
                    InputKeyIcon inputKeyIcon =
                        _inputKeyIconList.GetInputKeyIcon(inputKey);
                    _inputKeyImage.sprite = inputKeyIcon.keyIcon;
                }

                gameObject.SetActive(evt.isShowUI);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }

        private void LateUpdate()
        {
            transform.GetChild(0).LookAt(Camera.main.transform);
            if (_finisherTargetTrm)
            {
                transform.position = _finisherTargetTrm.position;
                float distance = Vector3.Distance(_finisherTargetTrm.position, _playerTrm.position);
                _farKeyIcon.gameObject.SetActive(distance > _showFarKeyIconDistance);
                _inputKeyImage.gameObject.SetActive(distance <= _showFarKeyIconDistance);
            }
        }
    }
}