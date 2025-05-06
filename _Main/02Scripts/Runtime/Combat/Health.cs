using System;
using Cysharp.Threading.Tasks;
using Main.Runtime.Combat.Core;
using Main.Runtime.Core.StatSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Main.Runtime.Combat
{
    public delegate void HealthChangeHandler(float currentHealth, float minHealth, float maxHealth);

    public class Health : SerializedMonoBehaviour, IDamageable
    {
        public event Action<float> OnApplyDamaged;
        public event Action OnDeath; //죽었을때
        public event HealthChangeHandler OnChangedHealth;

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

        public float MaxHealth { get; private set; }
        public bool IsDead { get; private set; }
        public bool IsInitialized { get; private set; }
        public bool IsInvincibility { get; set; }

        private StatSO _maxHealthStat;
        [SerializeField, ReadOnly] private float _currentHealth;

        public void ResetToMaxHealth()
        {
            CurrentHealth = MaxHealth;
            IsDead = false;
        }

        public virtual async void Init(StatSO maxHealthStat)
        {
            _maxHealthStat = maxHealthStat;
            MaxHealth = _maxHealthStat.Value;
            await UniTask.NextFrame();
            ResetToMaxHealth();
            IsInitialized = true;
        }


        public virtual bool ApplyDamage(GetDamagedInfo getDamagedInfo)
        {
            if (IsDead || IsInvincibility) return false;
            _getDamagedInfo = getDamagedInfo;
            OnApplyDamaged?.Invoke(_getDamagedInfo.damage);
            CurrentHealth -= _getDamagedInfo.damage;
            return true;
        }

        public virtual bool ApplyOnlyDamage(float damage)
        {
            if (IsDead || IsInvincibility) return false;
            CurrentHealth -= damage;
            return true;
        }

        public void SetDeath()
        {
            CurrentHealth = 0;
        }
    }
}