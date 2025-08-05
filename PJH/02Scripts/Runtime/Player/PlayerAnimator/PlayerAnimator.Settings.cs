namespace PJH.Runtime.Players
{
    public partial class PlayerAnimator
    {
        private void SubscribeEvents()
        {
            PlayerAnimationTrigger animationTriggerCompo = _player.GetCompo<PlayerAnimationTrigger>();
            animationTriggerCompo.OnEnableInputWhileRootMotion += HandleEnableInputWhileRootMotion;
            animationTriggerCompo.OnBlockEnd += DisableRootMotion;

            _attackCompo.OnAttack += HandleAttack;
            _attackCompo.OnEnterBattle += HandleEnterBattle;
            _attackCompo.OnExitBattle += HandleExitBattle;

            _movementCompo.OnTurn += HandleTurn;
            _movementCompo.OnEvasionWithAnimation += HandleEvasion;

            _blockCompo.OnBlock += HandleBlock;

            _player.OnStartStun += HandleStartStun;
            _player.OnEndStun += HandleEndStun;
            OnEndHitAnimation += DisableRootMotion;

            _player.GetCompo<PlayerWarpStrike>().OnWarpStrikeAttack += HandleWarpStrikeAttack;

            _player.GetCompo<PlayerCounterAttack>().OnCounterAttack += HandleCounterAttack;
            _player.GetCompo<PlayerEnemyFinisher>().OnFinisherEnd += DisableRootMotion;
        }

        private void UnSubscribeEvents()
        {
            PlayerAnimationTrigger animationTriggerCompo = _player.GetCompo<PlayerAnimationTrigger>();
            animationTriggerCompo.OnEnableInputWhileRootMotion -= HandleEnableInputWhileRootMotion;
            animationTriggerCompo.OnBlockEnd -= DisableRootMotion;

            _attackCompo.OnAttack -= HandleAttack;
            _attackCompo.OnEnterBattle -= HandleEnterBattle;
            _attackCompo.OnExitBattle -= HandleExitBattle;

            PlayerMovement movementCompo = _player.GetCompo<PlayerMovement>();
            movementCompo.OnTurn -= HandleTurn;
            movementCompo.OnEvasionWithAnimation -= HandleEvasion;
            _blockCompo.OnBlock -= HandleBlock;

            _player.OnStartStun -= HandleStartStun;
            _player.OnEndStun -= HandleEndStun;

            // _player.GetCompo<PlayerEnemyFinisher>().OnFinisherTimeline -= HandleFinisherTimeline;
            OnEndHitAnimation -= DisableRootMotion;

            _player.GetCompo<PlayerWarpStrike>().OnWarpStrikeAttack -= HandleWarpStrikeAttack;

            _player.GetCompo<PlayerCounterAttack>().OnCounterAttack -= HandleCounterAttack;
            _player.GetCompo<PlayerEnemyFinisher>().OnFinisherEnd -= DisableRootMotion;
            
        }
    }
}