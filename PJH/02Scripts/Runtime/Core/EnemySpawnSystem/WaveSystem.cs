using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BIS.Data;
using BIS.Events;
using Main.Core;
using Main.Runtime.Agents;
using Main.Runtime.Core.Events;
using UnityEngine;
using UnityEngine.AI;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace PJH.Runtime.Core.EnemySpawnSystem
{
    public class WaveSystem : MonoBehaviour
    {
        public event Action<byte> OnChangedCurrentWaveAction;

        public byte CurrentWave
        {
            get => _currentWave;
            set
            {
                byte prevWave = _currentWave;
                _currentWave = value;
                if (prevWave != _currentWave)
                    OnChangedCurrentWaveAction?.Invoke(_currentWave);
            }
        }

        private byte _currentWave;
        private byte _maxWave;
        private GameEventChannelSO _gameEventChannel;
        private PoolManagerSO _poolManager;
        private GameObject[] _spawnPoints;
        private EnemyPartySO _enemyParty;

        private List<Agent> _enemyList = new();

        private void Awake()
        {
            _currentWave = 1;
            _gameEventChannel = AddressableManager.Load<GameEventChannelSO>("GameEventChannel");

            _maxWave = (byte)UIEvent.EnemyPrevieChoiceEvent.enemyPartySO.UnitDatas.Count;
            _poolManager = AddressableManager.Load<PoolManagerSO>("PoolManager");
            _gameEventChannel.AddListener<StartWave>(HandleStartWave);
            _gameEventChannel.AddListener<ClearWave>(HandleClearWave);
        }

        private void OnDestroy()
        {
            _gameEventChannel.RemoveListener<StartWave>(HandleStartWave);
            _gameEventChannel.RemoveListener<ClearWave>(HandleClearWave);
        }

        private async void HandleClearWave(ClearWave evt)
        {
            await Task.Delay(2000);
            _gameEventChannel.RaiseEvent(GameEvents.StartWave);
        }

        private void Start()
        {
            _gameEventChannel.RaiseEvent(GameEvents.StartWave);
        }

        private void HandleStartWave(StartWave evt)
        {
            StartWave();
        }
        
        public void StartWave()
        {
            SpawnEnemies();
        }

        public void SetCurrentEnemyWave(EnemyPartySO enemyPartySO, GameObject[] spawnPoints)
        {
            _enemyParty = enemyPartySO;
            _spawnPoints = spawnPoints;
        }

        private void SpawnEnemies()
        {
            try
            {
                EnemyPartySO enemyParty = _enemyParty;
                List<SpawnData> spawnDatas = enemyParty.UnitDatas[CurrentWave - 1].spawnData;
                List<GameObject> copyOfSpawnPoints = _spawnPoints.ToList();
                for (byte i = 0; i < spawnDatas.Count; i++)
                {
                    SpawnData spawnData = spawnDatas[i];
                    for (byte j = 0; j < spawnData.spawnAmount; j++)
                    {
                        UnitSO unit = spawnData.spawnUnit;
                        Agent enemy = _poolManager.Pop(unit.UnitPoolType) as Agent;
                        NavMeshAgent navMeshAgent = enemy.GetComponent<NavMeshAgent>();
                        navMeshAgent.enabled = false;
                        int rnd = Random.Range(0, copyOfSpawnPoints.Count);
                        Transform spawnPoint = copyOfSpawnPoints[rnd].transform;
                        enemy.transform.position = spawnPoint.position;
                        navMeshAgent.enabled = true;
                        copyOfSpawnPoints.RemoveAt(rnd);
                        _enemyList.Add(enemy);
                        enemy.HealthCompo.OnDeath += () => { RemoveEnemy(enemy); };
                    }
                }
            }
            catch(Exception e)
            {
                Debug.LogError($"SpawnEnemies is failed, reason is : {e}");
                Debug.LogError("Please Fix this exception.");
            }
        }

        private void RemoveEnemy(Agent enemy)
        {
            if (_enemyList.Remove(enemy))
            {
                if (_enemyList.Count == 0)
                {
                    if (CurrentWave >= _maxWave)
                    {
                        var evt = GameEvents.FinishAllWave;
                        _gameEventChannel.RaiseEvent(evt);
                    }
                    else
                    {
                        CurrentWave++;
                        var evt = GameEvents.ClearWave;
                        _gameEventChannel.RaiseEvent(evt);
                    }
                }
            }
        }
    }
}