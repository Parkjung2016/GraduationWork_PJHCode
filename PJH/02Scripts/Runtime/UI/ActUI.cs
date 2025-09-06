using Main.Runtime.Core.Events;
using PJH.Runtime.Core.InputKeyIcon;
using PJH.Utility.Managers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace PJH.Runtime.UI
{
    public class ActUI : MonoBehaviour
    {
        [SerializeField] private InputKeyIconListSO _inputKeyIconList;
        private GameEventChannelSO _showActUIEventChannel;
        [SerializeField] private Image _inputKeyImage;

        private Transform _displayTargetTrm;
        private Transform _groupTrm;

        private void Awake()
        {
            _groupTrm = transform.GetChild(0);
            _showActUIEventChannel = AddressableManager.Load<GameEventChannelSO>("UIEventChannelSO");
            _groupTrm.gameObject.SetActive(false);
            _showActUIEventChannel.AddListener<ShowActUI>(HandleShowActUI);
        }

        private void OnDestroy()
        {
            _showActUIEventChannel.RemoveListener<ShowActUI>(HandleShowActUI);
        }

        private void HandleShowActUI(ShowActUI evt)
        {
            if (evt.isShowUI)
            {
                _displayTargetTrm = evt.displayTargetTrm;
                string inputKey = evt.actInputActionReference.action.GetBindingDisplayString(0,
                    InputBinding.DisplayStringOptions.DontIncludeInteractions);
                InputKeyIcon inputKeyIcon =
                    _inputKeyIconList.GetInputKeyIcon(inputKey);
                _inputKeyImage.sprite = inputKeyIcon.keyIcon;
            }

            _groupTrm.gameObject.SetActive(evt.isShowUI);
        }

        private void LateUpdate()
        {
            if (!_groupTrm.gameObject.activeSelf) return;
            transform.GetChild(0).LookAt(Camera.main.transform);
            if (_displayTargetTrm)
            {
                transform.position = _displayTargetTrm.position;
            }
        }
    }
}