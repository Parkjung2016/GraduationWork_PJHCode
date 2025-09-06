namespace PJH.Runtime.Players
{
    public partial class PlayerAttack
    {
        private void SubscribeEvents()
        {
            _player.OnStartStun += HandleEndCombo;
            _player.OnEndGrabbed += HandleEndCombo;

            _player.PlayerInput.AttackEvent += HandleAttack;
            _player.HealthCompo.OnApplyDamaged += HandleApplyDamaged;

            var animationTriggerCompo = _player.GetCompo<PlayerAnimationTrigger>();
            animationTriggerCompo.OnComboPossible += HandleComboPossible;
            animationTriggerCompo.OnEndCombo += HandleEndCombo;

            PlayerBlock blockCompo = _player.GetCompo<PlayerBlock>();
            blockCompo.OnBlock += HandleBlock;

            _player.GetCompo<PlayerEnemyDetection>().OnChangedTargetEnemy += HandleChangedTargetEnemy;

            _player.GetCompo<PlayerAnimator>().OnEndHitAnimation += HandleEndCombo;
            _player.GetCompo<PlayerEnemyFinisher>().OnFinisherEnd += HandleEndCombo;
            PlayerMovement movementCompo = _player.GetCompo<PlayerMovement>();
            movementCompo.OnEvasion += HandleEndCombo;
            movementCompo.OnEvasionWhileHitting += HandleEndCombo;
            PlayerCommandActionManager commandActionManagerCompo = _player.GetCompo<PlayerCommandActionManager>();
            commandActionManagerCompo.OnUseCommandAction += HandleUseCommandAction;
        }

        private void UnSubscribeEvents()
        {
            _player.OnStartStun -= HandleEndCombo;
            _player.OnEndGrabbed -= HandleEndCombo;

            _player.PlayerInput.AttackEvent -= HandleAttack;
            _player.HealthCompo.OnApplyDamaged -= HandleApplyDamaged;

            PlayerAnimationTrigger animationTriggerCompo = _player.GetCompo<PlayerAnimationTrigger>();
            animationTriggerCompo.OnComboPossible -= HandleComboPossible;
            animationTriggerCompo.OnEndCombo -= HandleEndCombo;
            PlayerBlock blockCompo = _player.GetCompo<PlayerBlock>();
            blockCompo.OnBlock -= HandleBlock;

            PlayerMovement movementCompo = _player.GetCompo<PlayerMovement>();
            movementCompo.OnEvasion -= HandleEndCombo;
            movementCompo.OnEvasionWhileHitting -= HandleEndCombo;
            _player.GetCompo<PlayerEnemyDetection>().OnChangedTargetEnemy -= HandleChangedTargetEnemy;
            _player.GetCompo<PlayerAnimator>().OnEndHitAnimation -= HandleEndCombo;
            _player.GetCompo<PlayerEnemyFinisher>().OnFinisherEnd -= HandleEndCombo;

            PlayerCommandActionManager commandActionManagerCompo = _player.GetCompo<PlayerCommandActionManager>();
            commandActionManagerCompo.OnUseCommandAction -= HandleUseCommandAction;
        }
    }
}