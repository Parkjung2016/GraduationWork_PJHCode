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

        public virtual void Init(StatSO maxHealthStat, StatSO maxShieldStat)
        {
            _agent = GetComponent<IAgent>();

            _maxShieldStat = maxShieldStat;
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
            Debug.Log(healthRatio);
            CurrentHealth = current * healthRatio;
        }

        private void Update()
        {
            ailmentStat.UpdateAilment();
            if (Input.GetKeyDown(KeyCode.H))
                ApplyHeal(10);
        }

        protected virtual bool CanApplyDamage(GetDamagedInfo getDamagedInfo) => !IsDead && !IsInvincibility;

        public virtual bool ApplyDamage(GetDamagedInfo getDamagedInfo)
        {
            if (!CanApplyDamage(getDamagedInfo)) return false;

            if (CurrentShield > 0)
            {
                CurrentShield -= getDamagedInfo.damage;
                getDamagedInfo.damage = 0;
            }
            else
            {
                GetDamagedInfo? returnValue = SetGetDamagedInfoBeforeApplyDamagedEvent?.Invoke(getDamagedInfo);
                if (returnValue.HasValue)
                    getDamagedInfo = returnValue.Value;
            }

            _getDamagedInfo = getDamagedInfo;
            OnApplyDamaged?.Invoke(_getDamagedInfo.damage);
            CurrentHealth -= _getDamagedInfo.damage;
            return true;
        }

        public virtual bool ApplyOnlyDamage(float damage)
        {
            GetDamagedInfo getDamagedInfo = new();
            getDamagedInfo.damage = damage;
            if (!CanApplyDamage(getDamagedInfo)) return false;
            CurrentHealth -= getDamagedInfo.damage;
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