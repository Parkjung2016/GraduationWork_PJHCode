using System;
using FMODUnity;
using Main.Core;
using Main.Runtime.Combat;
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
        [SerializeField] private PoolTypeSO _hitImpactPoolType;
        private WeaponFracture _fractureCompo;
        private Rigidbody _rigidbodyCompo;
        private Collider _meshCollider;
        private DamageCollider _damageCollider;
        private StatSO _powerStat, _increaseMomentumGaugeStat;
        private int _currentDurability;

        public int CurrentDurability
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
        }

        public void Equip(IAgent owner, StatSO powerStat, StatSO increaseMomentumGaugeStat)
        {
            base.Equip(owner);
            _powerStat = powerStat;
            _increaseMomentumGaugeStat = increaseMomentumGaugeStat;
            _damageCollider = GetComponentInChildren<DamageCollider>();
            float power = _powerStat.Value * WeaponData.damageMultiplier;
            float increaseMomentumGauge = _increaseMomentumGaugeStat.Value * WeaponData.increaseMomentumGaugeMultiplier;
            _damageCollider.Init(owner, power, increaseMomentumGauge);
            _damageCollider.OnDamageTrigger += HandleDamageTrigger;
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

        private void HandleDamageTrigger(Collider hitTarget, HitInfo hitInfo)
        {
            CurrentDurability -= 1;

            _owner.OnHitTarget?.Invoke(hitInfo);
            RuntimeManager.PlayOneShot(WeaponData.hitImpactSound, transform.position);
            if (!_hitImpactPoolType) return;
            Vector3 effectPosition = hitTarget.ClosestPoint(transform.position);
            var evt = SpawnEvents.SpawnEffect;
            evt.effectType = _hitImpactPoolType;
            evt.position = effectPosition;
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
            _damageCollider.TriggerCollider(combatData);
        }

        public void DisableDamageCollider()
        {
            _damageCollider.DisableCollider();
        }
    }
}