using System;
using Main.Core;
using Main.Runtime.Manager;
using PJH.Runtime.Core;
using PJH.Runtime.Players;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Debug = Main.Core.Debug;

namespace PJH.Runtime.UI
{
    public class ComboPieceItemUI : MonoBehaviour, IPointerClickHandler
    {
        private Image _iconImage, _outlineImage;
        private TextMeshProUGUI _equipSlotIndexText;
        private CanvasGroup _selectedOutline;

        private event Func<CommandActionPieceSO, bool> OnClickAction;

        private CommandActionPieceSO _commandActionPiece;

        private ComboColorInfoSO _comboColorInfo;

        private void Awake()
        {
            _comboColorInfo = AddressableManager.Load<ComboColorInfoSO>("ComboColorInfo");
            _iconImage = transform.Find("ComboIcon_Image").GetComponent<Image>();
            _outlineImage = transform.Find("ComboOutline_Image").GetComponent<Image>();
            _equipSlotIndexText = transform.Find("EquipSlotIndex_Text").GetComponent<TextMeshProUGUI>();
            _selectedOutline = transform.Find("SelectedOutline_Image").GetComponent<CanvasGroup>();
            _selectedOutline.alpha = 0;
        }

        public void SetItem(CommandActionPieceSO commandActionPiece, Func<CommandActionPieceSO, bool> OnClickCallBack)
        {
            _commandActionPiece = commandActionPiece;
            _iconImage.sprite = commandActionPiece.pieceIcon;
            OnClickAction = OnClickCallBack;
            _equipSlotIndexText.SetText("");
        }

        public void SetEquipIndex(int equipIndex, int equipSlotIndex)
        {
            _outlineImage.color = _comboColorInfo.GetComboSlotColor(equipIndex);

            _equipSlotIndexText.SetText(equipSlotIndex.ToString());
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            // if (eventData.button != PointerEventData.InputButton.Left) return;
            bool isActive = OnClickAction(_commandActionPiece);
            string soundPath = isActive ? "event:/UI/ComboSlotEquip" : "event:/UI/ComboSlotUnEquip";
            _selectedOutline.alpha = Convert.ToByte(isActive);
            Managers.FMODManager.PlaySound(soundPath);
        }
    }
}