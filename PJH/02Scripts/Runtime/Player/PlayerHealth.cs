using System;
using Animancer;
using Main.Core;
using Main.Runtime.Combat;
using Main.Runtime.Combat.Core;
using Main.Runtime.Core.Events;
using Main.Runtime.Core.StatSystem;
using Main.Shared;
using UnityEngine;

namespace PJH.Runtime.Players
{
    public class PlayerHealth : Health
    {
        public delegate void ChangedShieldEventHandler(float currentShield, float maxShield);

        public event Func<GetDamagedInfo, GetDamagedInfo?> SetGetDamagedInfoBeforeApplyDamagedEvent;
        public event Action OnAvoidingAttack;
        public event Action OnParrying;
        public event Action OnBlockAttack;
        public event ChangedShieldEventHandler OnChangedShield;

        [SerializeField] private float _blockAttackKnockBackPower;
        [SerializeField] private float _blockAttackKnockBackDuration;
        [SerializeField] private TransitionAsset _blockedAttackByParryingAnimation;

        [SerializeField] private StatSO _increaseHealthStatOnFinisher;
        [SerializeField] private StatSO _maxShieldStat;

        private float _currentShield;

        public float CurrentShield
        {
            get => _currentShield;
            set
            {
                _currentShield = Mathf.Clamp(value, 0, _maxShieldStat.Value);
                OnChangedShield?.Invoke(_currentShield, _maxShieldStat.Value);
            }
        }

        private Player _player;
        private GameEventChannelSO _gameEventChannel;


        public override void Init(StatSO maxHealthStat)
        {
            base.Init(maxHealthStat);
            _player = _agent as Player;
            _gameEventChannel = AddressableManager.Load<GameEventChannelSO>("GameEventChannel");
            PlayerStat statCompo = _player.GetCompo<PlayerStat>();
            _increaseHealthStatOnFinisher = statCompo.GetStat(_increaseHealthStatOnFinisher);
            _maxShieldStat = statCompo.GetStat(_maxShieldStat);
            _gameEventChannel.AddListener<FinishTimeline>(HandleFinishTimeline);
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
            PlayerMovement movementCompo = _player.GetCompo<PlayerMovement>();
            PlayerMomentumGauge momentumGaugeCompo = _player.GetCompo<PlayerMomentumGauge>();
            PlayerBlock blockCompo = _player.GetCompo<PlayerBlock>();
            if (movementCompo.IsEvading)
            {
                OnAvoidingAttack?.Invoke();
                momentumGaugeCompo.DecreaseMomentumGauge(
                    movementCompo.DecreaseMomentumGaugeWhenEvading);
                return false;
            }

            if (blockCompo.IsBlocking && !getDamagedInfo.isForceAttack)
            {
                if (blockCompo.CanParrying())
                {
                    _getDamagedInfo = getDamagedInfo;
                    OnParrying?.Invoke();
                }
                else
                {
                    OnBlockAttack?.Invoke();
                    momentumGaugeCompo.IncreaseMomentumGauge(
                        blockCompo.IncreaseMomentumGaugeOnBlock);
                    _player.KnockBack((getDamagedInfo.attacker as IAgent).ModelTrm.forward, _blockAttackKnockBackPower,
                        _blockAttackKnockBackDuration);
                }

                return false;
            }

            PlayerFullMount fullMountCompo = _player.GetCompo<PlayerFullMount>();
            PlayerCounterAttack counterAttackCompo = _player.GetCompo<PlayerCounterAttack>();

            if (fullMountCompo.IsFullMounting || counterAttackCompo.IsCounterAttacking) return false;

            if (CurrentShield > 0)
            {
                CurrentShield -= getDamagedInfo.damage;
                getDamagedInfo.damage = 0;
            }
            else
            {
                if (SetGetDamagedInfoBeforeApplyDamagedEvent != null)
                {
                    GetDamagedInfo? returnValue = SetGetDamagedInfoBeforeApplyDamagedEvent(getDamagedInfo);
                    if (returnValue.HasValue)
                        getDamagedInfo = returnValue.Value;
                }
            }

            return base.ApplyDamage(getDamagedInfo);
        }
    }
}