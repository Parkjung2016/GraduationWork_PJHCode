using Cysharp.Threading.Tasks;
using DG.Tweening;
using Main.Core;
using Main.Runtime.Core.Events;
using Main.Runtime.Manager;
using MoreMountains.Feedbacks;
using TMPro;
using UnityEngine;

namespace PJH.Runtime.UI
{
    public class DeathUI : MonoBehaviour
    {
        private GameEventChannelSO _showDeathUIEventChannel;
        [SerializeField] private MMF_Player _showFeedbackPlayer;
        [SerializeField] private RectTransform _lineEffectTrm;
        private TextMeshProUGUI _heartCountTMP;
        private CanvasGroup _canvasGroup;

        private void Awake()
        {
            _showDeathUIEventChannel = AddressableManager.Load<GameEventChannelSO>("UIEventChannelSO");
            _heartCountTMP = transform.Find("Heart Count").GetComponent<TextMeshProUGUI>();
            _canvasGroup = GetComponent<CanvasGroup>();
            _showDeathUIEventChannel.AddListener<ShowDeathUI>(HandleShowDeathUI);
            _canvasGroup.alpha = 0;
            gameObject.SetActive(false);

            PlayerManager.Instance.OnChangedHeartCount += HandleChangedHeartCount;
        }

        private void OnDestroy()
        {
            _showDeathUIEventChannel.RemoveListener<ShowDeathUI>(HandleShowDeathUI);
            var manager = PlayerManager.Instance;
            if (manager != null) manager.OnChangedHeartCount -= HandleChangedHeartCount;
        }

        private void HandleChangedHeartCount(int heartCount)
        {
            _heartCountTMP.SetText(heartCount.ToString());
        }

        private void HandleShowDeathUI(ShowDeathUI evt)
        {
            if (evt.isShowUI)
            {
                gameObject.SetActive(true);
                _heartCountTMP.SetText(PlayerManager.Instance.CurrentHeartCount.ToString());

                _canvasGroup.DOFade(1, 1);
                _showFeedbackPlayer.PlayFeedbacks();
            }
            else
            {
                _canvasGroup.DOFade(0, 1).OnComplete(() => { gameObject.SetActive(false); });
            }
        }

        public async void SpawnHeartCountTextEffect()
        {
            _lineEffectTrm.anchoredPosition = new Vector3(0, Screen.height, 0);
            await UniTask.NextFrame();
            var heartCountText = Instantiate(_heartCountTMP.gameObject, transform);
            heartCountText.transform.DOMoveY(heartCountText.transform.position.y - 200, 3);
            heartCountText.GetComponent<TextMeshProUGUI>().DOFade(0, 1);
            _lineEffectTrm.DOAnchorPosY(-Screen.height, .3f);
        }
    }
}