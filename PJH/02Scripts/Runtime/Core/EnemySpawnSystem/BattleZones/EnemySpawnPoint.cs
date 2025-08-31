using Animancer;
using DG.Tweening;
using Main.Core;
using Main.Runtime.Agents;
using Sirenix.OdinInspector;
using UnityEngine;
using YTH.Enemies;

namespace PJH.Runtime.Core.EnemySpawnSystem
{
    public class EnemySpawnPoint : MonoBehaviour
    {
        [SerializeField,] private Transform _battleEntryTargetPosition;

        [SerializeField, ShowIf("@this._battleEntryTargetPosition != null")]
        private float _battleEntryMoveDuration = .2f;

        [SerializeField] private TransitionAsset _idleAnimation;

        [SerializeField, ShowIf("@this._idleAnimation.IsValid")]
        private TransitionAsset _battleEntryAnimation;

        [SerializeField] private bool _enemyAutoRotate;

        [SerializeField, ShowIf("@!this._enemyAutoRotate && this._battleEntryAnimation.IsValid")]
        private bool _enemyAutoRotateWhenBattleEntry;

        [SerializeField, ShowIf("@this._enemyAutoRotate && this._battleEntryAnimation.IsValid")]
        private bool _enemyStopAutoRotateWhenBattleEntry;

        private PoolManagerSO _poolManager;

        private BaseEnemy _currentEnemy;

        private void Awake()
        {
            _poolManager = AddressableManager.Load<PoolManagerSO>("PoolManager");
        }

        public BaseEnemy SpawnEnemy(PoolTypeSO enemyPoolType)
        {
            BaseEnemy enemy = _poolManager.Pop(enemyPoolType) as BaseEnemy;
            enemy.SetAutoRotate(_enemyAutoRotate);
            enemy.GetCompo<EnemyMovement>().SetAiPath(false);
            enemy.transform.SetPositionAndRotation(transform.position, transform.rotation);
            enemy.BehaviorTreeCompo.enabled = false;
            enemy.HealthCompo.IsInvincibility = true;
            AgentAnimator animatorCompo = enemy.GetCompo<AgentAnimator>(true);
            if (_idleAnimation.IsValid())
                animatorCompo.PlayAnimationClip(_idleAnimation, playControllerOnEnd: false);
            else
                animatorCompo.Animancer.PlayController();
            _currentEnemy = enemy;
            return enemy;
        }

        public void PrepareForBattle()
        {
            if (!_currentEnemy) return;
            if (!_battleEntryTargetPosition && !_battleEntryAnimation.IsValid())
                EnableEnemy();
            else
            {
                Sequence seq = DOTween.Sequence();
                if (_battleEntryTargetPosition)
                    seq.Append(
                        _currentEnemy.transform.DOMove(_battleEntryTargetPosition.position, _battleEntryMoveDuration));
                if (_battleEntryAnimation.IsValid())
                {
                    if (_enemyAutoRotate)
                    {
                        if (_enemyStopAutoRotateWhenBattleEntry)
                            _currentEnemy.SetAutoRotate(false);
                    }
                    else if (_enemyAutoRotateWhenBattleEntry)
                        _currentEnemy.SetAutoRotate(true);

                    _currentEnemy.GetCompo<AgentAnimator>(true).PlayAnimationClip(_battleEntryAnimation,
                        EnableEnemy, false);
                }
                else
                    seq.OnComplete(EnableEnemy);
            }
        }

        private void EnableEnemy()
        {
            _currentEnemy.HealthCompo.IsInvincibility = false;
            _currentEnemy.BehaviorTreeCompo.enabled = true;
            _currentEnemy.GetCompo<EnemyAnimator>().Animancer.PlayController();
            _currentEnemy.GetCompo<EnemyMovement>().SetAiPath(true);
            _currentEnemy.SetAutoRotate(true);
        }
    }
}