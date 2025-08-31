using System;
using System.Collections.Generic;
using FMODUnity;
using Main.Core;
using Main.Runtime.Combat;
using Main.Runtime.Core;
using Main.Runtime.Core.Events;
using Main.Runtime.Core.StatSystem;
using Main.Runtime.Equipments.Datas;
using Main.Runtime.Equipments.Scripts.Weapons.Core;
using Main.Shared;
using MoreMountains.Feedbacks;
using MoreMountains.FeedbacksForThirdParty;
using UnityEngine;

namespace Main.Runtime.Equipments.Scripts
{
    public class Weapon : Equipment
    {
        public event Action OnBreak;
        public WeaponDataSO WeaponData { get; private set; }
        private GameEventChannelSO _hitImpactEffectSpawnChannel;
        private WeaponFracture _fractureCompo;
        private Rigidbody _rigidbodyCompo;
        private Collider _meshCollider;
        private DamageCollider _damageCollider;
        private StatSO _powerStat, _increaseMomentumGaugeStat;
        private byte _currentDurability;

        public byte CurrentDurability
        {
            get => _currentDurability;
            set
            {
                if (!WeaponData.isDurable) return;

                _currentDurability = value;
                if (_currentDurability <= 0)
                {
                    BreakWeapon();
                }
            }
        }

        protected override void Awake()
        {
            base.Awake();
            _hitImpactEffectSpawnChannel = AddressableManager.Load<GameEventChannelSO>("SpawnEventChannel");

            WeaponData = EquipmentData as WeaponDataSO;
            CurrentDurability = WeaponData.maxDurability;
            _fractureCompo = GetComponentInChildren<WeaponFracture>();

            _rigidbodyCompo = GetComponent<Rigidbody>();
            _meshCollider = GetComponent<Collider>();
            _damageCollider = GetComponentInChildren<DamageCollider>();
            _damageCollider.OnDamageTrigger += HandleDamageTrigger;
            _damageCollider.OnHitOther += HandleHitOther;
        }

        public void Equip(IAgent owner, StatSO powerStat, StatSO increaseMomentumGaugeStat)
        {
            base.Equip(owner);
            _powerStat = powerStat;
            _increaseMomentumGaugeStat = increaseMomentumGaugeStat;
            _damageCollider.Init(owner, _powerStat, _increaseMomentumGaugeStat);
            if (_rigidbodyCompo)
            {
                _rigidbodyCompo.isKinematic = true;
                _meshCollider.enabled = false;
            }

            gameObject.layer = LayerMask.NameToLayer("Weapon");
        }

        public override void UnEquip()
        {
            base.UnEquip();
            transform.SetParent(null);
            if (_rigidbodyCompo)
            {
                _rigidbodyCompo.isKinematic = false;
                _meshCollider.enabled = true;
            }

            gameObject.layer = LayerMask.NameToLayer("outline");
        }

        private void BreakWeapon()
        {
            OnBreak?.Invoke();
            if (_fractureCompo)
            {
                _fractureCompo.Explode();
            }

            Destroy(gameObject);
        }

        private void HandleHitOther(Collider other)
        {
            RuntimeManager.PlayOneShot(WeaponData.hitOtherImpactSound, transform.position);
            PoolTypeSO impactPoolType = WeaponData.hitOtherImpactPoolType;

            if (!impactPoolType) return;
            Vector3 effectPosition = other.ClosestPoint(transform.position);
            SpawnImpactEffect(impactPoolType, effectPosition);
        }

        private void HandleDamageTrigger(Collider hitTarget, HitInfo hitInfo)
        {
            CurrentDurability -= 1;

            _owner.OnHitTarget?.Invoke(hitInfo);
            RuntimeManager.PlayOneShot(WeaponData.hitImpactSound, transform.position);
            PoolTypeSO impactPoolType = WeaponData.hitImpactPoolType;
            if (!impactPoolType) return;
            Vector3 effectPosition = hitTarget.ClosestPoint(transform.position);
            SpawnImpactEffect(impactPoolType, effectPosition);
        }

        private void SpawnImpactEffect(PoolTypeSO impactPoolType, Vector3 position)
        {
            if (!impactPoolType) return;
            var evt = SpawnEvents.SpawnEffect;
            evt.effectType = impactPoolType;
            evt.position = position;
            evt.rotation = Quaternion.identity;
            _hitImpactEffectSpawnChannel.RaiseEvent(evt);
        }

        public void TriggerDamageCollider(CombatDataSO combatData)
        {
            MMF_Player feedback = _damageCollider.HitFeedback;
            CombatFeedbackData combatFeedbackData = combatData.combatFeedbackData;
            feedback.GetFeedbackOfType<MMF_CinemachineImpulseSource>().Velocity =
                Vector3.forward * combatFeedbackData.impulsePower;
            feedback.GetFeedbackOfType<MMF_FreezeFrame>().FreezeFrameDuration =
                combatFeedbackData.freezeFrameDuration;
            feedback.GetFeedbackOfType<MMF_BeautifyChromaticAberration_URP>().intensityValue =
                combatFeedbackData.chromaticAberrationIntensity;
            _damageCollider.TriggerCollider(combatData);
        }

        public void DisableDamageCollider()
        {
            _damageCollider.DisableCollider();
        }
    }
}