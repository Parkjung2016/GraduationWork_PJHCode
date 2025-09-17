using System;
using System.Collections.Generic;
using BIS.Events;
using Cysharp.Text;
using Cysharp.Threading.Tasks;
using Main.Runtime.Core.Events;
using PJH.Runtime.Core;
using PJH.Runtime.PlayerPassive;
using PJH.Runtime.Players;
using PJH.Utility.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace PJH.Runtime.UI
{
    public class ComboPieceTooltipUI : MonoBehaviour
    {
        private GameEventChannelSO _uiEventChannel;
        private ComboColorInfoSO _comboColorInfo;
        private CommandActionPiecePriceConfigSO _commandActionPiecePriceConfig;
        private TextMeshProUGUI _comboPieceNameTMP, _comboPieceInfoTMP, _comboPassiveInfoTMP, _comboPriceTMP;
        private Image _comboIconImage;
        private RectTransform _rectTrm;
        private Vector2 _originPivot;

        private void Awake()
        {
            _rectTrm = transform as RectTransform;
            Transform popupTrm = transform.Find("Max/Popup");
            _comboIconImage = popupTrm.Find("ComboIcon_Image").GetComponent<Image>();
            _comboPieceNameTMP = popupTrm.Find("ComboName_Text").GetComponent<TextMeshProUGUI>();
            _comboPieceInfoTMP = popupTrm.Find("ComboInfo_Text").GetComponent<TextMeshProUGUI>();
            _comboPassiveInfoTMP = popupTrm.Find("ComboPassive_Text").GetComponent<TextMeshProUGUI>();
            _comboPriceTMP = popupTrm.Find("Price_Text").GetComponent<TextMeshProUGUI>();
            _uiEventChannel = AddressableManager.Load<GameEventChannelSO>("UIEventChannelSO");
            _comboColorInfo = AddressableManager.Load<ComboColorInfoSO>("ComboColorInfo");
            _commandActionPiecePriceConfig =
                AddressableManager.Load<CommandActionPiecePriceConfigSO>("CommandActionPiecePriceConfig");
            _uiEventChannel.AddListener<ShowComboPieceTooltipUIEvent>(HandleShowComboPiecePreviewUI);
            gameObject.SetActive(false);
            _originPivot = _rectTrm.pivot;
        }

        private void OnDestroy()
        {
            _uiEventChannel.RemoveListener<ShowComboPieceTooltipUIEvent>(HandleShowComboPiecePreviewUI);
        }

        private void Update()
        {
            UpdatePosition();
        }

        private void UpdatePosition()
        {
            Vector3 mousePosition = Mouse.current.position.value;
            RectTransform canvasRectTrm = (RectTransform)transform.parent;

            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRectTrm,
                mousePosition,
                null,
                out localPoint
            );

            Vector2 sizeDelta = _rectTrm.sizeDelta;

            Vector2 halfCanvasSize = canvasRectTrm.sizeDelta * 0.5f;

            Vector2 targetPivot = _originPivot;
            if (localPoint.x - sizeDelta.x < -halfCanvasSize.x)
            {
                targetPivot.x = 0f;
            }

            if (localPoint.y - sizeDelta.y < -halfCanvasSize.y)
            {
                targetPivot.y = 0f;
            }

            _rectTrm.pivot = targetPivot;

            _rectTrm.anchoredPosition = localPoint;
        }

        private async void HandleShowComboPiecePreviewUI(ShowComboPieceTooltipUIEvent evt)
        {
            if (!evt.show || evt.comboPiece == null)
            {
                gameObject.SetActive(false);
                return;
            }

            UpdatePosition();
            try
            {
                await UniTask.Yield(gameObject.GetCancellationTokenOnDestroy());
                gameObject.SetActive(true);
                CommandActionPieceSO comboPiece = evt.comboPiece;
                _comboIconImage.sprite = comboPiece.pieceIcon;
                _comboPieceNameTMP.text = comboPiece.pieceDisplayName;
                _comboPieceInfoTMP.text = comboPiece.pieceDescription;
                _comboPassiveInfoTMP.text = string.Empty;
                IReadOnlyList<PassiveSO> passives = comboPiece.Passives;
                using (var sb = ZString.CreateStringBuilder())
                {
                    for (int i = 0; i < passives.Count; i++)
                    {
                        PassiveSO passive = passives[i];
                        Color rankColor = _comboColorInfo.GetPassiveRankColor(passive.RankType);
                        string rankColorHex = "#" + ColorUtility.ToHtmlStringRGBA(rankColor);

                        sb.Append($"<color={rankColorHex}>{passive.pieceDisplayName}</color>");
                        sb.AppendLine(" : ");
                        sb.Append(passive.pieceDescription);
                        sb.Append("\n\n");
                    }

                    _comboPassiveInfoTMP.SetText(sb.ToString());
                }


                if (UIEvent.ShowComboPieceTooltipUIEvent.showPrice)
                {
                    _comboPriceTMP.SetText($"{_commandActionPiecePriceConfig.GetPrice(comboPiece)}G");
                }
                else
                    _comboPriceTMP.gameObject.SetActive(false);
            }
            catch (Exception e)
            {
            }
        }
    }
}