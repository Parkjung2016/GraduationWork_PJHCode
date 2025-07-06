using System.Collections.Generic;
using Main.Runtime.Core.Events;
using PJH.Runtime.PlayerPassive;
using UnityEngine;
using UnityEngine.UI;

namespace PJH.Runtime.UI
{
    public class PassiveInfoUI : MonoBehaviour
    {
        private PassiveSO _currentPassive;
        private Image _passiveIcon;
        private List<PassiveInfoType> _infoTypes = new();

        private PassiveInfoProgress _buffProgress, _cooldownProgress;

        private void Awake()
        {
            Transform maskTrm = transform.Find("Mask");
            _passiveIcon = maskTrm.Find("Icon").GetComponent<Image>();
            _buffProgress = transform.Find("BuffProgress").GetComponent<PassiveInfoProgress>();
            _cooldownProgress = transform.Find("CooldownProgress").GetComponent<PassiveInfoProgress>();
            _buffProgress.gameObject.SetActive(false);
            _cooldownProgress.gameObject.SetActive(false);
        }

        public void SetPassiveInfo(PassiveSO passive)
        {
            _currentPassive = passive;
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
                case PassiveInfoType.Cooldown:
                {
                    if (_currentPassive is ICooldownPassive cooldownPassive)
                    {
                        _cooldownProgress.StartProgress(ref cooldownPassive.CooldownPassiveInfo.OnUpdateCooldownTime,
                            _infoTypes,
                            () =>
                            {
                                Destroy(gameObject);
                                RemovePassiveInfoType(PassiveInfoType.Buff,
                                    ref cooldownPassive.CooldownPassiveInfo.OnUpdateCooldownTime);
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