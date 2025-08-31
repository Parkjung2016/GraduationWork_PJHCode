using System;
using Main.Runtime.Agents;
using UnityEngine;

namespace PJH.Runtime.Players
{
    public class PlayerAnimationTrigger : AgentAnimationTrigger
    {
        public Action OnEndCombo;
        public Action OnEndCounterAttack;
        public Action OnEndEvasion;
        public Action OnEndFullMount;
        public Action OnEnableInputWhileRootMotion;
        public event Action OnComboPossible;
        public event Action OnBlockEnd;
        public event Action OnHitFullMountTarget;
        public event Action OnPlayEvasionSound;
        public event Action OnTriggerPassiveAfterAttack;
        public event Action OnWarpTargetAndAttack;
        public event Action OnHitCounterAttack;
        public event Action OnPlayPunchImpactSound;
        public event Action<float> OnFinisherSequenceShake;
        public event Action OnFinisherSequenceTargetDeath, OnFinisherSequenceFinish;
        public event Action OnStopNeckGrabbingSound, OnPlayNeckGrabbingSound, OnPlayNeckGrabSound;
        public event Action OnPlayCharacterFallingOnGroundSound;
        public event Action OnPlayArmGrabSound, OnPlayArmBreakSound;
        public event Action OnEndRotatingTargetWhileAttack;
        public event Action OnHeal;
        public event Action OnGrabHealItem;
        public event Action OnDestroyHealItem;
        public event Action OnPlayUseHealItemSound;
        public event Action OnPlayGrabHealItemSound;
        private Player _player;

        public override void Initialize(Agent agent)
        {
            base.Initialize(agent);
            _player = agent as Player;
        }

        private void ComboPossible()
        {
            OnComboPossible?.Invoke();
        }

        private void EnableInputWhileRootMotion()
        {
            OnEnableInputWhileRootMotion?.Invoke();
        }

        private void EndCombo()
        {
            OnEndCombo?.Invoke();
        }

        private void BlockEnd()
        {
            OnBlockEnd?.Invoke();
        }

        private void EndEvasion()
        {
            OnEndEvasion?.Invoke();
        }

        private void EndFullMount()
        {
            OnEndFullMount?.Invoke();
        }

        private void HitFullMountTarget()
        {
            OnHitFullMountTarget?.Invoke();
        }

        private void PlayEvasionSound()
        {
            OnPlayEvasionSound?.Invoke();
        }

        private void TriggerPassiveAfterAttack()
        {
            OnTriggerPassiveAfterAttack?.Invoke();
        }

        private void WarpTargetAndAttack()
        {
            OnWarpTargetAndAttack?.Invoke();
        }

        private void HitCounterTarget()
        {
            OnHitCounterAttack?.Invoke();
        }

        private void EndRotatingTargetWhileAttack()
        {
            OnEndRotatingTargetWhileAttack?.Invoke();
        }

        private void PlayPunchImpactSound()
        {
            OnPlayPunchImpactSound?.Invoke();
        }

        private void Heal()
        {
            OnHeal?.Invoke();
        }

        private void GrabHealItem()
        {
            OnGrabHealItem?.Invoke();
        }

        private void DestroyHealItem()
        {
            OnDestroyHealItem?.Invoke();
        }

        private void PlayUseHealItemSound()
        {
            OnPlayUseHealItemSound?.Invoke();
        }

        private void PlayGrabHealItemSound()
        {
            OnPlayGrabHealItemSound?.Invoke();
        }

        #region FinisherSequence

        private void StopNeckGrabbingSound()
        {
            OnStopNeckGrabbingSound?.Invoke();
        }

        private void PlayNeckGrabbingSound()
        {
            OnPlayNeckGrabbingSound?.Invoke();
        }

        private void PlayNeckGrabSound()
        {
            OnPlayNeckGrabSound?.Invoke();
        }

        private void PlayCharacterFallingOnGroundSound()
        {
            OnPlayCharacterFallingOnGroundSound?.Invoke();
        }

        private void PlayArmGrabSound()
        {
            OnPlayArmGrabSound?.Invoke();
        }

        private void PlayArmBreakSound()
        {
            OnPlayArmBreakSound?.Invoke();
        }

        private void FinisherSequenceShake(float impulseForce)
        {
            OnFinisherSequenceShake?.Invoke(impulseForce);
        }

        private void FinisherSequenceShake()
        {
            OnFinisherSequenceShake?.Invoke(1);
        }

        private void FinisherSequenceTargetDeath()
        {
            OnFinisherSequenceTargetDeath?.Invoke();
        }

        private void FinisherSequenceFinish()
        {
            OnFinisherSequenceFinish?.Invoke();
        }

        #endregion
    }
}