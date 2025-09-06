using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Main.Runtime.Agents;
using Main.Runtime.Core;
using Main.Runtime.Core.Events;
using Main.Shared;
using PJH.Utility.Managers;
using UnityEngine;
using YTH.Shared;
using ZLinq;

namespace PJH.Runtime.Players
{
    public class PlayerEnemyDetection : MonoBehaviour, IAgentComponent, IAfterInitable
    {
        public delegate void ChangedTargetEnemyEvent(Agent prevTarget, Agent currentTarget);

        public event ChangedTargetEnemyEvent OnChangedTargetEnemy;
        public event Action<Agent> OnChangedHitTargetEnemy;

        [SerializeField] private float _detectionRadius = 5f;
        [SerializeField] private float _detectInterval = 0.05f;
        [SerializeField] private int _maxDetectCount = 5;

        private Agent _target;
        private Collider[] _detectColliders;
        private Player _player;
        private CancellationTokenSource _cancellationToken;
        private GameEventChannelSO _showLockOnUIEventChannel;

        public void Initialize(Agent agent)
        {
            _showLockOnUIEventChannel = AddressableManager.Load<GameEventChannelSO>("UIEventChannelSO");
            _player = agent as Player;
            _detectColliders = new Collider[_maxDetectCount];
        }

        public void AfterInitialize()
        {
            _player.GetCompo<PlayerAttack>().OnAttack += HandleAttack;
            _player.GetCompo<PlayerAnimationTrigger>().OnComboPossible += HandleComboPossible;
            _player.OnHitTarget += HandleHitTarget;
            _player.PlayerInput.ChangeLockOnTargetEvent += HandleChangeLockOnTarget;
            DetectNearTarget().Forget();
        }

        private void OnDestroy()
        {

            _player.OnHitTarget -= HandleHitTarget;
            _player.GetCompo<PlayerAttack>().OnAttack -= HandleAttack;
            _player.GetCompo<PlayerAnimationTrigger>().OnComboPossible -= HandleComboPossible;
            _player.PlayerInput.ChangeLockOnTargetEvent -= HandleChangeLockOnTarget;
        }

        private void HandleHitTarget(HitInfo hitInfo)
        {
            OnChangedHitTargetEnemy?.Invoke(hitInfo.hitTarget as Agent);
        }

        private void HandleChangeLockOnTarget()
        {
            if (!_target) return;

            var nearEnemy = GetNearestEnemy(exclude: _target.gameObject);
            if (!nearEnemy || nearEnemy.HealthCompo.IsDead || nearEnemy == _target)
                return;

            ApplyNewTarget(nearEnemy);
        }

        private async UniTaskVoid DetectNearTarget()
        {
            _cancellationToken = new CancellationTokenSource();
            _cancellationToken.RegisterRaiseCancelOnDestroy(gameObject);
            try
            {
                while (!_cancellationToken.IsCancellationRequested)
                {
                    await UniTask.WaitUntil(() => gameObject.activeSelf, cancellationToken: _cancellationToken.Token);
                    await UniTask.Delay(TimeSpan.FromSeconds(_detectInterval),
                        cancellationToken: _cancellationToken.Token);

                    int count = Physics.OverlapSphereNonAlloc(transform.position, _detectionRadius, _detectColliders,
                        Define.MLayerMask.WhatIsEnemy);
                    if (count == 0)
                    {
                        if (_target)
                            ClearTarget();
                        continue;
                    }

                    if (_target && (_target.HealthCompo.IsDead || !(_target as IEnemy).IsLockOnTargetable))
                    {
                        ClearTarget();
                        continue;
                    }

                    if (!_target)
                    {
                        var nearEnemy = GetNearestEnemy(detectColliders: _detectColliders, count: count);
                        if (nearEnemy && !nearEnemy.HealthCompo.IsDead)
                            ApplyNewTarget(nearEnemy);
                    }
                }
            }
            catch (Exception e)
            {
            }
        }

        private Agent GetNearestEnemy(GameObject exclude = null, Predicate<Collider> filter = null,
            Collider[] detectColliders = null, int count = 0)
        {
            if (detectColliders == null)
            {
                count = Physics.OverlapSphereNonAlloc(transform.position, _detectionRadius, _detectColliders,
                    Define.MLayerMask.WhatIsEnemy);
            }

            if (count == 0) return null;

            return _detectColliders
                .Take(count)
                .Where(c => c && c.CompareTag("Enemy") && c.gameObject != exclude && (filter?.Invoke(c) ?? true))
                .OrderBy(c => Vector3.Distance(transform.position, c.transform.position))
                .Select(c => c.GetComponent<Agent>()).Where(a => (a as IEnemy).IsLockOnTargetable)
                .FirstOrDefault();
        }

        private void ApplyNewTarget(Agent newTarget)
        {
            Agent prevTarget = _target;

            if (_player.IsLockOn && prevTarget is IEnemy prevEnemy)
                prevEnemy.SetAsTargeted(false);

            _target = newTarget;

            if (_player.IsLockOn && _target is IEnemy newEnemy)
                newEnemy.SetAsTargeted(true);

            RaiseLockOnEvent(_target);
            OnChangedTargetEnemy?.Invoke(prevTarget, _target);
        }

        private void ClearTarget()
        {
            Agent prevTarget = _target;

            if (_target is IEnemy enemy)
                enemy.SetAsTargeted(false);

            _target = null;
            RaiseLockOnEvent(_target);
            OnChangedTargetEnemy?.Invoke(prevTarget, _target);
        }

        private void RaiseLockOnEvent(Agent target)
        {
            var evt = UIEvents.ShowLockOnUI;
            evt.isShowUI = target;
            evt.lockOnTarget = !target ? null : target.GetComponent<ILockOnAble>();
            _showLockOnUIEventChannel.RaiseEvent(evt);
        }

        private void HandleComboPossible() => enabled = true;
        private void HandleAttack() => enabled = false;

        public bool TryGetTargetEnemy<T>(out T target) where T : Agent
        {
            target = _target as T;
            return target;
        }

        public bool TryGetTargetEnemy(out Agent target)
        {
            target = GetTargetEnemy();
            return target;
        }

        public bool TryGetTargetEnemyNoInput(out Agent target)
        {
            target = _target;
            return target;
        }

        public Agent GetTargetEnemyNoInput() => _target;

        public void SetForceTargetEnemy(Agent target)
        {
            if (!target) return;
            ApplyNewTarget(target);
        }

        public Agent GetTargetEnemy()
        {
            Vector3 input = _player.PlayerInput.Input;
            if (input.sqrMagnitude <= 0)
                return _target;

            Vector3 inputDir = GetWorldInputDirection(input);

            var target = GetNearestEnemy(
                exclude: null,
                filter: col =>
                {
                    Vector3 dirToEnemy = (col.transform.position - _player.transform.position).normalized;
                    return Vector3.Angle(inputDir, dirToEnemy) < 125;
                });

            if (target)
            {
                ApplyNewTarget(target);
            }

            return _target;
        }

        private Vector3 GetWorldInputDirection(Vector3 input)
        {
            var cam = Camera.main;
            var forward = cam.transform.forward;
            var right = cam.transform.right;

            forward.y = 0f;
            right.y = 0f;

            return (forward.normalized * input.z + right.normalized * input.x).normalized;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _detectionRadius);
            Gizmos.color = Color.white;
        }
#endif
    }
}