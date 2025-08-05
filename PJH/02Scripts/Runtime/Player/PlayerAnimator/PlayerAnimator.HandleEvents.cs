using Animancer;

namespace PJH.Runtime.Players
{
    public partial class PlayerAnimator
    {
        private void HandleEndStun()
        {
            DisableRootMotion();
            SetParam(_isStunnedParam, false);
        }

        private void HandleStartStun()
        {
            SetParam(_isStunnedParam, true);
        }

        private void HandleExitBattle()
        {
            SetParam(_isInBattleParam, false);
        }

        private void HandleEnterBattle()
        {
            SetParam(_isInBattleParam, true);
        }

        private void HandleEvasion(ITransition evasionAnimation, ITransition animationAfterEvasion)
        {
            EnableRootMotion(true);
            PlayAnimationClip(evasionAnimation, () =>
            {
                EnableRootMotion(false);
                PlayerAnimationTrigger playerAnimationTriggerCompo = _player.GetCompo<PlayerAnimationTrigger>();

                playerAnimationTriggerCompo.OnEndEvasion?.Invoke();
            });
        }

        private void HandleBlock(bool isPressedBlockKey)
        {
            if (isPressedBlockKey)
            {
                EnableRootMotion(true);
                _hybridAnimancer.CrossFadeInFixedTime("BlockStart", .1f);
            }

            SetParam(_isBlockingParam, isPressedBlockKey);
        }

        private void HandleTurn(ITransition clip)
        {
            EnableRootMotion(true);
            PlayAnimationClip(clip, () => EnableRootMotion(false));
        }


        protected override void HandleDeath()
        {
            EnableRootMotion(true);
            base.HandleDeath();
        }

        protected override void HandleApplyDamaged(float damage)
        {
            EnableRootMotion(true);
            base.HandleApplyDamaged(damage);
        }

        // private void HandleFinisherTimeline(bool isFinishering)
        //
        // {
        //     _isPlayingTimeline = isFinishering;
        // }

        private void HandleAttack()
        {
            EnableRootMotion(!_attackCompo.CurrentCombatData.isManualMove);

            PlayerCombatDataSO combatData = _attackCompo.CurrentCombatData;
            combatData.SetAttackOverrideSpeed(_attackSpeedStat.Value);
            PlayAnimationClip(combatData.attackAnimationClip, () =>
            {
                if (!_attackCompo.CurrentCombatData.isManualMove)
                    EnableRootMotion(false);
                PlayerAnimationTrigger playerAnimationTriggerCompo = _player.GetCompo<PlayerAnimationTrigger>();

                playerAnimationTriggerCompo.OnEndCombo?.Invoke();
            });
        }

        private void HandleEnableInputWhileRootMotion()
        {
            IsEnabledInputWhileRootMotion = true;
        }

        private void HandleWarpStrikeAttack(ITransition animationClip)
        {
            PlayAnimationClip(animationClip, () => { EnableRootMotion(false); });
        }

        private void HandleCounterAttack(ITransition animationClip)
        {
            EnableRootMotion(true);

            PlayAnimationClip(animationClip, () =>
            {
                PlayerAnimationTrigger playerAnimationTriggerCompo = _player.GetCompo<PlayerAnimationTrigger>();
                playerAnimationTriggerCompo.OnEndCounterAttack?.Invoke();

                EnableRootMotion(false);
            });
        }
    }
}