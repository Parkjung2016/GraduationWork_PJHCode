using System;
using Animancer;
using DG.Tweening;
using Main.Runtime.Agents;
using Main.Shared;
using PJH.Utility.Managers;
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

        [SerializeField, ShowIf("@this._idleAnimation != null")]
        private TransitionAsset _battleEntryAnimation;

        [SerializeField] private bool _enemyAutoRotate;

        [SerializeField, ShowIf("@!this._enemyAutoRotate && this._battleEntryAnimation != null")]
        private bool _enemyAutoRotateWhenBattleEntry;

        [SerializeField, ShowIf("@this._enemyAutoRotate && this._battleEntryAnimation != null")]
        private bool _enemyStopAutoRotateWhenBattleEntry;

        [SerializeField] private bool _hideWeapons;

        [SerializeField, ShowIf("_hideWeapons")]
        private Define.ESocketType[] _hideWeaponSocketType;

        private PoolManagerSO _poolManager;

        private BaseEnemy _currentEnemy;
        private bool _updatePosition;
        private bool _updateRotation;

        private void Awake()
        {
            _updatePosition = true;
            _updateRotation = true;
            _poolManager = AddressableManager.Load<PoolManagerSO>("PoolManager");
        }

        public BaseEnemy SpawnEnemy(PoolTypeSO enemyPoolType)
        {
            BaseEnemy enemy = _poolManager.Pop(enemyPoolType) as BaseEnemy;
            enemy.SetAutoRotate(_enemyAutoRotate);
            if (_enemyAutoRotate)
                _updateRotation = false;
            enemy.GetCompo<EnemyMovement>().SetAiPath(false);
            enemy.BehaviorTreeCompo.enabled = false;
            enemy.HealthCompo.IsInvincibility = true;
            if (_hideWeapons)
            {
                foreach (Define.ESocketType socketType in _hideWeaponSocketType)
                {
                    enemy.GetCompo<AgentEquipmentSystem>().GetSocket(socketType).GetItem<MonoBehaviour>().gameObject
                        .SetActive(false);
                }
            }

            AgentAnimator animatorCompo = enemy.GetCompo<AgentAnimator>(true);
            if (_idleAnimation.IsValid())
                animatorCompo.PlayAnimationClip(_idleAnimation, playControllerOnEnd: false);
            else
                animatorCompo.Animancer.PlayController();
            _currentEnemy = enemy;
            _currentEnemy.transform.SetPositionAndRotation(transform.position, transform.rotation);

            return enemy;
        }

        private void Update()
        {
            if (_updatePosition)
            {
                _currentEnemy.transform.position = transform.position;
            }

            if (_updateRotation)
            {
                _currentEnemy.transform.rotation = transform.rotation;
            }
        }

        public void PrepareForBattle()
        {
            if (!_currentEnemy) return;
            if (!_battleEntryTargetPosition && _battleEntryAnimation == null)
                EnableEnemy();
            else
            {
                Sequence seq = DOTween.Sequence();
                if (_battleEntryTargetPosition != null)
                {
                    _updatePosition = false;
                    _updateRotation = false;
                    _currentEnemy.GetCompo<AgentAnimator>(true).Animator.applyRootMotion = false;
                    seq.Append(
                        _currentEnemy.transform.DOMove(_battleEntryTargetPosition.position,
                            _battleEntryMoveDuration));
                }

                if (_battleEntryAnimation != null)
                {
                    if (_enemyAutoRotate)
                    {
                        if (_enemyStopAutoRotateWhenBattleEntry)
                            _currentEnemy.SetAutoRotate(false);
                    }
                    else if (_enemyAutoRotateWhenBattleEntry)
                    {
                        _currentEnemy.SetAutoRotate(true);
                        _updateRotation = false;
                    }

                    _currentEnemy.GetCompo<AgentAnimator>(true).PlayAnimationClip(_battleEntryAnimation,
                        EnableEnemy, false);
                }
                else
                    seq.OnComplete(EnableEnemy);
            }
        }

        private void EnableEnemy()
        {
            if (_hideWeapons)
            {
                foreach (Define.ESocketType socketType in _hideWeaponSocketType)
                {
                    _currentEnemy.GetCompo<AgentEquipmentSystem>().GetSocket(socketType).GetItem<MonoBehaviour>()
                        .gameObject
                        .SetActive(true);
                }
            }

            _updatePosition = false;
            _updateRotation = false;
            _currentEnemy.HealthCompo.IsInvincibility = false;
            _currentEnemy.BehaviorTreeCompo.enabled = true;
            _currentEnemy.GetCompo<EnemyAnimator>().Animancer.PlayController();
            _currentEnemy.GetCompo<EnemyMovement>().SetAiPath(true);
            _currentEnemy.SetAutoRotate(true);
        }
    }
}