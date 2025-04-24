using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Main.Runtime.Core.StatSystem
{
    [Serializable]
    public class StatOverride
    {
        [InlineEditor] [SerializeField] private StatSO _stat;
        [SerializeField] private bool _isUseOverride;

        [SerializeField, ShowIf("_isUseOverride"),ProgressBar("GetMinValue", "GetMaxValue", 1,0,0)]
        private float _overrideValue;


        private float GetMinValue()
        {
            return _stat.MinValue;
        }
        private float GetMaxValue()
        {
            return _stat.MaxValue;
        }
        public StatOverride(StatSO stat) => _stat = stat;

        public StatSO CreateStat()
        {
            StatSO newStat = _stat.Clone() as StatSO;
            if (_isUseOverride)
                newStat.BaseValue = _overrideValue;

            return newStat;
        }
    }
}