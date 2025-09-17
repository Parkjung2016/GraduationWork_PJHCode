using System;
using System.Collections.Generic;
using Main.Runtime.Core.Events;
using Main.Shared;
using PJH.Utility.Managers;
using UnityEngine;

namespace PJH.Runtime.Core.EnemySpawnSystem
{
    public abstract class BattleZone : MonoBehaviour,IBattleZone
    {
        public event Action<BattleZone> OnEntered;
        protected GameEventChannelSO _gameEventChannel;
        protected PoolManagerSO _poolManager;
        protected Collider _colliderCompo;
        protected bool _isEntered;

        private List<GameObject> _restrictedZoneObjects;

        protected virtual void Awake()
        {
            _restrictedZoneObjects = new();
            Transform restrictedZoneParent = transform.Find("RestrictedZoneObjects");
            if (restrictedZoneParent)
                foreach (Transform childTrm in restrictedZoneParent)
                {
                    _restrictedZoneObjects.Add(childTrm.gameObject);
                }

            _colliderCompo = GetComponent<Collider>();
            TryGetGameEventChannel();
            UnLockZone();
        }

        public virtual void Init()
        {
            
        }

        public virtual void Dispose()
        {
            
        }
        protected bool TryGetGameEventChannel()
        {
            if (!_gameEventChannel)
            {
                _gameEventChannel = AddressableManager.Load<GameEventChannelSO>("GameEventChannel");
                _poolManager = AddressableManager.Load<PoolManagerSO>("PoolManager");
            }

            return _gameEventChannel;
        }

        public virtual void LockZone()
        {
            for (int i = 0; i < _restrictedZoneObjects.Count; i++)
            {
                _restrictedZoneObjects[i].SetActive(true);
            }

            _colliderCompo.enabled = false;
        }

        public virtual void UnLockZone()
        {
            for (int i = 0; i < _restrictedZoneObjects.Count; i++)
            {
                _restrictedZoneObjects[i].SetActive(false);
            }

            _colliderCompo.enabled = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            if (_isEntered) return;
            _isEntered = true;
            OnEntered?.Invoke(this);
            EnterZone();
        }

        protected abstract void EnterZone();
    }
}