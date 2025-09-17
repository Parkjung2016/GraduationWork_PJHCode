using System;
using Main.Runtime.Combat.Core;
using Main.Runtime.Core.StatSystem;
using Main.Shared;
using MoreMountains.Feedbacks;
using Unity.Cinemachine;
using UnityEngine;

namespace Main.Runtime.Combat
{
    public class DamageCollider : MonoBehaviour
    {
        public event Action<Collider, HitInfo> OnDamageTrigger;
        public event Action<Collider> OnHitOther;
        private MMF_Player _hitFeedback;
        private CinemachineImpulseSource _impulseSource;

        public MMF_Player HitFeedback => _hitFeedback;
        public IAgent Agent => _owner;

        private Collider _colliderCompo;
        private CombatDataSO _combatData;

        private IAgent _owner;

        private StatSO _powerStat;
        private StatSO _increaseMomentumGaugeStat;


        public void Init(IAgent owner, StatSO power, StatSO increaseMomentumGauge)
        {
            _hitFeedback = transform.Find("HitTargetFeedback").GetComponent<MMF_Player>();
            _impulseSource = _hitFeedback.GetComponent<CinemachineImpulseSource>();
            _owner = owner;
            _powerStat = power;
            _increaseMomentumGaugeStat = increaseMomentumGauge;
            _colliderCompo.excludeLayers = 1 << owner.GameObject.layer;
        }

        private void Awake()
        {
            _colliderCompo = GetComponent<Collider>();
            DisableCollider();
        }

        public void TriggerCollider(CombatDataSO combatData)
        {
            _combatData = combatData;
            _colliderCompo.enabled = true;
        }

        public void DisableCollider()
        {
            _colliderCompo.enabled = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == _owner.GameObject.layer) return;
            if (other.TryGetComponent(out IDamageable damageable))
            {
                Vector3 hitPoint = other.ClosestPoint(transform.position);
                Vector3 normal = (transform.position - hitPoint).normalized;

                float increaseMomentumGauge = _combatData.GetIncreaseMomentumGauge(_increaseMomentumGaugeStat);
                float power = _combatData.GetPower(_powerStat);
                GetDamagedAnimationClipInfo getDamagedAnimationClipInfo = _combatData.GetDamagedAnimationClip();
                MonoBehaviour attacker = _owner as MonoBehaviour;
                GetDamagedInfo getDamagedInfo = new GetDamagedInfo()
                    .SetIncreaseMomentumGauge(increaseMomentumGauge)
                    .SetDamage(power)
                    .SetAttacker(attacker)
                    .SetHitPoint(hitPoint)
                    .SetNormal(normal)
                    .SetGetDamagedAnimationClip(getDamagedAnimationClipInfo)
                    .SetIsForceAttack(_combatData.isForceAttack)
                    .SetIsKnockDown(_combatData.isKnockDown)
                    .SetKnockDownTime(_combatData.knockDownTime)
                    .SetGetUpAnimationClip(_combatData.getUpAnimationClip);
                IAgent hitTarget = other.GetComponent<IAgent>();
                HitInfo hitInfo = new()
                {
                    hitTarget = hitTarget
                };
                if (damageable.ApplyDamage(getDamagedInfo))
                {
                    _impulseSource.ImpulseDefinition.ImpulseShape = _combatData.combatFeedbackData.impulseShape;
                    _hitFeedback.PlayFeedbacks();
                    OnDamageTrigger?.Invoke(other, hitInfo);
                }

                if (_combatData.disableColliderOnHit)
                    DisableCollider();
            }
            else
            {
                OnHitOther?.Invoke(other);
            }
        }
    }
}