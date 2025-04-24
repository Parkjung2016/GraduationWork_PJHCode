using Main.Core;
using Main.Runtime.Core.Events;
using Main.Runtime.Manager;
using UnityEngine;

namespace Main.Runtime.Core
{
    public class SpawnManager : MonoBehaviour
    {
        private GameEventChannelSO _spawnEventChannel;
        private PoolManagerSO _poolManager;

        private void Awake()
        {
            _spawnEventChannel =AddressableManager.Load<GameEventChannelSO>("SpawnEventChannel");
            _poolManager = AddressableManager.Load<PoolManagerSO>("PoolManager");
        }

        private void Start()
        {
            _spawnEventChannel.AddListener<SpawnEffect>(HandleSpawnEffect);
        }

        private void OnDestroy()
        {
            _spawnEventChannel.RemoveListener<SpawnEffect>(HandleSpawnEffect);
        }

        private void HandleSpawnEffect(SpawnEffect evt)
        {
            if (!_poolManager) return;
            var effect = _poolManager.Pop(evt.effectType) as PoolEffectPlayer;
            effect.PlayEffects(evt.position, evt.rotation);
        }
    }
}