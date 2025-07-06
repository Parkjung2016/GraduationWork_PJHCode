using System;
using Main.Runtime.Agents;

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
        public event Action OnPlayAttackWhooshSound;
        public event Action OnBlockEnd;
        public event Action OnHitFullMountTarget;
        public event Action OnPlayEvasionSound;
        public event Action OnTriggerPassiveAfterAttack;
        public event Action OnWarpTargetAndAttack;
        public event Action OnHitCounterAttack;
        public event Action OnFinisherSequenceShake, OnFinisherSequenceTargetDeath, OnFinisherSequenceFinish;
        public event Action OnEndRotatingTargetWhileAttack;
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

        private void PlayAttackWhooshSound()
        {
            OnPlayAttackWhooshSound?.Invoke();
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

        private void FinisherSequenceShake()
        {
            OnFinisherSequenceShake?.Invoke();
        }

        private void FinisherSequenceTargetDeath()
        {
            OnFinisherSequenceTargetDeath?.Invoke();
        }

        private void FinisherSequenceFinish()
        {
            OnFinisherSequenceFinish?.Invoke();
        }

        private void EndRotatingTargetWhileAttack()
        {
            OnEndRotatingTargetWhileAttack?.Invoke();
        }
    }
}