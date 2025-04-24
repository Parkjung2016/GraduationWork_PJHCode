using System;
using Main.Core;
using Main.Runtime.Agents;
using Main.Runtime.Core;
using Main.Runtime.Core.Events;
using PJH.Runtime.Players.FinisherSequence;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PJH.Runtime.Players
{
    public class PlayerEnemyFinisher : MonoBehaviour, IAgentComponent, IAfterInitable
    {
        public bool IsFinishering { get; private set; }
        public event Action<bool> OnFinisherTimeline;
        public event Action OnFinisher;
        public event Action<Transform, float> OnAdjustTimelineModelPosition;
        [SerializeField, InlineEditor] private FinisherSequenceSO _finisherSequence;
        private GameEventChannelSO _gameEventChannel;

        private Player _player;

        private PlayerFinisherTargetDetection _finisherTargetDetectionCompo;
        private PlayerMovement _movementCompo;

        public void Initialize(Agent agent)
        {
            _gameEventChannel = AddressableManager.Load<GameEventChannelSO>("GameEventChannel");
            _player = agent as Player;
            _finisherTargetDetectionCompo = _player.GetCompo<PlayerFinisherTargetDetection>();
            _movementCompo = _player.GetCompo<PlayerMovement>();
        }

        public void AfterInitialize()
        {
            _player.PlayerInput.FinisherEvent += HandleFinisher;
            _gameEventChannel.AddListener<FinishTimeline>(HandleFinishTimeline);
        }


        private void OnDestroy()
        {
            _player.PlayerInput.FinisherEvent -= HandleFinisher;
            _gameEventChannel.RemoveListener<FinishTimeline>(HandleFinishTimeline);
        }

        private void HandleFinishTimeline(FinishTimeline evt)
        {
            IsFinishering = false;
            OnFinisherTimeline?.Invoke(IsFinishering);
        }

        private void HandleFinisher()
        {
            if (_player.IsStunned || _player.IsHitting || !_finisherTargetDetectionCompo.GetFinisherTarget(
                    out AgentFinisherable target) ||
                _movementCompo.IsEvading) return;

            target.SetToFinisherTarget();
            
            FinisherSequenceDataSO finisherSequenceData = GetFinisherSequenceData(_finisherSequence);
            OnAdjustTimelineModelPosition?.Invoke(target.transform, finisherSequenceData.distanceFromEnemy);
            IsFinishering = true;
            EnemyFinisherSequence evt = GameEvents.EnemyFinisherSequence;
            evt.sequenceAsset = finisherSequenceData.sequenceAsset;
            evt.enemyAnimator = target.Agent.GetCompo<AgentAnimator>(true).Animancer.Animator;
            _gameEventChannel.RaiseEvent(evt);
            OnFinisherTimeline?.Invoke(IsFinishering);
        }

        private FinisherSequenceDataSO GetFinisherSequenceData(FinisherSequenceSO finisherSequence)
        {
            int sequenceDataIdx = Random.Range(0, finisherSequence.sequenceDatas.Count);
            return finisherSequence.sequenceDatas[sequenceDataIdx];
        }
    }
}