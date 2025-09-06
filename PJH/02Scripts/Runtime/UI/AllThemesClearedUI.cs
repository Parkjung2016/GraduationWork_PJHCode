using Main.Runtime.Manager;
using UnityEngine;
using UnityEngine.Video;

namespace PJH.Runtime.UI
{
    public class AllThemesClearedUI : MonoBehaviour
    {
        private VideoPlayer _videoPlayer;

        private void Awake()
        {
            _videoPlayer = transform.Find("Video Player").GetComponent<VideoPlayer>();
        }

        private void Start()
        {
            _videoPlayer.Play();
            _videoPlayer.loopPointReached += HandleVideoEnd;
        }

        private void HandleVideoEnd(VideoPlayer source)
        {
            SceneManagerEx.LoadScene("Lobby", true);
        }
    }
}