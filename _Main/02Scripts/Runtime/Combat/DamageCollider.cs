using System;
using Animancer;
using Main.Runtime.Combat.Core;
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

        private float _power;
        private float _increaseMomentumGauge;

        public void Init(IAgent owner, float power, float increaseMomentumGauge)
        {
            _hitFeedback = transform.Find("HitTargetFeedback").GetComponent<MMF_Player>();
            _impulseSource = _hitFeedback.GetComponent<CinemachineImpulseSource>();
            _owner = owner;
            _power = power;
            _increaseMomentumGauge = increaseMomentumGauge;
            Physics.IgnoreCollision(_colliderCompo, _owner.GameObject.GetComponent<Collider>());
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
                float increaseMomentumGauge = _increaseMomentumGauge * _combatData.increaseMomentumGaugeMultiplier;
                int getDamagedAnimationIndex = _combatData.currentGetDamagedAnimationClipIndex;
                GetDamagedAnimationClipInfo getDamagedAnimationClip =
                    _combatData.getDamagedAnimationClips[getDamagedAnimationIndex];
                GetDamagedInfo getDamagedInfo = new()
                {
                    hitPoint = transform.position,
                    damage = _combatData.damageMultiplier * _power,
                    increaseMomentumGauge = increaseMomentumGauge,
                    attacker = _owner as MonoBehaviour,
                    isForceAttack = _combatData.isForceAttack,
                    isKnockDown = _combatData.isKnockDown,
                    knockDownTime = _combatData.knockDownTime,
                    getUpAnimationClip = _combatData.getUpAnimationClip,
                    getDamagedAnimationClip = getDamagedAnimationClip
                };
                IAgent hitTarget = (damageable as MonoBehaviour).GetComponent<IAgent>();
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