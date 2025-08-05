using System;
using Animancer;
using Main.Core;
using Main.Runtime.Combat;
using Main.Runtime.Combat.Core;
using Main.Runtime.Core.Events;
using Main.Runtime.Core.StatSystem;
using Main.Runtime.Manager;
using Main.Shared;
using UnityEngine;

namespace PJH.Runtime.Players
{
    public class PlayerHealth : Health
    {
        public event Action OnAvoidingAttack;
        public event Action OnParrying;
        public event Action OnBlockAttack;

        [SerializeField] private float _blockAttackKnockBackPower;
        [SerializeField] private float _blockAttackKnockBackDuration;
        [SerializeField] private TransitionAsset _blockedAttackByParryingAnimation;

        [SerializeField] private StatSO _increaseHealthStatOnFinisher;

        private Player _player;
        private GameEventChannelSO _gameEventChannel;


        public override void Init(StatSO maxHealthStat, StatSO maxShieldStat)
        {
            base.Init(maxHealthStat, maxShieldStat);
            _player = _agent as Player;
            _gameEventChannel = AddressableManager.Load<GameEventChannelSO>("GameEventChannel");
            PlayerStat statCompo = _player.GetCompo<PlayerStat>();
            _increaseHealthStatOnFinisher = statCompo.GetStat(_increaseHealthStatOnFinisher);
            _gameEventChannel.AddListener<FinishEnemyFinisher>(HandleFinishTimeline);
            OnChangedHealth += HandleChangedHealth;
        }

        private void OnDestroy()
        {
            _gameEventChannel.RemoveListener<FinishEnemyFinisher>(HandleFinishTimeline);
            OnChangedHealth -= HandleChangedHealth;
        }

        private void HandleChangedHealth(float currentHealth, float minHealth, float maxHealth)
        {
            if (currentHealth <= maxHealth * 0.25f)
            {
                Managers.FMODManager.SetBeforePlayerDead(true);
            }
            else
            {
                Managers.FMODManager.SetBeforePlayerDead(false);
            }
        }

        private void HandleFinishTimeline(FinishEnemyFinisher evt)
        {
            CurrentHealth += _increaseHealthStatOnFinisher.Value;
        }

        protected override bool CanApplyDamage(GetDamagedInfo getDamagedInfo)
        {
            if (!base.CanApplyDamage(getDamagedInfo)) return false;
            _getDamagedInfo = getDamagedInfo;
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

            return true;
        }
    }
}