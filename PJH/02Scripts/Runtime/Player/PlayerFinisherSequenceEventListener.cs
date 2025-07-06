using Main.Core;
using Main.Runtime.Agents;
using Main.Runtime.Core;
using Main.Runtime.Core.Events;
using Unity.Cinemachine;
using UnityEngine;

namespace PJH.Runtime.Players
{
    public class PlayerFinisherSequenceEventListener : MonoBehaviour, IAgentComponent, IAfterInitable
    {
        private Player _player;

        private CinemachineImpulseSource _impulseSource;

        private GameEventChannelSO _gameEventChannel;

        public void Initialize(Agent agent)
        {
            _player = agent as Player;
            _gameEventChannel = AddressableManager.Load<GameEventChannelSO>("GameEventChannel");
            _impulseSource = transform.Find("ImpulseSource").GetComponent<CinemachineImpulseSource>();
        }

        public void AfterInitialize()
        {
            PlayerAnimationTrigger animationTrigger = _player.GetCompo<PlayerAnimationTrigger>();
            animationTrigger.OnFinisherSequenceShake += HandleFinisherSequenceShake;
            animationTrigger.OnFinisherSequenceTargetDeath += HandleFinisherSequenceTargetDeath;
            animationTrigger.OnFinisherSequenceFinish += HandleFinisherSequenceFinish;
        }

        private void OnDestroy()
        {
            PlayerAnimationTrigger animationTrigger = _player.GetCompo<PlayerAnimationTrigger>();
            animationTrigger.OnFinisherSequenceShake -= HandleFinisherSequenceShake;
            animationTrigger.OnFinisherSequenceTargetDeath -= HandleFinisherSequenceTargetDeath;
            animationTrigger.OnFinisherSequenceFinish -= HandleFinisherSequenceFinish;
        }

        private void HandleFinisherSequenceFinish()
        {
            var evt = GameEvents.FinishEnemyFinisher;
            _gameEventChannel.RaiseEvent(evt);
        }

        private void HandleFinisherSequenceTargetDeath()
        {
            var evt = GameEvents.DeadFinisherTarget;
            _gameEventChannel.RaiseEvent(evt);
        }

        private void HandleFinisherSequenceShake()
        {
            _impulseSource.GenerateImpulse(1);
        }
    }
}