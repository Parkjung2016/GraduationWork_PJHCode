using DG.Tweening;
using Main.Runtime.Core.Events;
using Main.Runtime.Manager;
using MoreMountains.Feedbacks;
using PJH.Utility.Extensions;
using PJH.Utility.Managers;
using TMPro;
using TransitionsPlus;
using UnityEngine;

namespace PJH.Runtime.UI
{
    public class DeathUI : MonoBehaviour
    {
        private GameEventChannelSO _showDeathUIEventChannel;
        [SerializeField] private MMF_Player _showFeedbackPlayer;
        [SerializeField] private TextMeshProUGUI _deathTMP;
        [SerializeField] private string[] _deathTexts;
        private CanvasGroup _canvasGroup;
        private TransitionAnimator _transitionAnimator;

        private void Awake()
        {
            _transitionAnimator = GetComponent<TransitionAnimator>();
            _showDeathUIEventChannel = AddressableManager.Load<GameEventChannelSO>("UIEventChannelSO");
            Transform deathUITrm = transform.Find("Transition Root/DeathUI");
            _canvasGroup = deathUITrm.GetComponent<CanvasGroup>();
            _showDeathUIEventChannel.AddListener<ShowDeathUI>(HandleShowDeathUI);
            _canvasGroup.alpha = 0;
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            _showDeathUIEventChannel.RemoveListener<ShowDeathUI>(HandleShowDeathUI);
        }

        private void HandleShowDeathUI(ShowDeathUI evt)
        {
            gameObject.SetActive(true);
            _transitionAnimator.Play();
        }

        public void ShowDeathUI()
        {
            ShowDeathUI showDeathUIEvt = UIEvents.ShowDeathUI;
            if (showDeathUIEvt.isShowUI)
            {
                Managers.FMODManager.PlaySound("event:/UI/DeathUI");
                _deathTMP.SetText(_deathTexts.Random());
                _canvasGroup.DOFade(1, 1);
                _showFeedbackPlayer.PlayFeedbacks();
            }
            else
            {
                _canvasGroup.DOFade(0, 1).OnComplete(() => { gameObject.SetActive(false); });
            }
        }
    }
}