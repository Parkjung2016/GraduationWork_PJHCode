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
            _originValue = floatParameter.value;
        }

        public void SetValue(float value)
        {
            _floatParameter.Override(value);
        }

        public Tween SetValue(float value, float duration)
        {
            if (_valueTween != null && _valueTween.IsActive()) _valueTween.Kill();
            _valueTween = DOTween.To(() => _floatParameter.value, SetValue,
                value, duration);
            return _valueTween;
        }

        public void ResetValue()
        {
            _floatParameter.value = _originValue;
        }

        public void ResetValue(float duration)
        {
            SetValue(_originValue, duration);
        }
    }
}