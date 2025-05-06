using System;
using System.Collections.Generic;
using Animancer;
using DG.Tweening;
using Main.Runtime.Agents;
using Main.Runtime.Combat.Core;
using Main.Runtime.Core;
using Main.Runtime.Core.StatSystem;
using MoreMountains.Feedbacks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PJH.Runtime.Players
{
    public class PlayerCounterAttack : MonoBehaviour, IAgentComponent, IAfterInitable
    {
        public event Action<ITransition> OnCounterAttack;
        public event Action OnCounterAttackWithoutAnimationClip;

        public bool IsCounterAttacking { get; private set; }
        [SerializeField] private List<PlayerCounterAttackDataSO> _counterAttackCombatDatas;
        [SerializeField] private StatSO _powerStat;

        private MMF_Player _hitFeedback, _onCounterAttackFeedback;
        private Player _player;
        private PlayerCounterAttackDataSO _currentCounterAttackCombatData;

        public void Initialize(Agent agent)
        {
            _player = agent as Player;
            _hitFeedback = transform.Find("HitFeedback").GetComponent<MMF_Player>();
            _onCounterAttackFeedback = transform.Find("OnCounterAttackFeedback").GetComponent<MMF_Player>();
        }

        public void AfterInitialize()
        {
            PlayerAnimationTrigger animationTriggerCompo = _player.GetCompo<PlayerAnimationTrigger>();
            animationTriggerCompo.OnHitCounterAttack += HandleHitCounterAttack;
            animationTriggerCompo.OnEndCounterAttack += HandleEndCounterAttack;
            _powerStat = _player.GetCompo<PlayerStat>().GetStat(_powerStat);

            (_player.HealthCompo as PlayerHealth).OnParrying += HandleParrying;
        }

        private void OnDestroy()
        {
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
            GetDamagedInfo info = new GetDamagedInfo
            {
                attacker = _player,
                damage = _powerStat.Value,
                getDamagedAnimationClip = _currentCounterAttackCombatData.getDamagedAnimationClip
            };
            _hitFeedback.PlayFeedbacks();

            target.HealthCompo.ApplyDamage(info);
        }

        private void HandleParrying()
        {
            IsCounterAttacking = true;

            int idx = Random.Range(0, _counterAttackCombatDatas.Count);
            _currentCounterAttackCombatData = _counterAttackCombatDatas[idx];
            OnCounterAttack?.Invoke(_currentCounterAttackCombatData.attackAnimationClip);
            _onCounterAttackFeedback.PlayFeedbacks();
            OnCounterAttackWithoutAnimationClip?.Invoke();
            _player.ModelTrm.DOKill();
            Vector3 attackerPosition = _player.HealthCompo.GetDamagedInfo.attacker.GameObject.transform.position;
            _player.ModelTrm.DOLookAt(attackerPosition, .3f);
        }
    }
}