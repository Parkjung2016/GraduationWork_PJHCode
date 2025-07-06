using System;
using Kinemation.MotionWarping.Runtime.Examples;
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
        // public event Action<bool> OnFinisherTimeline;

        public event Action OnFinisher;
        public event Action OnFinisherEnd;

        // public event Action<Transform, float> OnAdjustTimelineModelPosition;
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
            _gameEventChannel.AddListener<FinishEnemyFinisher>(HandleFinishEnemyFinisher);
        }


        private void OnDestroy()
        {
            _player.PlayerInput.FinisherEvent -= HandleFinisher;
            _gameEventChannel.RemoveListener<FinishEnemyFinisher>(HandleFinishEnemyFinisher);
        }

        private void HandleFinishEnemyFinisher(FinishEnemyFinisher evt)
        {
            IsFinishering = false;
            OnFinisherEnd?.Invoke();
        }

        private void HandleFinisher()
        {
            PlayerFinisherTargetDetection finisherTargetDetectionCompo =
                _player.GetCompo<PlayerFinisherTargetDetection>();
            PlayerMovement movementCompo = _player.GetCompo<PlayerMovement>();
            if (_player.IsStunned || _player.IsHitting || !finisherTargetDetectionCompo.GetFinisherTarget(
                    out AgentFinisherable target) ||
                movementCompo.IsEvading) return;
            FinisherDataSO finisherData = GetFinisherSequenceData(_finisherSequence);
            AlignComponent alignComponent = target.Agent.GetComponent<AlignComponent>();
            alignComponent.targetAnim = finisherData.executedClip;
            alignComponent.motionWarpingAsset = finisherData.executionAsset;

            bool result = _player.WarpingComponent.Interact(alignComponent);
            if (!result) return;

            target.SetToFinisherTarget();


            IsFinishering = true;
            EnemyFinisherSequence evt = GameEvents.EnemyFinisherSequence;
            evt.sequenceAsset = finisherData;
            evt.playerAnimator = _player.GetCompo<PlayerAnimator>().Animator;

            _gameEventChannel.RaiseEvent(evt);
            OnFinisher?.Invoke();
        }

        private FinisherDataSO GetFinisherSequenceData(FinisherSequenceSO finisherSequence)
        {
            return finisherSequence.sequenceDatas.GetRandom();
        }
    }
}