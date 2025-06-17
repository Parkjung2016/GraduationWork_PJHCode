using System;
using Main.Core;
using Main.Runtime.Agents;
using Main.Runtime.Core;
using Main.Runtime.Core.Events;
using PJH.Runtime.Players.FinisherSequence;
using Sirenix.OdinInspector;
using UnityEngine;

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


        public void Initialize(Agent agent)
        {
            _gameEventChannel = AddressableManager.Load<GameEventChannelSO>("GameEventChannel");
            _player = agent as Player;
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
            PlayerFinisherTargetDetection finisherTargetDetectionCompo =
                _player.GetCompo<PlayerFinisherTargetDetection>();
            PlayerMovement movementCompo = _player.GetCompo<PlayerMovement>();
            if (_player.IsStunned || _player.IsHitting || !finisherTargetDetectionCompo.GetFinisherTarget(
                    out AgentFinisherable target) ||
                movementCompo.IsEvading) return;

            target.SetToFinisherTarget();

            FinisherSequenceDataSO finisherSequenceData = GetFinisherSequenceData(_finisherSequence);
            OnAdjustTimelineModelPosition?.Invoke(target.transform, finisherSequenceData.distanceFromEnemy);
            IsFinishering = true;
            EnemyFinisherSequence evt = GameEvents.EnemyFinisherSequence;
            evt.sequenceAsset = finisherSequenceData.sequenceAsset;
            evt.enemyAnimator = target.Agent.GetCompo<AgentAnimator>(true).Animator;
            _gameEventChannel.RaiseEvent(evt);
            OnFinisherTimeline?.Invoke(IsFinishering);
        }

        private FinisherSequenceDataSO GetFinisherSequenceData(FinisherSequenceSO finisherSequence)
        {
            return finisherSequence.sequenceDatas.GetRandom();
        }
    }
}