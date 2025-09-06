using System;
using System.Collections.Generic;
using System.Threading;
using BIS.Data;
using BIS.Events;
using Cysharp.Threading.Tasks;
using Main.Runtime.Core.Events;
using Main.Runtime.Manager;
using PJH.Runtime.Core;
using PJH.Runtime.Players;
using PJH.Utility.Managers;
using TMPro;
using UnityEngine;
using CommandActionData = PJH.Runtime.Players.CommandActionData;

namespace PJH.Runtime.UI
{
    public class ComboPieceLoadoutUI : MonoBehaviour
    {
        [SerializeField] private Transform _comboPieceContentTrm;

        [SerializeField] private TextMeshProUGUI _totalPriceText;
        [SerializeField] private TextMeshProUGUI _currentGoldText;
        [SerializeField] private TextMeshProUGUI _noticeText;

        [SerializeField] private CommandActionPiecePriceConfigSO _piecePriceData;

        private CurrencySO _moneySO;
        private InventorySO _inventory;
        private PlayerInputSO _playerInput;

        private Dictionary<CommandActionPieceSO, int> _savedPieces = new();

        private GameEventChannelSO _gameEventChannel, _uiEventChannelSO;
        private CancellationTokenSource _noticeCancellationTokenSource;
        private int _totalPrice;

        private int totalPrice
        {
            get => _totalPrice;
            set
            {
                _totalPrice = value;
                string totalPriceToString = value.ToString("#,0");

                _totalPriceText.SetText($"기억 비용: <color=#FFD700>{totalPriceToString}</color>");
            }
        }

        private void Awake()
        {
            Time.timeScale = 0;
            _moneySO = BIS.Manager.Managers.Resource.Load<CurrencySO>("Money");
            _playerInput = AddressableManager.Load<PlayerInputSO>("PlayerInputSO");
            _uiEventChannelSO = AddressableManager.Load<GameEventChannelSO>("UIEventChannelSO");
            _gameEventChannel = AddressableManager.Load<GameEventChannelSO>("GameEventChannel");
            _playerInput.EnablePlayerInput(false);
            _playerInput.EnableUIInput(false);
            CursorManager.SetCursorLockMode(CursorLockMode.None);

            _inventory = AddressableManager.Load<InventorySO>("InventorySO");
            Player player = PlayerManager.Instance.Player as Player;
            PlayerCommandActionManager commandActionManager = player.GetCompo<PlayerCommandActionManager>();
            for (int i = 0; i < commandActionManager.CommandActions.Count; i++)
            {
                CommandActionData commandActionData = commandActionManager.CommandActions[i];
                commandActionData.ExecuteCommandActionPieces
                    .ForEach(piece => { CreateComboPieceItemUI(piece, i, piece.equipSlotIndex); });
            }

            totalPrice = 0;
            _inventory.ComboPieceItemSOs.ForEach(piece =>
            {
                if (piece != null)
                    CreateComboPieceItemUI(piece);
            });
            int currentGold = _moneySO.CurrentAmmount;
            UpdateCurrentGoldText(currentGold);
            _moneySO.ValueChangeEvent += UpdateCurrentGoldText;
            _noticeText.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            _moneySO.ValueChangeEvent -= UpdateCurrentGoldText;
        }

        private void UpdateCurrentGoldText(int value)
        {
            string valueToString = value.ToString("#,0");
            _currentGoldText.SetText(valueToString);
        }

        private ComboPieceItemUI CreateComboPieceItemUI(CommandActionPieceSO piece)
        {
            ComboPieceItemUI comboPieceItemUI =
                AddressableManager.Instantiate<ComboPieceItemUI>("ComboPieceItemUI", _comboPieceContentTrm);
            comboPieceItemUI.SetItem(piece, selectedPiece =>
            {
                int price = 0;
                var evt = UIEvent.ComboDescriptingUIEvent;
                if (_savedPieces.Remove(selectedPiece, out price))
                {
                    totalPrice -= price;
                    if (_savedPieces.Count == 0)
                    {
                        evt.commandActionPieceSO = null;
                        _uiEventChannelSO.RaiseEvent(evt);
                    }
                    else
                    {
                        evt.commandActionPieceSO = selectedPiece;
                        _uiEventChannelSO.RaiseEvent(evt);
                    }

                    return false;
                }

                price = _piecePriceData.GetPrice(selectedPiece);
                _savedPieces.Add(selectedPiece, price);
                totalPrice += price;
                evt.commandActionPieceSO = selectedPiece;
                _uiEventChannelSO.RaiseEvent(evt);
                return true;
            });
            return comboPieceItemUI;
        }

        private void CreateComboPieceItemUI(CommandActionPieceSO piece, int equipIndex, int equipSlotIndex)
        {
            ComboPieceItemUI comboPieceItemUI =
                CreateComboPieceItemUI(piece);
            comboPieceItemUI.SetEquipIndex(equipIndex, equipSlotIndex);
        }

        public void SavePieceButton()
        {
            PurchaseData purchaseData = _moneySO.Purchase(totalPrice);
            if (purchaseData.isPurchasable)
            {
                _inventory.ResetList();
                foreach (var pair in _savedPieces)
                {
                    _inventory.AddElement(pair.Key);
                }

                _gameEventChannel.RaiseEvent(GameEvents.GoToLobby);
            }
            else
            {
                ShowNoticeText();
            }
        }

        private async void ShowNoticeText()
        {
            if (_noticeCancellationTokenSource is { IsCancellationRequested: false })
            {
                _noticeCancellationTokenSource.Cancel();
                _noticeCancellationTokenSource.Dispose();
            }

            try
            {
                _noticeCancellationTokenSource = new();
                _noticeCancellationTokenSource.RegisterRaiseCancelOnDestroy(gameObject);
                Managers.FMODManager.PlayErrorSound();
                _noticeText.gameObject.SetActive(true);
                await UniTask.WaitForSeconds(1f, cancellationToken: _noticeCancellationTokenSource.Token);
                _noticeText.gameObject.SetActive(false);
            }
            catch
            {
            }
        }
    }
}