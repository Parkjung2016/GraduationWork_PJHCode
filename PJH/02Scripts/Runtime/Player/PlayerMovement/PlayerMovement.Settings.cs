namespace PJH.Runtime.Players
{
    public partial class PlayerMovement
    {
        private void SubscribeEvents()
        {
            _player.OnStartStun += HandleEndEvasion;
            _player.OnGrabbed += HandleEndEvasion;

            _player.HealthCompo.OnDeath += HandleDeath;
            _player.PlayerInput.RunEvent += HandleRun;
            _player.PlayerInput.EvadeEvent += Evasion;
            _player.PlayerInput.MovementEvent += HandleMovement;
            _player.OnStartKnockBack += HandleStartKnockBack;
            _player.OnEndKnockBack += HandleEndKnockBack;

            PlayerAnimator animatorCompo = _player.GetCompo<PlayerAnimator>();
            animatorCompo.AnimatorMoveEvent += HandleAnimatorMove;
            animatorCompo.OnEndHitAnimation += EndManualMove;
            
            PlayerEnemyFinisher enemyFinisherCompo = _player.GetCompo<PlayerEnemyFinisher>();
            enemyFinisherCompo.OnFinisherEnd += HandleEndEvasion;
            
            PlayerAnimationTrigger animationTriggerCompo = _player.GetCompo<PlayerAnimationTrigger>();
            animationTriggerCompo.OnEndEvasion += HandleEndEvasion;
            animationTriggerCompo.OnBlockEnd += HandleEndEvasion;
            animationTriggerCompo.OnEndCombo += EndManualMove;
            animationTriggerCompo.OnBlockEnd += HandleBlockEnd;
            animationTriggerCompo.OnEndRotatingTargetWhileAttack += HandleEndRotatingTargetWhileAttack;

            _attackCompo.OnAttack += HandleAttack;
        }

        private void UnSubscribeEvents()
        {
            _player.OnStartStun -= HandleEndEvasion;
            _player.OnGrabbed -= HandleEndEvasion;

            _player.HealthCompo.OnDeath -= HandleDeath;
            _player.PlayerInput.RunEvent -= HandleRun;
            _player.PlayerInput.EvadeEvent -= Evasion;

            _player.PlayerInput.MovementEvent -= HandleMovement;
            _player.OnStartKnockBack -= HandleStartKnockBack;
            _player.OnEndKnockBack -= HandleEndKnockBack;

            PlayerAnimator animatorCompo = _player.GetCompo<PlayerAnimator>();
            animatorCompo.AnimatorMoveEvent -= HandleAnimatorMove;
            animatorCompo.OnEndHitAnimation -= EndManualMove;

            PlayerAnimationTrigger animationTriggerCompo = _player.GetCompo<PlayerAnimationTrigger>();
            animationTriggerCompo.OnEndEvasion -= HandleEndEvasion;
            animationTriggerCompo.OnBlockEnd -= HandleEndEvasion;
            animationTriggerCompo.OnEndCombo -= EndManualMove;
            animationTriggerCompo.OnBlockEnd -= HandleBlockEnd;
            animationTriggerCompo.OnEndRotatingTargetWhileAttack -= HandleEndRotatingTargetWhileAttack;


            _attackCompo.OnAttack -= HandleAttack;
        }
    }
}