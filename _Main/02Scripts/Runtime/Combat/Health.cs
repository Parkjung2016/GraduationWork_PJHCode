using System;
using System.Data.SqlTypes;
using Animancer;
using Main.Runtime.Combat.Core;
using Main.Runtime.Core.StatSystem;
using Main.Shared;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Main.Runtime.Combat
{
    public delegate void HealthChangeHandler(float currentHealth, float minHealth, float maxHealth);

    public delegate void ChangedShieldEventHandler(float currentShield, float maxShield);


    public class Health : SerializedMonoBehaviour, IDamageable
    {
        public event Func<GetDamagedInfo, GetDamagedInfo?> SetGetDamagedInfoBeforeApplyDamagedEvent;
        public event Action<float> OnApplyDamaged;
        public event Action<float> OnHeal;
        public event Action OnDeath;
        public event HealthChangeHandler OnChangedHealth;
        public event AilmentChangedEvent OnAilmentChanged;
        public event ChangedShieldEventHandler OnChangedShield;


        protected GetDamagedInfo _getDamagedInfo = new();
        public GetDamagedInfo GetDamagedInfo => _getDamagedInfo;

        public float CurrentHealth
        {
            get => _currentHealth;
            set
            {
                _currentHealth = Mathf.Clamp(value, 0, MaxHealth);
                OnChangedHealth?.Invoke(_currentHealth, 0, MaxHealth);
                if (_currentHealth <= 0 && !IsDead)
                {
                    IsDead = true;
                    ailmentStat.ClearAilment();
                    OnDeath?.Invoke();
                }
            }
        }


        public float CurrentShield
        {
            get => _currentShield;
            set
            {
                _currentShield = Mathf.Clamp(value, 0, _maxShieldStat.Value);
                OnChangedShield?.Invoke(_currentShield, _maxShieldStat.Value);
            }
        }

        public float MaxHealth => _maxHealthStat.Value;
        public bool IsDead { get; private set; }
        public bool IsInitialized { get; private set; }
        public bool IsInvincibility { get; set; }
        public AilmentStat ailmentStat;

        protected StatSO _maxShieldStat;
        protected StatSO _maxHealthStat;

        protected IAgent _agent;
        [SerializeField, ReadOnly] protected float _currentShield;
        [SerializeField, ReadOnly] protected float _currentHealth;

        public void ResetToMaxHealth()
        {
            CurrentHealth = MaxHealth;
            IsDead = false;
        }

        public virtual void Init(IAgent agent, StatSO maxHealthStat, StatSO maxShieldStat)
        {
            _agent = agent;

            _maxShieldStat = maxShieldStat;
            _maxHealthStat = maxHealthStat;
            if (_maxHealthStat)
            {
                ResetToMaxHealth();
                _maxHealthStat.OnValueChange += HandleMaxHealthChanged;
            }

            IsInitialized = true;

            ailmentStat = new AilmentStat();
            ailmentStat.Init();
            ailmentStat.OnAilmentChanged += HandAilmentChangeEvent;
            ailmentStat.OnDotDamage += HandleDotDamageEvent;
        }


        private void OnDestroy()
        {
            if (_maxHealthStat)
                _maxHealthStat.OnValueChange -= HandleMaxHealthChanged;
            if (ailmentStat != null)
            {
                ailmentStat.OnAilmentChanged -= HandAilmentChangeEvent;
                ailmentStat.OnDotDamage -= HandleDotDamageEvent;
            }
        }

        private void HandleMaxHealthChanged(StatSO stat, float current, float prev)
        {
            float healthRatio = CurrentHealth / prev;
            CurrentHealth = current * healthRatio;
        }

        private void Update()
        {
            ailmentStat.UpdateAilment();
        }

        protected virtual bool CanApplyDamage(GetDamagedInfo getDamagedInfo) => !IsDead && !IsInvincibility;

        public virtual bool ApplyDamage(GetDamagedInfo getDamagedInfo)
        {
            _getDamagedInfo = getDamagedInfo;
            if (!CanApplyDamage(getDamagedInfo)) return false;

            if (CurrentShield > 0)
            {
                CurrentShield -= getDamagedInfo.damage;
                _getDamagedInfo.damage = 0;
            }
            else
            {
                GetDamagedInfo? returnValue = SetGetDamagedInfoBeforeApplyDamagedEvent?.Invoke(getDamagedInfo);
                if (returnValue.HasValue)
                    _getDamagedInfo = returnValue.Value;
            }

            OnApplyDamaged?.Invoke(_getDamagedInfo.damage);
            CurrentHealth -= _getDamagedInfo.damage;
            return true;
        }

        public virtual bool ApplyOnlyDamage(float damage)
        {
            GetDamagedInfo getDamagedInfo = new()
            {
                damage = damage,
                attacker = _getDamagedInfo.attacker,
                hitPoint = _getDamagedInfo.hitPoint,
                normal = _getDamagedInfo.normal,
                getUpAnimationClip = _getDamagedInfo.getUpAnimationClip,
                increaseMomentumGauge = _getDamagedInfo.increaseMomentumGauge,
            };
            _getDamagedInfo = getDamagedInfo;

            if (!CanApplyDamage(_getDamagedInfo)) return false;
            if (CurrentShield > 0)
            {
                CurrentShield -= _getDamagedInfo.damage;
                _getDamagedInfo.damage = 0;
            }
            else
            {
                GetDamagedInfo? returnValue = SetGetDamagedInfoBeforeApplyDamagedEvent?.Invoke(_getDamagedInfo);
                if (returnValue.HasValue)
                    _getDamagedInfo = returnValue.Value;
            }

            OnApplyDamaged?.Invoke(_getDamagedInfo.damage);
            CurrentHealth -= _getDamagedInfo.damage;
            return true;
        }

        public virtual bool ApplyHeal(float healAmount)
        {
            if (IsDead) return false;
            OnHeal?.Invoke(healAmount);
            CurrentHealth += healAmount;
            return true;
        }

        private void HandleDotDamageEvent(Ailment ailmentType, float damage, ITransition getDamagedAnimation)
        {
            GetDamagedInfo getDamagedInfo = new GetDamagedInfo
            {
                damage = damage,
                isForceAttack = true,
                ignoreDirection = true,
                getDamagedAnimationClipOnIgnoreDirection = getDamagedAnimation,
                attacker = _agent as MonoBehaviour,
                hitPoint = transform.position
            };
            ApplyDamage(getDamagedInfo);
        }

        private void HandAilmentChangeEvent(Ailment oldAilment, Ailment newAilment)
        {
            OnAilmentChanged?.Invoke(oldAilment, newAilment);
        }

        public void SetDeath()
        {
            CurrentHealth = 0;
        }
    }
}