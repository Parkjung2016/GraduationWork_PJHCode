using System;
using Main.Core;
using Main.Runtime.Core.Events;
using PJH.Runtime.Players;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PJH.Runtime.UI
{
    public class EvasionWhileHittingInfoUI : MonoBehaviour
    {
        private GameEventChannelSO _uiEventChannel;

        private Image _cooldownProgress;
        private TextMeshProUGUI _cooldownProgressText;

        private Player _player;

        private void Awake()
        {
            Transform fillGroup = transform.Find("EvasionInfoUI/Mask/FillGroup");
            _cooldownProgress = fillGroup.Find("CooldownProgress").GetComponent<Image>();
            _cooldownProgressText =
                _cooldownProgress.transform.Find("RemainingTimeText").GetComponent<TextMeshProUGUI>();
            _uiEventChannel = AddressableManager.Load<GameEventChannelSO>("UIEventChannelSO");
            _uiEventChannel.AddListener<ShowEvasionWhileHittingInfUI>(HandleShowEvasionWhileHittingInfUI);
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            _uiEventChannel.RemoveListener<ShowEvasionWhileHittingInfUI>(HandleShowEvasionWhileHittingInfUI);
        }

        private void HandleShowEvasionWhileHittingInfUI(ShowEvasionWhileHittingInfUI evt)
        {
            _player = evt.player as Player;
            gameObject.SetActive(true);
            _player.GetCompo<PlayerMovement>().OnUpdateCooldownEvasionWhileHitting +=
                HandleUpdateCooldown;
        }

        private void HandleUpdateCooldown(float currentCooldown, float delay)
        {
            _cooldownProgress.fillAmount = currentCooldown / delay;
            _cooldownProgressText.SetText($"{currentCooldown:F1}s");
            if (currentCooldown <= 0)
            {
                gameObject.SetActive(false);
                _player.GetCompo<PlayerMovement>().OnUpdateCooldownEvasionWhileHitting -=
                    HandleUpdateCooldown;
            }
        }
    }
}