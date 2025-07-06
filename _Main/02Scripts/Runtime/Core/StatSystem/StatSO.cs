using System.Collections.Generic;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Main.Runtime.Core.StatSystem
{
    [CreateAssetMenu(fileName = "StatSO", menuName = "SO/StatSystem/Stat")]
    public class StatSO : ScriptableObject, ICloneable
    {
        public delegate void ValueChangeHandler(StatSO stat, float current, float prev);

        public event ValueChangeHandler OnValueChange;


        [InfoBox("Automatically changed according to SO name setting value")]
        [Delayed, OnValueChanged("ChangeAssetName"), BoxGroup("Basic Info")]
        public string statName;

        [TextArea, BoxGroup("Basic Info", centerLabel: true)]
        public string description;

        [SerializeField, BoxGroup("Basic Info")]
        private string _displayName;

        public string DisplayName => _displayName;


        [SerializeField, PreviewField(75), HorizontalGroup("Data", 75)]
        private Sprite _icon;


        [SerializeField, ProgressBar("_minValue", "_maxValue", R = 0.0f, G = 0.8f, B = 1f), VerticalGroup("Data/Stats")]
        private float _baseValue;

        [SerializeField, VerticalGroup("Data/Stats")]
        private float _minValue, _maxValue;

        private Dictionary<object, Stack<float>> _modifyValueByKeys = new Dictionary<object, Stack<float>>();
        private Dictionary<object, float> _modifyValuePercentByKeys = new();

        [field: SerializeField, VerticalGroup("Data/Stats")]
        public bool IsPercent { get; private set; }

        private float _modifiedValue = 0;
        private float _modifiedValuePercent = 0f;
        public Sprite Icon => _icon;

        public float MaxValue
        {
            get => _maxValue;
            set => _maxValue = value;
        }

        public float MinValue
        {
            get => _minValue;
            set => _minValue = value;
        }

        public float Value
        {
            get
            {
                float value = Mathf.Clamp(_baseValue + _modifiedValue, MinValue, MaxValue);
                if (_modifiedValuePercent != 0)
                {
                    value *= (1 + _modifiedValuePercent * .01f);
                }

                float roundedValue = (float)System.Math.Round(value, 1);
                return roundedValue;
            }
        }

        public bool IsMax => Mathf.Approximately(Value, MaxValue);
        public bool IsMin => Mathf.Approximately(Value, MinValue);

        public float BaseValue
        {
            get
            {
                float roundedValue = (float)System.Math.Round(_baseValue, 1);
                return roundedValue;
            }
            set
            {
                float prevValue = Value;
                _baseValue = Mathf.Clamp(value, MinValue, MaxValue);
                TryInvokeValueChangeEvent(Value, prevValue);
            }
        }

#if UNITY_EDITOR
        private void ChangeAssetName()
        {
            string assetName = $"{statName}Stat";
            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(this), assetName);
        }
#endif

        public void AddModifyValue(object key, float value)
        {
            //이부분은  중첩을 고려할 꺼면 바꿔야 해.

            float prevValue = Value;
            _modifiedValue += value;

            if (!_modifyValueByKeys.ContainsKey(key))
                _modifyValueByKeys.Add(key, new Stack<float>(new[] { value }));
            else
                _modifyValueByKeys[key].Push(value);

            TryInvokeValueChangeEvent(Value, prevValue);
        }

        public void RemoveModifyValue(object key)
        {
            if (_modifyValueByKeys.TryGetValue(key, out Stack<float> value))
            {
                if (value.Count <= 0) return;

                float prevValue = Value;
                _modifiedValue -= value.Pop();

                TryInvokeValueChangeEvent(Value, prevValue);
            }
        }


        public void AddModifyValuePercent(object key, float value)
        {
            if (_modifyValuePercentByKeys.ContainsKey(key)) return;
            float prevValue = Value;
            _modifiedValuePercent += value;

            _modifyValuePercentByKeys.Add(key, value);

            TryInvokeValueChangeEvent(Value, prevValue);
        }

        public void RemoveModifyValuePercent(object key)
        {
            if (_modifyValuePercentByKeys.Remove(key, out float value))
            {
                float prevValue = Value;
                _modifiedValuePercent -= value;

                TryInvokeValueChangeEvent(Value, prevValue);
            }
        }

        public void ClearModifier()
        {
            ClearModifyValue();
            ClearModifyValuePercent();
        }

        public void ClearModifyValue()
        {
            float prevValue = Value;
            _modifyValueByKeys.Clear();
            _modifiedValue = 0;
            TryInvokeValueChangeEvent(Value, prevValue);
        }

        public void ClearModifyValuePercent()
        {
            float prevValue = Value;
            _modifyValuePercentByKeys.Clear();
            _modifiedValuePercent = 0;
            TryInvokeValueChangeEvent(Value, prevValue);
        }

        private void TryInvokeValueChangeEvent(float value, float prevValue)
        {
            if (!Mathf.Approximately(value, prevValue))
                OnValueChange?.Invoke(this, value, prevValue);
        }

        public object Clone()
        {
            return Instantiate(this);
        }
    }
}