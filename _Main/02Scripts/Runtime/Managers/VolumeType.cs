using DG.Tweening;
using UnityEngine.Rendering;

namespace Main.Runtime.Manager
{
    public abstract class VolumeType
    {
        protected float _originValue;
        protected Tween _valueTween;
        protected FloatParameter _floatParameter;

        public VolumeType(Volume volume)
        {
        }

        public void SetFloatParameter(FloatParameter floatParameter)
        {
            _floatParameter = floatParameter;
            if (_floatParameter == null) return;
            _originValue = floatParameter.value;
        }

        public void SetValue(float value)
        {
            if (_floatParameter == null) return;
            _floatParameter.Override(value);
        }

        public Tween SetValue(float value, float duration)
        {
            if (_floatParameter == null) return null;

            if (_valueTween != null && _valueTween.IsActive()) _valueTween.Kill();
            _valueTween = DOTween.To(() => _floatParameter.value, SetValue,
                value, duration);
            return _valueTween;
        }

        public void ResetValue()
        {
            if (_floatParameter == null) return;

            _floatParameter.value = _originValue;
        }

        public void ResetValue(float duration)
        {
            SetValue(_originValue, duration);
        }
    }
}