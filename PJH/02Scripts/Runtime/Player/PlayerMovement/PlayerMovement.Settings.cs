namespace PJH.Runtime.Players
{
    public partial class PlayerMovement
    {
        private void SubscribeEvents()
        {
            _player.HealthCompo.OnDeath += HandleDeath;
            _player.PlayerInput.RunEvent += HandleRun;
            _player.PlayerInput.EvadeEvent += HandleEvade;
            _player.PlayerInput.MovementEvent += HandleMovement;
            _player.OnStartKnockBack += HandleStartKnockBack;
            _player.OnEndKnockBack += HandleEndKnockBack;

            _player.GetCompo<PlayerEnemyFinisher>().OnFinisherTimeline += HandleFinisherTimeline;
            PlayerAnimator animatorCompo = _player.GetCompo<PlayerAnimator>();
            animatorCompo.AnimatorMoveEvent += HandleAnimatorMove;
            animatorCompo.OnEndHitAnimation += EndManualMove;
            
            PlayerAnimationTrigger animationTriggerCompo = _player.GetCompo<PlayerAnimationTrigger>();
            animationTriggerCompo.OnEndEvasion += HandleEndEvasion;
            animationTriggerCompo.OnBlockEnd += HandleEndEvasion;
            animationTriggerCompo.OnEndCombo += EndManualMove;
            animationTriggerCompo.OnBlockEnd += HandleBlockEnd;
            animationTriggerCompo.OnEndFullMount += HandleEndFullMount;
            PlayerFullMount fullMountCompo = _player.GetCompo<PlayerFullMount>();
            fullMountCompo.OnFullMount += HandleFullMount;

            _attackCompo.OnAttack += CheckManualMove;
        }

        private void UnSubscribeEvents()
        {
            _player.HealthCompo.OnDeath -= HandleDeath;
            _player.PlayerInput.RunEvent -= HandleRun;
            _player.PlayerInput.EvadeEvent -= HandleEvade;

            _player.PlayerInput.MovementEvent -= HandleMovement;
            _player.OnStartKnockBack -= HandleStartKnockBack;
            _player.OnEndKnockBack -= HandleEndKnockBack;

            _player.GetCompo<PlayerEnemyFinisher>().OnFinisherTimeline -= HandleFinisherTimeline;
            PlayerAnimator animatorCompo = _player.GetCompo<PlayerAnimator>();
            animatorCompo.AnimatorMoveEvent -= HandleAnimatorMove;
            animatorCompo.OnEndHitAnimation -= EndManualMove;

            PlayerAnimationTrigger animationTriggerCompo = _player.GetCompo<PlayerAnimationTrigger>();
            animationTriggerCompo.OnEndEvasion -= HandleEndEvasion;
            animationTriggerCompo.OnBlockEnd -= HandleEndEvasion;
            animationTriggerCompo.OnEndCombo -= EndManualMove;
            animationTriggerCompo.OnBlockEnd -= HandleBlockEnd;
            animationTriggerCompo.OnEndFullMount -= HandleEndFullMount;

            PlayerFullMount fullMountCompo = _player.GetCompo<PlayerFullMount>();
            fullMountCompo.OnFullMount -= HandleFullMount;

            _attackCompo.OnAttack -= CheckManualMove;
        }
    }
}