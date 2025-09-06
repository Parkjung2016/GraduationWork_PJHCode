using System.Collections.Generic;
using Main.Runtime.Core.Events;
using PJH.Runtime.Core;
using PJH.Runtime.PlayerPassive;
using PJH.Utility.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace PJH.Runtime.UI
{
    public class PassiveInfoUI : MonoBehaviour
    {
        private static readonly int ShiftingFadeShaderHash = Shader.PropertyToID("_ShiftingFade");
        private PassiveSO _currentPassive;
        private Image _passiveIcon;
        private Image _outline;
        private List<PassiveInfoType> _infoTypes = new();

        private PassiveInfoProgress _buffProgress;
        private ComboColorInfoSO _comboColorInfo;

        private void Awake()
        {
            _comboColorInfo = AddressableManager.Load<ComboColorInfoSO>("ComboColorInfo");
            _outline = transform.Find("Outline").GetComponent<Image>();
            Transform maskTrm = transform.Find("Mask");
            _passiveIcon = maskTrm.Find("Icon").GetComponent<Image>();
            _buffProgress = transform.Find("BuffProgress").GetComponent<PassiveInfoProgress>();
            _buffProgress.gameObject.SetActive(false);
        }

        public void SetPassiveInfo(PassiveSO passive)
        {
            _currentPassive = passive;
            Color outlineColor = Color.white;
            float shiftingFadeValue = 1;
            if (passive.RankType != PassiveRankType.High)
            {
                outlineColor = _comboColorInfo.GetPassiveRankColor(passive.RankType);
                shiftingFadeValue = 0;
            }

            _outline.color = outlineColor;
            _outline.material.SetFloat(ShiftingFadeShaderHash, shiftingFadeValue);

            _passiveIcon.sprite = passive.pieceIcon;
        }

        private void OnDestroy()
        {
            if (_currentPassive is IBuffPassive buffPassive)
            {
                buffPassive.BuffPassiveInfo.OnUpdateBuffTime = null;
            }

            if (_currentPassive is ICooldownPassive cooldownPassive)
            {
                cooldownPassive.CooldownPassiveInfo.OnUpdateCooldownTime = null;
            }
        }

        public void AddPassiveInfoType(PassiveInfoType infoType)
        {
            _infoTypes.Add(infoType);
            switch (infoType)
            {
                case PassiveInfoType.Buff:
                {
                    if (_currentPassive is IBuffPassive buffPassive)
                    {
                        _buffProgress.StartProgress(ref buffPassive.BuffPassiveInfo.OnUpdateBuffTime, _infoTypes,
                            () =>
                            {
                                Destroy(gameObject);
                                RemovePassiveInfoType(PassiveInfoType.Buff,
                                    ref buffPassive.BuffPassiveInfo.OnUpdateBuffTime);
                            });
                    }

                    break;
                }
                case PassiveInfoType.None:
                {
                    _currentPassive.OnUnEquipped += HandleUnEquipped;
                    break;
                }
            }
        }

        private void HandleUnEquipped()
        {
            _currentPassive.OnUnEquipped -= HandleUnEquipped;
            Destroy(gameObject);
        }

        private void RemovePassiveInfoType(PassiveInfoType infoType,
            ref UpdatePassiveTimeEventHandler updateEventHandler)
        {
            updateEventHandler = null;
            _infoTypes.Remove(infoType);
        }
    }
}