using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Main.Core;
using Main.Runtime.Agents;
using Main.Runtime.Core;
using Main.Runtime.Core.Events;
using Main.Shared;
using Sirenix.OdinInspector;
using UnityEngine;
using ZLinq;


namespace PJH.Runtime.Players
{
    public class PlayerEnemyDetection : MonoBehaviour, IAgentComponent, IAfterInitable
    {
        public delegate void ChangedTargetEnemyEvent(Agent prevTarget, Agent currentTarget);

        public event ChangedTargetEnemyEvent OnChangedTargetEnemy;
        [SerializeField] private float _detectionRadius = 5f;
        [SerializeField] private float _detectInterval = 0.05f;
        [SerializeField] private int _maxDetectCount = 5;


        [SerializeField, ReadOnly] private Agent _target;
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
            _player.GetCompo<PlayerEnemyFinisher>().OnFinisherTimeline += HandleFinisherTimeline;
            PlayerAttack attackCompo = _player.GetCompo<PlayerAttack>();
            attackCompo.OnAttack += HandleAttack;
            _player.GetCompo<PlayerAnimationTrigger>().OnComboPossible += HandleComboPossible;
            _player.PlayerInput.ChangeLockOnTargetEvent += HandleChangeLockOnTarget;
            DetectNearTarget().Forget();
        }

        private void OnDestroy()
        {
            if (_cancellationToken != null && !_cancellationToken.IsCancellationRequested)
            {
                _cancellationToken.Cancel();
                _cancellationToken.Dispose();
            }

            _player.GetCompo<PlayerEnemyFinisher>().OnFinisherTimeline -= HandleFinisherTimeline;

            PlayerAttack attackCompo = _player.GetCompo<PlayerAttack>();
            attackCompo.OnAttack -= HandleAttack;
            _player.GetCompo<PlayerAnimationTrigger>().OnComboPossible -= HandleComboPossible;

            _player.PlayerInput.ChangeLockOnTargetEvent -= HandleChangeLockOnTarget;
        }

        private void HandleChangeLockOnTarget()
        {
            if (!_target) return;
            int cnt = Physics.OverlapSphereNonAlloc(transform.position, _detectionRadius, _detectColliders,
                Define.MLayerMask.WhatIsEnemy);
            Agent prevTarget = null;
            var evt = UIEvents.ShowLockOnUI;

            if (cnt > 0)
            {
                var copyArray = _detectColliders.ToArray();
                Array.Resize(ref copyArray, cnt);
                Collider nearCollider;
                Agent nearTarget;

                nearCollider =
                    copyArray.Where(collider => collider.gameObject != _target.gameObject)
                        .OrderBy(c => Vector3.Distance(transform.position, c.transform.position))
                        .FirstOrDefault();

                if (!nearCollider)
                    return;

                nearTarget = nearCollider.GetComponent<Agent>();
                if (nearTarget.HealthCompo.IsDead)
                    return;
                if (nearTarget == _target) return;
                prevTarget = _target;
                _target = nearTarget;
                evt.isShowUI = true;
                evt.lockOnTarget = _target.GetComponent<ILockOnAble>();
                _showLockOnUIEventChannel.RaiseEvent(evt);
                OnChangedTargetEnemy?.Invoke(prevTarget, _target);
            }
        }

