using Main.Core;
using Main.Runtime.Agents;
using Main.Runtime.Core;
using Main.Runtime.Core.Events;
using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using UnityEngine;


namespace PJH.Runtime.Players
{
    public class PlayerEffect : SerializedMonoBehaviour, IAgentComponent, IAfterInitable
    {
        private Player _player;
        private PlayerAttack _attackCompo;
        private GameEventChannelSO _spawnEventChannel;
        private GameEventChannelSO _gameEventChannel;
        [SerializeField] private PoolTypeSO _parryingEffectPoolType;
        [SerializeField] private MMF_Player _applyDamagedFeedback;

        public void Initialize(Agent agent)
        {
            _gameEventChannel = AddressableManager.Load<GameEventChannelSO>("GameEventChannel");
            _spawnEventChannel = AddressableManager.Load<GameEventChannelSO>("SpawnEventChannel");

            _player = agent as Player;
            _attackCompo = _player.GetCompo<PlayerAttack>();
        }

        public void AfterInitialize()
        {
            _player.HealthCompo.OnApplyDamaged += HandleApplyDamaged;

            _player.OnStartStun += HandleStartStun;
            _player.OnEndStun += HandleEndStun;

            PlayerHealth playerHealth = (_player.HealthCompo as PlayerHealth);
            playerHealth.OnParrying += HandleParrying;
        }

        private void OnDestroy()
        {
            _player.HealthCompo.OnApplyDamaged -= HandleApplyDamaged;
            _player.OnStartStun -= HandleStartStun;
            _player.OnEndStun -= HandleEndStun;


            PlayerHealth playerHealth = (_player.HealthCompo as PlayerHealth);
            playerHealth.OnParrying -= HandleParrying;
        }

        private void HandleStartStun()
        {
            var evt = GameEvents.PlayerStunned;
            evt.isStunned = true;
            _gameEventChannel.RaiseEvent(evt);
        }

        private void HandleEndStun()
        {
            var evt = GameEvents.PlayerStunned;
            evt.isStunned = false;
            _gameEventChannel.RaiseEvent(evt);
        }


        private void HandleParrying()
        {
            var evt = SpawnEvents.SpawnEffect;
            evt.effectType = _parryingEffectPoolType;
            evt.position = _player.HealthCompo.GetDamagedInfo.hitPoint;
            evt.rotation = Quaternion.identity;
            _spawnEventChannel.RaiseEvent(evt);
        }

        private void HandleApplyDamaged(float damage)
        {
            _applyDamagedFeedback.PlayFeedbacks();
        }
    }
}