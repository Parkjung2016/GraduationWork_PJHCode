using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Main.Runtime.Agents;
using Main.Runtime.Core;
using Main.Runtime.Core.Events;
using PJH.Utility.Managers;
using UnityEngine;

namespace PJH.Runtime.Players
{
    public class PlayerFinisherTargetDetection : MonoBehaviour, IAgentComponent, IAfterInitable
    {
        private GameEventChannelSO _showFinisherTargetUIEventChannel;
        [SerializeField] private float _detectInterval = 0.05f;
        private Player _player;

        private PlayerEnemyFinisher _enemyFinisherCompo;
        private PlayerFullMount _fullMountCompo;
        private Agent _finisherTarget;
        private Agent _checkTarget;
        private CancellationTokenSource _cancellationToken;

        public void Initialize(Agent agent)
        {
            _player = agent as Player;
            _enemyFinisherCompo = _player.GetCompo<PlayerEnemyFinisher>();
            _fullMountCompo = _player.GetCompo<PlayerFullMount>();
            _showFinisherTargetUIEventChannel = AddressableManager.Load<GameEventChannelSO>("UIEventChannelSO");
        }


        public void AfterInitialize()
        {
            _player.GetCompo<PlayerEnemyDetection>().OnChangedHitTargetEnemy += HandleChangedTargetEnemy;
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

            _player.GetCompo<PlayerEnemyDetection>().OnChangedHitTargetEnemy -= HandleChangedTargetEnemy;
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
                        if (_checkTarget.HealthCompo.IsDead)
                        {
                            _checkTarget = null;
                            continue;
                        }

                        AgentFinisherable finisherable = _checkTarget.GetCompo<AgentFinisherable>();
                        if (!finisherable) continue;
                        float dis = Vector3.Distance(finisherable.Agent.transform.position,
                            _player.transform.position);
                        bool canFinisher = !_fullMountCompo.IsFullMounting && !_enemyFinisherCompo.IsFinishering &&
                                           finisherable.CanFinisher() &&
                                           dis <= 1.5f;
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

        private void HandleChangedTargetEnemy(Agent target)
        {
            _checkTarget = target;
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