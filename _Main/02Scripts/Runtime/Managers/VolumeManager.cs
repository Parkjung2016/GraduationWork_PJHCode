using System;
using DG.Tweening;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Main.Runtime.Manager
{
    public class VolumeManager
    {
        private Beautify.Universal.Beautify _beautify;
        private float _originSaturate;
        private float _originBrightness;
        private float _originVignettingFade;
        private float _originSepia;
        private float _finalblur;

        private Tween _saturateTween;
        private Tween _brightnessTween;
        private Tween _vignettingFadeTween;
        private Tween _sepiaTween;
        private Tween _finalblurTween;

        public VolumeManager()
        {
            SceneManager.sceneLoaded += HandleSceneLoaded;
            FindVolumeComponent();
        }

        private void HandleSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            FindVolumeComponent();
        }

        private void FindVolumeComponent()
        {
            Volume volume = Object.FindAnyObjectByType<Volume>();
            if (volume)
            {
                volume.profile.TryGet(out _beautify);
                _originSaturate = _beautify.saturate.value;
                _originBrightness = _beautify.brightness.value;
                _originVignettingFade = _beautify.vignettingFade.value;
                _originSepia = _beautify.sepia.value;
                _finalblur = _beautify.blurIntensity.value;
            }
        }

        public void SetSepia(float value)
        {
            _beautify.sepia.Override(value);
        }

        public void SetSepia(float value, float duration)
        {
            if (_sepiaTween != null && _sepiaTween.IsActive()) _sepiaTween.Kill();
            _sepiaTween = DOTween.To(() => _beautify.sepia.value, x => SetSepia(x), value, duration);
        }

        public void SetSaturate(float value)
        {
            _beautify.saturate.Override(value);
        }

        public void SetSaturate(float value, float duration)
        {
            if (_saturateTween != null && _saturateTween.IsActive()) _saturateTween.Kill();
            _saturateTween = DOTween.To(() => _beautify.saturate.value, x => SetSaturate(x), value, duration);
        }

        public void SetBrightness(float value)
        {
            _beautify.brightness.Override(value);
        }

        public void SetBrightness(float value, float duration)
        {
            if (_brightnessTween != null && _brightnessTween.IsActive()) _brightnessTween.Kill();
            _brightnessTween = DOTween.To(() => _beautify.brightness.value, x => SetBrightness(x), value, duration);
        }

        public void SetVignettingFade(float value)
        {
            _beautify.vignettingFade.Override(value);
        }

        public void SetVignettingFade(float value, float duration)
        {
            if (_vignettingFadeTween != null && _vignettingFadeTween.IsActive()) _vignettingFadeTween.Kill();
            _vignettingFadeTween = DOTween.To(() => _beautify.vignettingFade.value, x => SetVignettingFade(x), value,
                duration);
        }

        public void SetFinalBlur(float value)
        {
            _beautify.blurIntensity.Override(value);
        }
        
        public void SetFinalBlur(float value, float duration)
        {
            if (_finalblurTween != null && _finalblurTween.IsActive()) _finalblurTween.Kill();
            _finalblurTween = DOTween.To(() => _beautify.blurIntensity.value, x => SetFinalBlur(value), 
                value, duration);
        }

        public void ResetSepia()
        {
            SetSepia(_originSepia);
        }

        public void ResetSepia(float duration)
        {
            SetSepia(_originSepia, duration);
        }

        public void ResetBrightness()
        {
            SetBrightness(_originBrightness);
        }

        public void ResetBrightness(float duration)
        {
            SetBrightness(_originBrightness, duration);
        }

        public void ResetVignetteFade()
        {
            SetVignettingFade(_originVignettingFade);
        }

        public void ResetVignetteFade(float duration)
        {
            SetVignettingFade(_originVignettingFade, duration);
        }

        public void ResetSaturate()
        {
            SetSaturate(_originSaturate);
        }

        public void ResetSaturate(float duration)
        {
            SetSaturate(_originSaturate, duration);
        }

        public void ResetFinalBlur()
        {
            SetFinalBlur(_finalblur);
        }
        
        public void ResetFinalBlur(float duration)
        {
            SetFinalBlur(_finalblur, duration);
        }

        public Tween SetBlink(float value, float duration = .5f)
        {
            return DOTween.To(() => _beautify.vignettingBlink.value, x => _beautify.vignettingBlink.value = x, value,
                duration);
        }
    }
}