using System;
using DG.Tweening;
using Kinemation.MotionWarping.Runtime.Examples;
using Main.Runtime.Agents;
using Main.Runtime.Core;
using Main.Runtime.Core.Events;
using PJH.Runtime.Players.FinisherSequence;
using PJH.Utility.Extensions;
using PJH.Utility.Managers;
using Sirenix.OdinInspector;
using UnityEngine;
using ZLinq;

namespace PJH.Runtime.Players
{
    public class PlayerEnemyFinisher : MonoBehaviour, IAgentComponent, IAfterInitable
    {
        public bool IsFinishering { get; private set; }

        public event Action OnFinisher;
        public event Action OnFinisherEnd;

        [SerializeField, InlineEditor] private FinisherSequenceSO _finisherSequence;
        [SerializeField] private LayerMask _whatIsObstacle;
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
        }

        private void OnDestroy()
        {
            _player.PlayerInput.FinisherEvent -= HandleFinisher;
        }


        private void HandleFinisher()
        {
            PlayerFinisherTargetDetection finisherTargetDetectionCompo =
                _player.GetCompo<PlayerFinisherTargetDetection>();
            PlayerMovement movementCompo = _player.GetCompo<PlayerMovement>();
            if (_player.IsStunned || _player.IsHitting || !finisherTargetDetectionCompo.GetFinisherTarget(
                    out AgentFinisherable target) ||
                movementCompo.IsEvading) return;
            target.Agent.HealthCompo.IsInvincibility = true;
            FinisherDataSO finisherData = GetFinisherSequenceData(_finisherSequence);
            AlignComponent alignComponent = target.Agent.GetComponent<AlignComponent>();
            alignComponent.targetAnim = finisherData.executedClip;
            alignComponent.motionWarpingAsset = finisherData.executionAsset;
            _player.GetCompo<PlayerAnimationTrigger>().OnDisableDamageCollider?.Invoke();
            target.SetToFinisherTarget();
            target.Agent.transform.DOMoveY(_player.transform.position.y, .1f);
            target.Agent.transform.DOLookAt(_player.transform.position, .1f);
            _player.ModelTrm.DOLookAt(target.Agent.transform.position, .2f, AxisConstraint.Y).OnComplete(() =>
            {
                _player.WarpingComponent.Interact(alignComponent);
                EnemyFinisherSequence evt = GameEvents.EnemyFinisherSequence;
                evt.sequenceAsset = finisherData;

                AgentAnimator animatorComp = target.Agent.GetCompo<AgentAnimator>(true);
                animatorComp.lockedTransitionAnimation = true;
                evt.targetAnimator = animatorComp.Animator;

                animatorComp = _player.GetCompo<AgentAnimator>(true);
                animatorComp.lockedTransitionAnimation = true;
                evt.playerAnimator = animatorComp.Animator;

                IsFinishering = true;
                _gameEventChannel.RaiseEvent(evt);
                OnFinisher?.Invoke();
                _player.WarpingComponent.OnAnimationFinished += HandleAnimationFinished;
            });
        }

        private void HandleAnimationFinished()
        {
            if (IsFinishering)
            {
                _player.WarpingComponent.OnAnimationFinished -= HandleAnimationFinished;
                _player.GetCompo<AgentAnimator>(true).lockedTransitionAnimation = false;
                IsFinishering = false;
                OnFinisherEnd?.Invoke();
            }
        }

        private FinisherDataSO GetFinisherSequenceData(FinisherSequenceSO finisherSequence)
        {
            Vector3 playerPosition = _player.transform.position;
            var filteredList = finisherSequence.sequenceDatas
                .GroupBy(data => data.spaceToExecute)
                .Where(data =>
                {
                    if (data.Key == 0) return true;
                    bool result = Physics.CheckSphere(playerPosition, data.Key, _whatIsObstacle);
                    return !result;
                }).OrderByDescending(data => data.Key).FirstOrDefault().ToList();
            return filteredList.Random();
        }
    }
}