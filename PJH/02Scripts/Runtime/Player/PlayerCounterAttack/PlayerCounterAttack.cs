using System;
using System.Collections.Generic;
using Animancer;
using DG.Tweening;
using FMODUnity;
using Main.Runtime.Agents;
using Main.Runtime.Combat.Core;
using Main.Runtime.Core;
using Main.Runtime.Core.StatSystem;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PJH.Runtime.Players
{
    public class PlayerCounterAttack : MonoBehaviour, IAgentComponent, IAfterInitable
    {
        public event Action<ITransition> OnCounterAttack;
        public event Action OnCounterAttackWithoutAnimationClip;
        public event Action OnHitCounterAttackTarget;
        public bool IsCounterAttacking { get; set; }
        [SerializeField] private List<PlayerCounterAttackDataSO> _counterAttackCombatDatas;
        [SerializeField] private EventReference _counterSound, _targetHitSound;
        [SerializeField] private StatSO _powerStat;

        private Player _player;
        private PlayerCounterAttackDataSO _currentCounterAttackCombatData;

        public void Initialize(Agent agent)
        {
            _player = agent as Player;
        }

        public void AfterInitialize()
        {
            _player.OnStartStun += HandleEndCounterAttack;
            _player.OnGrabbed += HandleEndCounterAttack;
            PlayerAnimationTrigger animationTriggerCompo = _player.GetCompo<PlayerAnimationTrigger>();
            animationTriggerCompo.OnHitCounterAttack += HandleHitCounterAttack;
            animationTriggerCompo.OnEndCounterAttack += HandleEndCounterAttack;
            _powerStat = _player.GetCompo<PlayerStat>().GetStat(_powerStat);

            (_player.HealthCompo as PlayerHealth).OnParrying += HandleParrying;
        }

        private void OnDestroy()
        {
            _player.OnStartStun -= HandleEndCounterAttack;
            _player.OnGrabbed -= HandleEndCounterAttack;
            
            PlayerAnimationTrigger animationTriggerCompo = _player.GetCompo<PlayerAnimationTrigger>();
            animationTriggerCompo.OnHitCounterAttack -= HandleHitCounterAttack;
            animationTriggerCompo.OnEndCounterAttack -= HandleEndCounterAttack;
            (_player.HealthCompo as PlayerHealth).OnParrying -= HandleParrying;
        }

        private void HandleEndCounterAttack()
        {
            IsCounterAttacking = false;
        }

        private void HandleHitCounterAttack()
        {
            Agent target = _player.HealthCompo.GetDamagedInfo.attacker as Agent;
            _player.GetCompo<PlayerEnemyDetection>().SetForceTargetEnemy(target);
            float damage = _powerStat.Value * _currentCounterAttackCombatData.damageMultiplier;
            GetDamagedInfo info = new GetDamagedInfo
            {
                attacker = _player,
                damage = damage,
                ignoreDirection = true,
                getDamagedAnimationClipOnIgnoreDirection = _currentCounterAttackCombatData.getDamagedAnimationClip,
                hitPoint = _player.transform.position
            };
            RuntimeManager.PlayOneShot(_targetHitSound, transform.position);
            OnHitCounterAttackTarget?.Invoke();
            target.HealthCompo.ApplyDamage(info);
        }

        private void HandleParrying()
        {
            IsCounterAttacking = true;

            int idx = Random.Range(0, _counterAttackCombatDatas.Count);
            _currentCounterAttackCombatData = _counterAttackCombatDatas[idx];
            OnCounterAttack?.Invoke(_currentCounterAttackCombatData.attackAnimationClip);
            OnCounterAttackWithoutAnimationClip?.Invoke();
            RuntimeManager.PlayOneShot(_counterSound, transform.position);
            _player.ModelTrm.DOKill();
            Vector3 attackerPosition = _player.HealthCompo.GetDamagedInfo.attacker.transform.position;
            _player.ModelTrm.DOLookAt(attackerPosition, .3f);
        }
    }
}