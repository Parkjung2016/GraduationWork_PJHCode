using Main.Core;
using Main.Runtime.Core.Events;
using Main.Shared;
using PJH.Runtime.Core.InputKeyIcon;
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

        private void Awake()
        {
            _showInteractUIEventChannel = AddressableManager.Load<GameEventChannelSO>("UIEventChannelSO");
            gameObject.SetActive(false);
            _showInteractUIEventChannel.AddListener<ShowInteractUI>(HandleShowInteractUI);
        }

        private void OnDestroy()
        {
            _showInteractUIEventChannel.RemoveListener<ShowInteractUI>(HandleShowInteractUI);
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

            gameObject.SetActive(evt.isShowUI);
        }

        private void LateUpdate()
        {
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