        private async UniTaskVoid DetectNearTarget()
        {
            _cancellationToken = new CancellationTokenSource();
            try
            {
                while (!_cancellationToken.IsCancellationRequested)
                {
                    await UniTask.WaitUntil(() => gameObject.activeSelf, cancellationToken: _cancellationToken.Token);
                    await UniTask.WaitForSeconds(_detectInterval,
                        cancellationToken: _cancellationToken.Token);
                    int cnt = Physics.OverlapSphereNonAlloc(transform.position, _detectionRadius, _detectColliders,
                        Define.MLayerMask.WhatIsEnemy);
                    Agent prevTarget = null;
                    var evt = UIEvents.ShowLockOnUI;

                    if (cnt > 0)
                    {
                        var copyArray = _detectColliders.ToArray();
                        Array.Resize(ref copyArray, cnt);
                        Collider nearCollider;
                        Agent nearTarget;

                        if (_target)
                        {
                            if (_target.HealthCompo.IsDead)
                            {
                                prevTarget = _target;
                                _target = null;
                                OnChangedTargetEnemy?.Invoke(prevTarget, _target);
                                evt.isShowUI = false;
                                evt.lockOnTarget = null;
                                _showLockOnUIEventChannel.RaiseEvent(evt);
                            }

                            continue;
                        }

                        nearCollider =
                            copyArray.OrderBy(c => Vector3.Distance(transform.position, c.transform.position))
                                .FirstOrDefault();

                        if (!nearCollider)
                            continue;

                        nearTarget = nearCollider.GetComponent<Agent>();
                        if (nearTarget.HealthCompo.IsDead)
                            continue;
                        if (nearTarget == _target) continue;
                        prevTarget = _target;
                        _target = nearTarget;
                        evt.isShowUI = true;
                        evt.lockOnTarget = _target.GetComponent<ILockOnAble>();
                    }
                    else
                    {
                        if (!_target) continue;
                        prevTarget = _target;
                        _target = null;
                        evt.isShowUI = false;
                        evt.lockOnTarget = null;
                    }

                    _showLockOnUIEventChannel.RaiseEvent(evt);
                    OnChangedTargetEnemy?.Invoke(prevTarget, _target);
                }
            }
            catch (Exception e)
            {
            }
        }

        private void HandleComboPossible()
        {
            enabled = true;
        }

        private void HandleAttack()
        {
            enabled = false;
        }

        private void HandleFinisherTimeline(bool isPlayingTimeline)
        {
            enabled = !isPlayingTimeline;
        }

        public bool TryGetTargetEnemy<T>(out T target) where T : Agent
        {
            Agent targetEnemy = GetTargetEnemy();
            if (targetEnemy != null)
            {
                target = targetEnemy as T;
                return target != null;
            }

            target = null;
            return false;
        }

        public Agent GetTargetEnemyNoInput()
        {
            return _target;
        }

        public void SetForceTargetEnemy(Agent target)
        {
            Agent prevTarget = _target;
            _target = target;
            var evt = UIEvents.ShowLockOnUI;
            evt.isShowUI = true;
            evt.lockOnTarget = _target.GetComponent<ILockOnAble>();

            OnChangedTargetEnemy?.Invoke(prevTarget, _target);
            _showLockOnUIEventChannel.RaiseEvent(evt);
        }

        public Agent GetTargetEnemy()
        {
            Vector3 input = _player.PlayerInput.Input;
            if (input.sqrMagnitude > 0)
            {
                var camera = Camera.main;
                var forward = camera.transform.forward;
                var right = camera.transform.right;

                forward.y = 0f;
                right.y = 0f;

                forward.Normalize();
                right.Normalize();

                Vector3 inputDirection = forward * input.z + right * input.x;
                inputDirection = inputDirection.normalized;
                int cnt = Physics.OverlapSphereNonAlloc(transform.position, _detectionRadius, _detectColliders,
                    Define.MLayerMask.WhatIsEnemy);
                if (cnt > 0)
                {
                    var copyArray = _detectColliders.ToArray();
                    Array.Resize(ref copyArray, cnt);
                    Collider nearCollider;
                    Agent nearTarget;
                    nearCollider =
                        copyArray.Where(col =>
                            {
                                Vector3 dirToEnemy = (col.transform.position - _player.transform.position)
                                    .normalized;
                                return Vector3.Angle(inputDirection, dirToEnemy) < 125;
                            })
                            .OrderBy(c => Vector3.Distance(transform.position, c.transform.position))
                            .FirstOrDefault();
                    if (!nearCollider)
                        return _target;
                    nearTarget = nearCollider.GetComponent<Agent>();
                    Agent prevTarget = _target;
                    _target = nearTarget;
                    var evt = UIEvents.ShowLockOnUI;
                    evt.isShowUI = true;
                    evt.lockOnTarget = _target.GetComponent<ILockOnAble>();

                    OnChangedTargetEnemy?.Invoke(prevTarget, _target);
                    _showLockOnUIEventChannel.RaiseEvent(evt);

                    return _target;
                }
            }

            return _target;
        }

        public bool TryGetTargetEnemy(out Agent target)
        {
            target = GetTargetEnemy();
            return target != null;
        }

        public bool TryGetTargetEnemyNoInput(out Agent target)
        {
            target = _target;
            return target != null;
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