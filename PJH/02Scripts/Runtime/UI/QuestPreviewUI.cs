using DG.Tweening;
using Main.Runtime.Core.Events;
using PJH.Utility.Managers;
using UnityEngine;
using UnityEngine.Video;

namespace PJH.Runtime.UI
{
    public class QuestPreviewUI : MonoBehaviour
    {
        private CanvasGroup _canvasGroup;

        private GameEventChannelSO _uiEventChannel;

        private VideoPlayer _videoPlayer;
        private GameObject _previewImage;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _videoPlayer = transform.Find("Video Player").GetComponent<VideoPlayer>();
            _previewImage = transform.Find("PreviewImage").gameObject;
            _canvasGroup.alpha = 0;
            _previewImage.gameObject.SetActive(false);
            _uiEventChannel = AddressableManager.Load<GameEventChannelSO>("UIEventChannelSO");
        }

        private void Start()
        {
            _uiEventChannel.AddListener<ShowQuestPreviewUI>(HandleShowQuestPreview);
        }

        private void OnDestroy()
        {
            _uiEventChannel.RemoveListener<ShowQuestPreviewUI>(HandleShowQuestPreview);
        }

        private void HandleShowQuestPreview(ShowQuestPreviewUI evt)
        {
            if (evt.show)
            {
                _previewImage.gameObject.SetActive(true);
                _videoPlayer.clip = evt.previewVideo;
                _videoPlayer.Play();
                _canvasGroup.DOFade(1, 1f).SetUpdate(true);
            }
            else
            {
                _canvasGroup.DOFade(0, 1f).SetUpdate(true).OnComplete(() =>
                {
                    _previewImage.gameObject.SetActive(false);
                });
                _videoPlayer.Stop();
            }
        }
    }
}