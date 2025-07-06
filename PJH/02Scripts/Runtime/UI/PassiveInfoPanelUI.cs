using System.Collections.Generic;
using Main.Core;
using Main.Runtime.Core.Events;
using Main.Runtime.Manager;
using PJH.Runtime.PlayerPassive;
using PJH.Runtime.Players;
using UnityEngine;


namespace PJH.Runtime.UI
{
    public class PassiveInfoPanelUI : MonoBehaviour
    {
        private GameEventChannelSO _uiEventChannel;

        private Dictionary<PassiveSO, PassiveInfoUI> _passiveInfoUIDictionary =
            new Dictionary<PassiveSO, PassiveInfoUI>();

        private Player _player;

        private void Awake()
        {
            _player = PlayerManager.Instance.Player as Player;
            _uiEventChannel = AddressableManager.Load<GameEventChannelSO>("UIEventChannelSO");
            _uiEventChannel.AddListener<ShowPassiveInfoUI>(HandleShowPassiveInfoUI);
        }

        private void OnDestroy()
        {
            _uiEventChannel.RemoveListener<ShowPassiveInfoUI>(HandleShowPassiveInfoUI);
        }

        private void HandleShowPassiveInfoUI(ShowPassiveInfoUI evt)
        {
            if (!_player.CanApplyPassive) return;
            PassiveSO passive = evt.passive as PassiveSO;
            if (_passiveInfoUIDictionary.TryGetValue(passive, out PassiveInfoUI passiveInfoUI))
            {
                if (!passiveInfoUI)
                {
                    _passiveInfoUIDictionary.Remove(passive);
                    CreatePassiveInfoUI(passive, evt.passiveInfoType);
                    return;
                }

                passiveInfoUI.AddPassiveInfoType(evt.passiveInfoType);
            }
            else
            {
                CreatePassiveInfoUI(passive, evt.passiveInfoType);
            }
        }

        private void CreatePassiveInfoUI(PassiveSO passive, PassiveInfoType infoType)
        {
            PassiveInfoUI passiveInfoUI = AddressableManager.Instantiate<PassiveInfoUI>("PassiveInfoUI", transform);
            passiveInfoUI.SetPassiveInfo(passive);
            passiveInfoUI.AddPassiveInfoType(infoType);
            _passiveInfoUIDictionary.Add(passive, passiveInfoUI);
        }
    }
}