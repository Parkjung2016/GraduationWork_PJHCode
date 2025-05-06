using System;
using Animancer;
using Main.Runtime.Agents;
using Main.Runtime.Core;
using UnityEngine;


namespace PJH.Runtime.Players
{
    public class PlayerBlock : MonoBehaviour, IAgentComponent, IAfterInitable
    {
        public event Action<bool> OnBlock;
        public bool IsBlocking { get; private set; }

        [field: SerializeField] public float IncreaseTargetMomentumGaugeOnParrying { get; private set; } = 20;
        [field: SerializeField] public float IncreaseMomentumGaugeOnBlock { get; private set; } = 20;
        [field: SerializeField] public float IncreaseMomentumGaugeOnParrying { get; private set; } = 20;
        private Player _player;

        private float _currentBlockingTime;

        public void Initialize(Agent agent)
        {
            _player = agent as Player;
        }

        public void AfterInitialize()
        {
            _player.OnStartStun += HandleBlockEnd;
            _player.PlayerInput.BlockEvent += HandleBlock;
            _player.HealthCompo.OnApplyDamaged += HandleApplyDamaged;
            _player.GetCompo<PlayerAttack>().OnAttack += HandleBlockEnd;
            _player.GetCompo<PlayerAnimationTrigger>().OnBlockEnd += HandleBlockEnd;
            _player.GetCompo<PlayerEnemyFinisher>().OnFinisher += HandleBlockEnd;
            _player.GetCompo<PlayerCounterAttack>().OnCounterAttackWithoutAnimationClip += HandleBlockEnd;

            PlayerMovement movementCompo = _player.GetCompo<PlayerMovement>();
            movementCompo.OnEvasion += HandleBlockEnd;
        }

        private void OnDestroy()
        {
            _player.OnStartStun -= HandleBlockEnd;

            _player.PlayerInput.BlockEvent -= HandleBlock;
            _player.HealthCompo.OnApplyDamaged -= HandleApplyDamaged;
            _player.GetCompo<PlayerAttack>().OnAttack -= HandleBlockEnd;
            _player.GetCompo<PlayerAnimationTrigger>().OnBlockEnd -= HandleBlockEnd;
            _player.GetCompo<PlayerEnemyFinisher>().OnFinisher -= HandleBlockEnd;
            _player.GetCompo<PlayerCounterAttack>().OnCounterAttackWithoutAnimationClip -= HandleBlockEnd;

            PlayerMovement movementCompo = _player.GetCompo<PlayerMovement>();
            movementCompo.OnEvasion -= HandleBlockEnd;
        }

        private void HandleApplyDamaged(float damage)
        {
            HandleBlockEnd();
        }

        private void HandleBlockEnd()
        {
            IsBlocking = false;
        }

        private void HandleBlock(bool isPressedBlockKey)
        {
            if (isPressedBlockKey)
            {
                HybridAnimancerComponent animancer = _player.GetCompo<AgentAnimator>(true).Animancer;
                AnimatorStateInfo stateInfo = animancer.GetCurrentAnimatorStateInfo(0);
                if (stateInfo.IsTag("Blocking")) return;
            }

            PlayerMovement movementCompo = _player.GetCompo<PlayerMovement>();

            if (movementCompo.IsEvading || _player.IsStunned ||
                isPressedBlockKey && _player.IsHitting) return;
            if (isPressedBlockKey)
            {
                IsBlocking = true;
            }

            _currentBlockingTime = Time.time;
            OnBlock?.Invoke(isPressedBlockKey);
        }

        public bool CanParrying()
        {
            bool canParrying = Time.time - _currentBlockingTime <= .2f;
            return canParrying;
        }
    }
}