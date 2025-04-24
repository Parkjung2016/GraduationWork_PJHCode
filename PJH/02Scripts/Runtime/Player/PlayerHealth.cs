using System;
using Animancer;
using Main.Core;
using Main.Runtime.Agents;
using Main.Runtime.Combat;
using Main.Runtime.Combat.Core;
using Main.Runtime.Core.Events;
using Main.Runtime.Core.StatSystem;
using UnityEngine;


namespace PJH.Runtime.Players
{
    public class PlayerHealth : Health
    {
        public delegate void OnChangedShield(float currentShield, float maxShield);

        public event Action OnAvoidingAttack;
        public event Action OnParrying;
        public event Action OnBlockAttack;
        public event Action<float, float> ChangedShieldEvent;

        [SerializeField] private float _blockAttackKnockBackPower;
        [SerializeField] private float _blockAttackKnockBackDuration;
        [SerializeField] private TransitionAsset _blockedAttackByParryingAnimation;

        [SerializeField] private StatSO _increaseHealthStatOnFinisher;
        [SerializeField] private StatSO _shieldStat;

        private float _currentShield;

        public float CurrentShield
        {
            get => _currentShield;
            set
            {
                _currentShield = Mathf.Clamp(value, 0, _shieldStat.Value);
                ChangedShieldEvent?.Invoke(_currentShield, _shieldStat.Value);
            }
        }

        private Player _player;
        private PlayerMovement _movementCompo;
        private PlayerBlock _blockCompo;
        private PlayerFullMount _fullMountCompo;
        private AgentMomentumGauge _momentumGaugeCompo;

        private GameEventChannelSO _gameEventChannel;

        private void Awake()
        {
            _player = GetComponent<Player>();
        }

        public override void Init(StatSO maxHealthStat)
        {
            base.Init(maxHealthStat);
            _gameEventChannel = AddressableManager.Load<GameEventChannelSO>("GameEventChannel");
            _momentumGaugeCompo = _player.GetCompo<AgentMomentumGauge>(true);
            _movementCompo = _player.GetCompo<PlayerMovement>();
            PlayerStat statCompo = _player.GetCompo<PlayerStat>();
            _increaseHealthStatOnFinisher = statCompo.GetStat(_increaseHealthStatOnFinisher);
            _shieldStat = statCompo.GetStat(_shieldStat);
            _fullMountCompo = _player.GetCompo<PlayerFullMount>();
            _blockCompo = _player.GetCompo<PlayerBlock>();

            _gameEventChannel.AddListener<FinishTimeline>(HandleFinishTimeline);

            CurrentShield = _shieldStat.Value;
        }

        private void OnDestroy()
        {
            _gameEventChannel.RemoveListener<FinishTimeline>(HandleFinishTimeline);
        }

        private void HandleFinishTimeline(FinishTimeline evt)
        {
            CurrentHealth += _increaseHealthStatOnFinisher.Value;
        }

        public override bool ApplyDamage(GetDamagedInfo getDamagedInfo)
        {
            if (_movementCompo.IsEvading)
            {
                OnAvoidingAttack?.Invoke();
                _momentumGaugeCompo.DecreaseMomentumGauge(
                    _movementCompo.DecreaseMomentumGaugeWhenEvading);
                return false;
            }

            if (_blockCompo.IsBlocking && !getDamagedInfo.isForceAttack)
            {
                if (_blockCompo.CanParrying())
                {
                    OnParrying?.Invoke();

                    (getDamagedInfo.attacker as IParryingable).ParryingSuccess(
                        _blockCompo.IncreaseTargetMomentumGaugeOnParrying, _blockedAttackByParryingAnimation);
                    _momentumGaugeCompo.IncreaseMomentumGauge(
                        _blockCompo.IncreaseMomentumGaugeOnParrying);
                }
                else
                {
                    OnBlockAttack?.Invoke();
                    _momentumGaugeCompo.IncreaseMomentumGauge(
                        _blockCompo.IncreaseMomentumGaugeOnBlock);
                    _player.KnockBack(getDamagedInfo.attacker.ModelTrm.forward, _blockAttackKnockBackPower,
                        _blockAttackKnockBackDuration);
                }

                return false;
            }

            if (_fullMountCompo.IsFullMounting) return false;
            // _player.ModelTrm

            if (CurrentShield > 0)
            {
                CurrentShield -= getDamagedInfo.damage;
                getDamagedInfo.damage = 0;
            }

            return base.ApplyDamage(getDamagedInfo);
        }
    }
}