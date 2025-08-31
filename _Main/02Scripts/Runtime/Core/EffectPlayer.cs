using DG.Tweening;
using UnityEngine;
using UnityEngine.VFX;

namespace Main.Runtime.Core
{
    public class EffectPlayer : MonoBehaviour
    {
        private ParticleSystem[] _particleSystems;
        private VisualEffect[] _visualEffects;
        private TrailRenderer[] _trailRenderers;

        [SerializeField] private bool _isLooped;

        [SerializeField] private float _playTime;

        private void Awake()
        {
            _particleSystems = GetComponentsInChildren<ParticleSystem>();
            _visualEffects = GetComponentsInChildren<VisualEffect>();
            _trailRenderers = GetComponentsInChildren<TrailRenderer>(true);
        }

        public virtual void PlayEffects()
        {
            if (_particleSystems.Length > 0)
            {
                for (int i = 0; i < _particleSystems.Length; i++)
                {
                    _particleSystems[i].Play();
                }
            }

            if (_visualEffects.Length > 0)
            {
                for (int i = 0; i < _visualEffects.Length; i++)
                {
                    _visualEffects[i].Play();
                }
            }
            
            if (_trailRenderers.Length > 0)
            {
                for (int i = 0; i < _trailRenderers.Length; i++)
                {
                    _trailRenderers[i].enabled = true;
                }
            }
            
            if (_isLooped) return;
            DOVirtual.DelayedCall(_playTime, StopEffects);
        }

        public virtual void StopEffects()
        {
            if (_visualEffects.Length > 0)
            {
                for (int i = 0; i < _visualEffects.Length; i++)
                {
                    _visualEffects[i].Stop();
                }
            }

            if (_particleSystems.Length > 0)
            {
                for (int i = 0; i < _particleSystems.Length; i++)
                {
                    _particleSystems[i].Stop();
                }
            }
            
            if (_trailRenderers.Length > 0)
            {
                for (int i = 0; i < _trailRenderers.Length; i++)
                {
                    _trailRenderers[i].enabled = false;
                }
            }
        }
    }
}