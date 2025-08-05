using System;
using Animancer;
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
        private MMF_Player _hitFeedback;
        private CinemachineImpulseSource _impulseSource;

        public MMF_Player HitFeedback => _hitFeedback;

        private Collider _colliderCompo;
        private CombatDataSO _combatData;

        private IAgent _owner;

        private StatSO _power;
        private StatSO _increaseMomentumGauge;

        private float _powerMultiplier;
        private float _increaseMomentumGaugeMultiplier;

        public void Init(IAgent owner, StatSO power, StatSO increaseMomentumGauge, float powerMultiplier,
            float increaseMomentumGaugeMultiplier)
        {
            _hitFeedback = transform.Find("HitTargetFeedback").GetComponent<MMF_Player>();
            _impulseSource = _hitFeedback.GetComponent<CinemachineImpulseSource>();
            _owner = owner;
            _power = power;
            _powerMultiplier = powerMultiplier;
            _increaseMomentumGauge = increaseMomentumGauge;
            _increaseMomentumGaugeMultiplier = increaseMomentumGaugeMultiplier;
            _colliderCompo.excludeLayers = owner.GameObject.layer;
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
                float increaseMomentumGauge =
                    _increaseMomentumGauge.Value * _combatData.increaseMomentumGaugeMultiplier *
                    _increaseMomentumGaugeMultiplier;
                int getDamagedAnimationIndex = _combatData.currentGetDamagedAnimationClipIndex;
                IAgent hitTarget = other.GetComponent<IAgent>();
                GetDamagedAnimationClipInfo getDamagedAnimationClip =
                    _combatData.getDamagedAnimationClips[getDamagedAnimationIndex];
                Vector3 hitPoint = other.ClosestPoint(transform.position);
                Vector3 normal = (transform.position - hitPoint).normalized;
                float damage = _combatData.damageMultiplier * _power.Value * _powerMultiplier;
                GetDamagedInfo getDamagedInfo = new()
                {
                    hitPoint = hitPoint,
                    normal = normal,
                    damage = damage,
                    increaseMomentumGauge = increaseMomentumGauge,
                    attacker = _owner as MonoBehaviour,
                    isForceAttack = _combatData.isForceAttack,
                    isKnockDown = _combatData.isKnockDown,
                    knockDownTime = _combatData.knockDownTime,
                    getUpAnimationClip = _combatData.getUpAnimationClip,
                    getDamagedAnimationClip = getDamagedAnimationClip
                };
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
        }
    }
}