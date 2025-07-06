using System;
using System.Collections.Generic;
using Main.Runtime.Core.Events;
using PJH.Runtime.PlayerPassive;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PJH.Runtime.UI
{
    public class PassiveInfoProgress : MonoBehaviour
    {
        private TextMeshProUGUI _progressText;

        private void Awake()
        {
            _progressText = transform.Find("RemainingTimeText").GetComponent<TextMeshProUGUI>();
        }

        public void StartProgress(ref UpdatePassiveTimeEventHandler updatePassiveTimeEventHandler,
            IReadOnlyList<PassiveInfoType> infoTypes, Action
                OnEndTimer)
        {
            gameObject.SetActive(true);

            updatePassiveTimeEventHandler += (time, cooldownTime) =>
            {
                float fillAmount = time / cooldownTime;
                _progressText.SetText($"{time:F1}s");
                if (fillAmount <= 0f)
                {
                    OnEndTimer?.Invoke();
                }
            };
        }
    }
}