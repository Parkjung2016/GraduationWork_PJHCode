using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.VFX;

namespace Main.Runtime.Core
{
    public class PoolEffectPlayer : MonoBehaviour, IPoolable
    {
        [SerializeField] private PoolTypeSO _poolType;
        [SerializeField] private bool _isLooped;
        public PoolTypeSO PoolType => _poolType;
        public GameObject GameObject => gameObject;
        [SerializeField] protected float _playTime;

        protected Pool _myPool;
        protected List<ParticleSystem> _particles;
        protected List<VisualEffect> _visualEffects;

        public virtual void SetUpPool(Pool pool)
        {
            _myPool = pool;
            _particles = GetComponentsInChildren<ParticleSystem>().ToList();
            _visualEffects = GetComponentsInChildren<VisualEffect>().ToList();

            if (_particles.Count > 0)
            {
                float maxLifeTime = _particles.Select(p => p.main.startLifetime.constant).Max();
                float maxDurationTime = _particles.Select(p => p.main.duration).Max();
                _playTime = Mathf.Max(maxLifeTime, maxDurationTime);
            }
        }

        public virtual void ResetItem()
        {
            if (_particles != null)
            {
                _particles.ForEach(p =>
                {
                    p.Stop();
                    p.Simulate(0);
                });
            }
            else
            {
                _visualEffects.ForEach(effect =>
                {
                    effect.Stop();
                    effect.Simulate(0);
                });
            }
        }


        public virtual void PlayEffects(Vector3 position, Quaternion rotation)
        {
            transform.SetPositionAndRotation(position, rotation);
            PlayEffects();
        }

        public void PlayEffects()
        {
            if (_particles != null)
            {
                foreach (var particle in _particles)
                {
                    particle.Play();
                }
            }
            else
            {
                foreach (var effect in _visualEffects)
                {
                    effect.Play();
                }
            }

            if (_isLooped) return;
            DOVirtual.DelayedCall(_playTime, PushEffect);
        }

        public void StopEffects()
        {
            if (_particles != null)
            {
                foreach (var particle in _particles)
                {
                    particle.Stop();
                }
            }
            else
            {
                foreach (var effect in _visualEffects)
                {
                    effect.Stop();
                }
            }

            DOVirtual.DelayedCall(3, PushEffect);
        }

        public void PushEffect()
        {
            _myPool.Push(this);
        }
    }
}