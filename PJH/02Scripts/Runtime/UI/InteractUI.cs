using System;
using Main.Core;
using Main.Runtime.Core.Events;
using Main.Runtime.Manager;
using Main.Shared;
using PJH.Runtime.Core.InputKeyIcon;
using PJH.Runtime.Players;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace PJH.Runtime.UI
{
    public class InteractUI : MonoBehaviour
    {
        [SerializeField] private InputKeyIconListSO _inputKeyIconList;
        [SerializeField] private InputActionReference _interactKeyActionReference;
        private GameEventChannelSO _showInteractUIEventChannel;
        [SerializeField] private TextMeshProUGUI _descriptionText;
        [SerializeField] private Image _inputKeyImage;

        private IInteractable _interactableTarget;
        private Transform _groupTrm;

        private void Awake()
        {
            _groupTrm = transform.GetChild(0);
            _showInteractUIEventChannel = AddressableManager.Load<GameEventChannelSO>("UIEventChannelSO");
            _groupTrm.gameObject.SetActive(false);
            _showInteractUIEventChannel.AddListener<ShowInteractUI>(HandleShowInteractUI);
        }

        private void Start()
        {
            PlayerEnemyFinisher enemyFinisherCompo = PlayerManager.Instance.Player.GetCompo<PlayerEnemyFinisher>();
            enemyFinisherCompo.OnFinisher += HandleFinisher;
            enemyFinisherCompo.OnFinisherEnd += HandleFinisherEnd;
        }

        private void OnDestroy()
        {
            _showInteractUIEventChannel.RemoveListener<ShowInteractUI>(HandleShowInteractUI);

            PlayerEnemyFinisher enemyFinisherCompo = PlayerManager.Instance.Player?.GetCompo<PlayerEnemyFinisher>();
            if (enemyFinisherCompo)
            {
                enemyFinisherCompo.OnFinisher -= HandleFinisher;
                enemyFinisherCompo.OnFinisherEnd -= HandleFinisherEnd;
            }
        }

        private void HandleFinisherEnd()
        {
            gameObject.SetActive(true);
        }

        private void HandleFinisher()
        {
            gameObject.SetActive(false);
        }


        private void HandleShowInteractUI(ShowInteractUI evt)
        {
            _interactableTarget = evt.interactableTarget;
            if (evt.isShowUI)
            {
                _descriptionText.text = evt.interactDescription;
                string inputKey = _interactKeyActionReference.action.GetBindingDisplayString();
                InputKeyIcon inputKeyIcon = _inputKeyIconList.GetInputKeyIcon(inputKey);
                _inputKeyImage.sprite = inputKeyIcon.keyIcon;
            }

            _groupTrm.gameObject.SetActive(evt.isShowUI);
        }

        private void LateUpdate()
        {
            if (!_groupTrm.gameObject.activeSelf) return;
            transform.GetChild(0).LookAt(Camera.main.transform);
            if (_interactableTarget == null) return;
            if (_interactableTarget?.UIDisplayTrm)
            {
                transform.position = _interactableTarget.UIDisplayTrm.position +
                                     _interactableTarget.AdditionalUIDisplayPos;
            }
        }
    }
}