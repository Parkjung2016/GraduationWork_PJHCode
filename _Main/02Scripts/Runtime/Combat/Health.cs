using System;
using Animancer;
using Cysharp.Threading.Tasks;
using Main.Runtime.Combat.Core;
using Main.Runtime.Core.StatSystem;
using Main.Shared;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Main.Runtime.Combat
{
    public delegate void HealthChangeHandler(float currentHealth, float minHealth, float maxHealth);

    public class Health : SerializedMonoBehaviour, IDamageable
    {
        public event Action<float> OnApplyDamaged;
        public event Action<float> OnHeal;
        public event Action OnDeath;
        public event HealthChangeHandler OnChangedHealth;
        public event AilmentChangedEvent OnAilmentChanged;


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
                    OnDeath?.Invoke();
                }
            }
        }

        public float MaxHealth => _maxHealthStat.Value;
        public bool IsDead { get; private set; }
        public bool IsInitialized { get; private set; }
        public bool IsInvincibility { get; set; }
        public AilmentStat ailmentStat;

        private StatSO _maxHealthStat;

        protected IAgent _agent;
        [SerializeField, ReadOnly] private float _currentHealth;

        public void ResetToMaxHealth()
        {
            CurrentHealth = MaxHealth;
            IsDead = false;
        }

        public virtual async void Init(StatSO maxHealthStat)
        {
            _agent = GetComponent<IAgent>();
            _maxHealthStat = maxHealthStat;
            ResetToMaxHealth();
            IsInitialized = true;

            ailmentStat = new AilmentStat();
            ailmentStat.OnAilmentChanged += HandAilmentChangeEvent;
            ailmentStat.OnDotDamage += HandleDotDamageEvent;

            _maxHealthStat.OnValueChange += HandleMaxHealthChanged;
        }


        private void OnDestroy()
        {
            if (_maxHealthStat)
                _maxHealthStat.OnValueChange -= HandleMaxHealthChanged;

            ailmentStat.OnAilmentChanged -= HandAilmentChangeEvent;
            ailmentStat.OnDotDamage -= HandleDotDamageEvent;
            ailmentStat = null;
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

        protected virtual bool CanApplyDamage() => IsDead || IsInvincibility;

        public virtual bool ApplyDamage(GetDamagedInfo getDamagedInfo)
        {
            if (CanApplyDamage()) return false;
            _getDamagedInfo = getDamagedInfo;
            OnApplyDamaged?.Invoke(_getDamagedInfo.damage);
            CurrentHealth -= _getDamagedInfo.damage;
            return true;
        }

        public virtual bool ApplyOnlyDamage(float damage)
        {
            if (CanApplyDamage()) return false;
            CurrentHealth -= damage;
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