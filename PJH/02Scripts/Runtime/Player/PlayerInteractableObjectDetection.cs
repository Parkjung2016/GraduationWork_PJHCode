using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Main.Core;
using Main.Runtime.Agents;
using Main.Runtime.Core;
using Main.Runtime.Core.Events;
using Main.Shared;
using UnityEngine;
using ZLinq;
using Debug = Main.Core.Debug;

namespace PJH.Runtime.Players
{
    public class PlayerInteractableObjectDetection : MonoBehaviour, IAgentComponent, IAfterInitable
    {
        private GameEventChannelSO _showInteractUIEventChannel;
        [SerializeField] private float _detectInterval = 0.05f;
        [SerializeField] private LayerMask _whatIsInteractable;
        [SerializeField] private float _detectionRadius = 5f;
        [SerializeField] private int _maxDetectCount = 5;

        private IInteractable _interactableTarget;
        private Collider[] _detectColliders;
        private Player _player;
        private CancellationTokenSource _cancellationToken;

        public void Initialize(Agent agent)
        {
            _showInteractUIEventChannel = AddressableManager.Load<GameEventChannelSO>("UIEventChannelSO");
            _player = agent as Player;
            _detectColliders = new Collider[_maxDetectCount];
        }

        public void AfterInitialize()
        {
            _player.GetCompo<PlayerEnemyFinisher>().OnFinisher += HandleFinisher;
            _player.GetCompo<PlayerEnemyFinisher>().OnFinisherEnd += HandleFinisherEnd;
            DetectNearInteractTarget();
        }

        private void OnDestroy()
        {
            if (_cancellationToken is { IsCancellationRequested: false })
            {
                _cancellationToken.Cancel();
                _cancellationToken.Dispose();
            }

            _player.GetCompo<PlayerEnemyFinisher>().OnFinisher -= HandleFinisher;
            _player.GetCompo<PlayerEnemyFinisher>().OnFinisherEnd -= HandleFinisherEnd;
        }

        private void HandleFinisherEnd()
        {
            enabled = true;
        }

        private void HandleFinisher()
        {
            enabled = false;
        }

        private async void DetectNearInteractTarget()
        {
            try
            {
                _cancellationToken = new CancellationTokenSource();
                while (!_cancellationToken.IsCancellationRequested)
                {
                    await UniTask.WaitUntil(() => gameObject.activeSelf, cancellationToken: _cancellationToken.Token);
                    await UniTask.WaitForSeconds(_detectInterval,
                        cancellationToken: _cancellationToken.Token);
                    int cnt = Physics.OverlapSphereNonAlloc(transform.position, _detectionRadius, _detectColliders,
                        _whatIsInteractable);
                    if (cnt > 0)
                    {
                        var copyArray = _detectColliders.ToArray();
                        Array.Resize(ref copyArray, cnt);

                        Collider nearCollider =
                            copyArray.OrderBy(c => Vector3.Distance(_player.transform.position, c.transform.position))
                                .FirstOrDefault();
                        if (nearCollider)
                        {
                            _interactableTarget = nearCollider.GetComponent<IInteractable>();
                        }
                        else
                            _interactableTarget = null;
                    }
                    else
                    {
                        _interactableTarget = null;
                    }

                    var evt = UIEvents.ShowInteractUIEventChannel;
                    bool isShowUI = _interactableTarget != null;
                    if (evt.isShowUI != isShowUI)
                    {
                        evt.isShowUI = _interactableTarget != null;
                        evt.interactableTarget = _interactableTarget;
                        if (_interactableTarget != null)
                        {
                            evt.interactDescription = _interactableTarget.Description;
                        }

                        _showInteractUIEventChannel.RaiseEvent(evt);
                    }
                }
            }
            catch (Exception e)
            {
            }
        }

        public bool GetNearTarget<T>(out T target) where T : class, IInteractable
        {
            if (_interactableTarget != null)
            {
                target = _interactableTarget as T;
                return true;
            }

            target = null;
            return false;
        }

        public bool GetNearTarget(out IInteractable target)
        {
            target = _interactableTarget;
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