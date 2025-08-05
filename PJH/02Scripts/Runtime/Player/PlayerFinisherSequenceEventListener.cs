using FMOD.Studio;
using FMODUnity;
using Main.Core;
using Main.Runtime.Agents;
using Main.Runtime.Core;
using Main.Runtime.Core.Events;
using Main.Runtime.Equipments.Datas;
using Unity.Cinemachine;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace PJH.Runtime.Players
{
    public class PlayerFinisherSequenceEventListener : MonoBehaviour, IAgentComponent, IAfterInitable
    {
        [SerializeField] private EventReference _playCharacterFallingOnGroundSound;
        [SerializeField] private EventReference _playNeckGrabbingSound, _playNeckGrabSound;
        [SerializeField] private EventReference _playArmGrabSound, _playArmBreakSound;

        private EventInstance _neckGrabbingSoundInstance;
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
            PlayerAnimationTrigger animationTriggerCompo = _player.GetCompo<PlayerAnimationTrigger>();
            animationTriggerCompo.OnFinisherSequenceShake += HandleFinisherSequenceShake;
            animationTriggerCompo.OnFinisherSequenceTargetDeath += HandleFinisherSequenceTargetDeath;
            animationTriggerCompo.OnFinisherSequenceFinish += HandleFinisherSequenceFinish;
            animationTriggerCompo.OnPlayPunchImpactSound += HandlePlayPunchImpactSound;
            animationTriggerCompo.OnPlayNeckGrabSound += HandlePlayNeckGrabSound;
            animationTriggerCompo.OnPlayNeckGrabbingSound += HandlePlayNeckGrabbingSound;
            animationTriggerCompo.OnStopNeckGrabbingSound += HandleStopNeckGrabbingSound;
            animationTriggerCompo.OnPlayCharacterFallingOnGroundSound += HandlePlayCharacterFallingOnGroundSound;
            animationTriggerCompo.OnPlayArmGrabSound += HandlePlayArmGrabSound;
            animationTriggerCompo.OnPlayArmBreakSound += HandlePlayArmBreakSound;
        }

        private void OnDestroy()
        {
            PlayerAnimationTrigger animationTriggerCompo = _player.GetCompo<PlayerAnimationTrigger>();
            animationTriggerCompo.OnFinisherSequenceShake -= HandleFinisherSequenceShake;
            animationTriggerCompo.OnFinisherSequenceTargetDeath -= HandleFinisherSequenceTargetDeath;
            animationTriggerCompo.OnFinisherSequenceFinish -= HandleFinisherSequenceFinish;
            animationTriggerCompo.OnPlayPunchImpactSound -= HandlePlayPunchImpactSound;
            animationTriggerCompo.OnPlayNeckGrabSound -= HandlePlayNeckGrabSound;
            animationTriggerCompo.OnPlayNeckGrabbingSound -= HandlePlayNeckGrabbingSound;
            animationTriggerCompo.OnStopNeckGrabbingSound -= HandleStopNeckGrabbingSound;
            animationTriggerCompo.OnPlayCharacterFallingOnGroundSound -= HandlePlayCharacterFallingOnGroundSound;
            animationTriggerCompo.OnPlayArmGrabSound -= HandlePlayArmGrabSound;
            animationTriggerCompo.OnPlayArmBreakSound -= HandlePlayArmBreakSound;
        }

        private void HandlePlayArmBreakSound()
        {
            RuntimeManager.PlayOneShot(_playArmBreakSound);
        }

        private void HandlePlayArmGrabSound()
        {
            RuntimeManager.PlayOneShot(_playArmGrabSound);
        }

        private void HandlePlayNeckGrabbingSound()
        {
            _neckGrabbingSoundInstance = RuntimeManager.CreateInstance(_playNeckGrabbingSound);
            _neckGrabbingSoundInstance.start();
        }

        private void HandleStopNeckGrabbingSound()
        {
            _neckGrabbingSoundInstance.stop(STOP_MODE.ALLOWFADEOUT);
            _neckGrabbingSoundInstance.release();
        }

        private void HandlePlayCharacterFallingOnGroundSound()
        {
            RuntimeManager.PlayOneShot(_playCharacterFallingOnGroundSound);
        }

        private void HandlePlayNeckGrabSound()
        {
            RuntimeManager.PlayOneShot(_playNeckGrabSound);
        }

        private void HandlePlayPunchImpactSound()
        {
            WeaponDataSO weaponData = _player.GetCompo<AgentWeaponManager>().CurrentWeapon.WeaponData;
            RuntimeManager.PlayOneShot(weaponData.hitImpactSound, Camera.main.transform.position);
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

        private void HandleFinisherSequenceShake(float impulseForce)
        {
            _impulseSource.GenerateImpulse(impulseForce);
        }
    }
}