using System.Collections.Generic;
using PJH.Runtime.PlayerPassive;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PJH.Runtime.Core
{
    [CreateAssetMenu(menuName = "SO/ComboColorInfo")]
    public class ComboColorInfoSO : SerializedScriptableObject
    {
        [SerializeField]
        private Dictionary<PassiveRankType, Color> _passiveRankColors = new Dictionary<PassiveRankType, Color>();

        [SerializeField] private Color[] _comboSlotColors;
        [SerializeField] private Color[] _comboSynthesisSlotColors;

        public Color GetPassiveRankColor(PassiveRankType rankType) => _passiveRankColors[rankType];
        public Color GetComboSlotColor(int index) => _comboSlotColors[index];
        public Color GetComboSynthesisSlotColor(int index) => _comboSynthesisSlotColors[index];
    }
}