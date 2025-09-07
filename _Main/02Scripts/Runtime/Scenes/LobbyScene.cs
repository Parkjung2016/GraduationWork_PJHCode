using BIS.Data;
using Main.Runtime.Core.Events;
using Main.Runtime.Manager;
using PJH.Runtime.Core;
using PJH.Runtime.Players;
using PJH.Utility.Extensions;
using PJH.Utility.Managers;
using UnityEngine;

namespace Main.Scenes
{
    public class LobbyScene : BaseScene
    {
        private GameEventChannelSO _gameEventChannel;
        private InventorySO _inventory;

        protected override void Start()
        {
            base.Start();
            _inventory = AddressableManager.Load<InventorySO>("InventorySO");
            _gameEventChannel = AddressableManager.Load<GameEventChannelSO>("GameEventChannel");
            AddressableManager.Load<ComboSynthesisPriceInfoSO>("ComboSynthesisPriceInfo")
                .ResetIncreaseLevel();
            Application.targetFrameRate = 60;

            _gameEventChannel.AddListener<ResetEquippedCombo>(HandleResetEquippedCombo);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _gameEventChannel.RemoveListener<ResetEquippedCombo>(HandleResetEquippedCombo);
        }

        private void HandleResetEquippedCombo(ResetEquippedCombo evt)
        {
            Player player = PlayerManager.Instance.Player as Player;
            player.GetCompo<PlayerCommandActionManager>().CommandActions.ForEach(x =>
            {
                x.ExecuteCommandActionPieces.ForEach(piece =>
                {
                    if (piece.Passives.Count > 0)
                    {
                        _inventory.AddElement(piece);
                    }
                });
            });
        }
    }
}