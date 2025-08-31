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
        public bool isLooped;
        public PoolTypeSO PoolType => _poolType;
        public GameObject GameObject => gameObject;
        public float playTime;

        protected Pool _myPool;
        protected List<ParticleSystem> _particles;
        protected List<VisualEffect> _visualEffects;

        public virtual void SetUpPool(Pool pool)
        {
            _myPool = pool;
            _particles = GetComponentsInChildren<ParticleSystem>().ToList();
            _visualEffects = GetComponentsInChildren<VisualEffect>().ToList();
        }

        public virtual void ResetItem()
        {
            if (_particles != null)
            {
                _particles.ForEach(p =>
                {
                    p.Stop();
                    // p.Simulate(0);
                });
            }
            else
            {
                _visualEffects.ForEach(effect =>
                {
                    effect.Stop();
                    // effect.Simulate(0);
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

            if (isLooped) return;
            DOVirtual.DelayedCall(playTime, PushEffect);
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

        public void ChangeLifeTime(float value)
        {
            if (_particles is { Count: > 0 })
            {
                var ps = _particles[0];
                var main = ps.main;
                var curve = main.startLifetime;
                curve.constant = value;
                main.startLifetime = curve;
            }
        }
    }
}