using Main.Core;
using Main.Runtime.Agents;
using Main.Runtime.Core;
using Main.Runtime.Core.Events;
using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using TrailsFX;
using UnityEngine;


namespace PJH.Runtime.Players
{
    public class PlayerEffect : AgentEffect
    {
        [SerializeField] private PoolTypeSO _parryingEffectPoolType;
        [SerializeField] private TrailEffect _meshTrailEffect;
        private Player _player;
        private GameEventChannelSO _spawnEventChannel;
        private GameEventChannelSO _gameEventChannel;
        private MMF_Player _applyDamagedFeedback, _evasionFeedbackWhileHitting;
        private MMF_Player _counterAttackFeedback, _hitCounterAttackTargetFeedback;
        private MMF_Player _hitWarpStrikeTargetFeedback, _avoidingAttackFeedback;

        public override void Initialize(Agent agent)
        {
            base.Initialize(agent);
            _counterAttackFeedback = transform.Find("CounterAttackFeedback").GetComponent<MMF_Player>();
            _hitCounterAttackTargetFeedback =
                transform.Find("HitCounterAttackTargetFeedback").GetComponent<MMF_Player>();
            _applyDamagedFeedback = transform.Find("ApplyDamagedFeedback").GetComponent<MMF_Player>();
            _hitWarpStrikeTargetFeedback = transform.Find("HitWarpStrikeTargetFeedback").GetComponent<MMF_Player>();
            _evasionFeedbackWhileHitting = transform.Find("EvasionFeedbackWhileHitting").GetComponent<MMF_Player>();
            _avoidingAttackFeedback = transform.Find("AvoidingAttackFeedback").GetComponent<MMF_Player>();
            _gameEventChannel = AddressableManager.Load<GameEventChannelSO>("GameEventChannel");
            _spawnEventChannel = AddressableManager.Load<GameEventChannelSO>("SpawnEventChannel");
            _player = agent as Player;
            _meshTrailEffect.active = false;
        }

        public override void AfterInitialize()
        {
            base.AfterInitialize();
            _player.HealthCompo.OnApplyDamaged += HandleApplyDamaged;

            _player.OnStartStun += HandleStartStun;
            _player.OnEndStun += HandleEndStun;

            PlayerHealth playerHealth = (_player.HealthCompo as PlayerHealth);
            playerHealth.OnParrying += HandleParrying;
            playerHealth.OnAvoidingAttack += HandleAvoidingAttack;
            PlayerMovement movementCompo = _player.GetCompo<PlayerMovement>();
            movementCompo.OnEvasionWhileHitting += HandleEvasionWhileHitting;
            movementCompo.OnEvasionEndWhileHitting += HandleEvasionEndWhileHitting;

            PlayerCounterAttack counterAttackCompo = _player.GetCompo<PlayerCounterAttack>();
            counterAttackCompo.OnCounterAttackWithoutAnimationClip += HandleCounterAttack;
            counterAttackCompo.OnHitCounterAttackTarget += HandleHitCounterAttackTarget;

            PlayerWarpStrike warpStrikeCompo = _player.GetCompo<PlayerWarpStrike>();
            warpStrikeCompo.OnHitWarpStrikeTarget += HandleHitWarpStrikeTarget;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _player.HealthCompo.OnApplyDamaged -= HandleApplyDamaged;
            _player.OnStartStun -= HandleStartStun;
            _player.OnEndStun -= HandleEndStun;


            PlayerHealth playerHealth = (_player.HealthCompo as PlayerHealth);
            playerHealth.OnParrying -= HandleParrying;
            playerHealth.OnAvoidingAttack -= HandleAvoidingAttack;

            PlayerMovement movementCompo = _player.GetCompo<PlayerMovement>();
            movementCompo.OnEvasionWhileHitting -= HandleEvasionWhileHitting;
            movementCompo.OnEvasionEndWhileHitting -= HandleEvasionEndWhileHitting;

            PlayerCounterAttack counterAttackCompo = _player.GetCompo<PlayerCounterAttack>();
            counterAttackCompo.OnCounterAttackWithoutAnimationClip -= HandleCounterAttack;
            counterAttackCompo.OnHitCounterAttackTarget -= HandleHitCounterAttackTarget;

            PlayerWarpStrike warpStrikeCompo = _player.GetCompo<PlayerWarpStrike>();
            warpStrikeCompo.OnHitWarpStrikeTarget -= HandleHitWarpStrikeTarget;
        }

        private void HandleAvoidingAttack()
        {
            if (!_avoidingAttackFeedback) return;
            if (_avoidingAttackFeedback.IsPlaying)
                _avoidingAttackFeedback.StopFeedbacks();
            _avoidingAttackFeedback.PlayFeedbacks();
        }

        private void HandleHitWarpStrikeTarget()
        {
            _hitWarpStrikeTargetFeedback?.PlayFeedbacks();
        }

        private void HandleHitCounterAttackTarget()
        {
            _hitCounterAttackTargetFeedback?.PlayFeedbacks();
        }

        private void HandleCounterAttack()
        {
            _counterAttackFeedback?.PlayFeedbacks();
        }

        private void HandleEvasionWhileHitting()
        {
            _evasionFeedbackWhileHitting?.PlayFeedbacks();
            _meshTrailEffect.active = true;
        }

        private void HandleEvasionEndWhileHitting()
        {
            _meshTrailEffect.active = false;
        }

        private void HandleStartStun()
        {
            var evt = GameEvents.PlayerStunned;
            evt.isStunned = true;
            _gameEventChannel.RaiseEvent(evt);
        }

        private void HandleEndStun()
        {
            var evt = GameEvents.PlayerStunned;
            evt.isStunned = false;
            _gameEventChannel.RaiseEvent(evt);
        }


        private void HandleParrying()
        {
            var evt = SpawnEvents.SpawnEffect;
            evt.effectType = _parryingEffectPoolType;
            evt.position = _player.HealthCompo.GetDamagedInfo.hitPoint;
            evt.rotation = Quaternion.identity;
            _spawnEventChannel.RaiseEvent(evt);
        }

        private void HandleApplyDamaged(float damage)
        {
            _applyDamagedFeedback.PlayFeedbacks();
        }
    }
}