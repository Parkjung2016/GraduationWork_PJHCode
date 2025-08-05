using System;
using Cysharp.Threading.Tasks;
using Febucci.UI;
using Main.Core;
using Main.Runtime.Core.Events;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace PJH.Runtime.UI
{
    public class TextDialogueCanvas : MonoBehaviour
    {
        private TextMeshProUGUI _dialogueText;
        private TypewriterByCharacter _typeWriter;

        private GameEventChannelSO _uiEventChannel;

        private void Awake()
        {
            _uiEventChannel = AddressableManager.Load<GameEventChannelSO>("UIEventChannelSO");
            _dialogueText = transform.Find("DialogueText").GetComponent<TextMeshProUGUI>();
            _dialogueText.SetText(string.Empty);
            _typeWriter = _dialogueText.GetComponent<TypewriterByCharacter>();
            _typeWriter.onTextShowed.AddListener(HandleTextShowed);
            _typeWriter.onTextDisappeared.AddListener(HandleTextDisappeared);
            _uiEventChannel.AddListener<ShowTextDialogueUI>(HandleShowTextDialogue);
            HandleTextDisappeared();
        }

        private void OnDestroy()
        {
            _typeWriter.onTextShowed.RemoveListener(HandleTextShowed);
            _typeWriter.onTextDisappeared.RemoveListener(HandleTextDisappeared);
            _uiEventChannel.RemoveListener<ShowTextDialogueUI>(HandleShowTextDialogue);
        }

        private void HandleTextDisappeared()
        {
            gameObject.SetActive(false);
        }

        private void HandleShowTextDialogue(ShowTextDialogueUI evt)
        {
            gameObject.SetActive(true);
            _dialogueText.SetText(evt.dialogueText);
        }

        private async void HandleTextShowed()
        {
            try
            {
                await UniTask.WaitForSeconds(2f, cancellationToken: gameObject.GetCancellationTokenOnDestroy());
                _typeWriter.StartDisappearingText();
            }
            catch (Exception e)
            {
            }
        }


#if UNITY_EDITOR
        [Button]
        private void TestDialogueButton(string text)
        {
            _dialogueText.SetText(text);
        }
#endif
    }
}