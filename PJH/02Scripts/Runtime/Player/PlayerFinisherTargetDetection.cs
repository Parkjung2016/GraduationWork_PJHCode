using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Main.Core;
using Main.Runtime.Agents;
using Main.Runtime.Core;
using Main.Runtime.Core.Events;
using UnityEngine;
using Debug = Main.Core.Debug;

namespace PJH.Runtime.Players
{
    public class PlayerFinisherTargetDetection : MonoBehaviour, IAgentComponent, IAfterInitable
    {
        private GameEventChannelSO _showFinisherTargetUIEventChannel;
        [SerializeField] private float _detectInterval = 0.05f;
        private Player _player;

        private Agent _finisherTarget;
        private Agent _checkTarget;
        private CancellationTokenSource _cancellationToken;

        public void Initialize(Agent agent)
        {
            _showFinisherTargetUIEventChannel = AddressableManager.Load<GameEventChannelSO>("UIEventChannelSO");
            _player = agent as Player;
        }


        public void AfterInitialize()
        {
            _player.GetCompo<PlayerEnemyDetection>().OnChangedTargetEnemy += HandleChangedTargetEnemy;
            DetectTarget().Forget();
        }

        private void OnDestroy()
        {
            if (_cancellationToken != null && !_cancellationToken.IsCancellationRequested)
            {
                _cancellationToken.Cancel();
                _cancellationToken.Dispose();
            }

            _player.GetCompo<PlayerEnemyDetection>().OnChangedTargetEnemy -= HandleChangedTargetEnemy;
        }

        private async UniTaskVoid DetectTarget()
        {
            _cancellationToken = new CancellationTokenSource();
            try
            {
                while (!_cancellationToken.IsCancellationRequested)
                {
                    await UniTask.WaitUntil(() => gameObject.activeSelf, cancellationToken: _cancellationToken.Token);
                    await UniTask.WaitForSeconds(_detectInterval,
                        cancellationToken: _cancellationToken.Token);
                    var evt = UIEvents.ShowFinisherTargetUI;
                    if (_checkTarget)
                    {
                        AgentFinisherable finisherable = _checkTarget.GetCompo<AgentFinisherable>();
                        if (!finisherable) continue;

                        bool canFinisher = finisherable.CanFinisher();
                        evt.isShowUI = canFinisher;
                        if (canFinisher)
                        {
                            if (evt.finisherTargetTrm == _checkTarget.transform) continue;
                            evt.finisherTargetTrm = _checkTarget.transform;
                            _finisherTarget = _checkTarget;
                        }
                        else
                        {
                            _finisherTarget = null;
                            evt.finisherTargetTrm = null;
                        }

                        _showFinisherTargetUIEventChannel.RaiseEvent(evt);
                    }
                    else
                    {
                        if (!evt.isShowUI) continue;
                        _finisherTarget = null;
                        evt.isShowUI = false;
                        _showFinisherTargetUIEventChannel.RaiseEvent(evt);
                    }
                }
            }
            catch (Exception e)
            {
            }
        }

        private void HandleChangedTargetEnemy(Agent prevTarget, Agent currentTarget)
        {
            _checkTarget = currentTarget;
        }


        public bool GetFinisherTarget(out AgentFinisherable target)
        {
            target = null;
            if (!_finisherTarget)
                return false;
            if (!_finisherTarget.TryGetCompo(out AgentFinisherable finiserable, true)) return false;
            if (!finiserable.CanFinisher()) return false;
            target = finiserable;
            return true;
        }
    }
}