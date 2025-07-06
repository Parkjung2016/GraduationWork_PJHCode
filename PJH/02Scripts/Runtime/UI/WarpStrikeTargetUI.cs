using Main.Core;
using Main.Runtime.Agents;
using Main.Runtime.Core.Events;
using UnityEngine;

namespace PJH.Runtime.UI
{
    public class WarpStrikeTargetUI : MonoBehaviour
    {
        private GameEventChannelSO _showWarpStrikeTargetUIEventChannel;

        private Agent _warpStrikeTarget;

        private void Awake()
        {
            _showWarpStrikeTargetUIEventChannel = AddressableManager.Load<GameEventChannelSO>("UIEventChannelSO");
            _showWarpStrikeTargetUIEventChannel.AddListener<ShowWarpStrikeTargetUI>(HandleShowWarpStrikeTargetUI);
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            _showWarpStrikeTargetUIEventChannel.RemoveListener<ShowWarpStrikeTargetUI>(HandleShowWarpStrikeTargetUI);
        }

        private void HandleShowWarpStrikeTargetUI(ShowWarpStrikeTargetUI evt)
        {
            _warpStrikeTarget = evt.target as Agent;
            gameObject.SetActive(evt.isShowUI);
        }

        private void LateUpdate()
        {
            if (!_warpStrikeTarget) return;
            transform.GetChild(0).LookAt(Camera.main.transform);
            transform.position = _warpStrikeTarget.HeadTrm.position;
        }
    }
}