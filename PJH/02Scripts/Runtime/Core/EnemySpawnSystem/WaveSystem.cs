using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BIS.Data;
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
        private int _maxWave;
        private GameEventChannelSO _gameEventChannel;
        private PoolManagerSO _poolManager;
        private GameObject[] _spawnPoints;
        private EnemyPartySO _enemyParty;

        private List<Agent> _enemyList = new();
        public List<Agent> EnemyList
        {
            get
            {
                return _enemyList;
            }

            set
            {
                var evt = GameEvents.ChangeCurrentEnemy;
                evt.enemyCount = _enemyList.Count;
                _gameEventChannel.RaiseEvent(evt);
            }
        }

private void Awake()
        {
            _currentWave = 1;
            _gameEventChannel = AddressableManager.Load<GameEventChannelSO>("GameEventChannel");

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

        // private void Start()
        // {
        //     _gameEventChannel.RaiseEvent(GameEvents.StartWave);
        // }

        private void HandleStartWave(StartWave evt)
        {
            _enemyParty = evt.enemyPartySO as EnemyPartySO;
            _spawnPoints = evt.spawnPoints;
            _maxWave = ((EnemyPartySO)evt.enemyPartySO).UnitDatas.Count;
            StartWave();
        }

        public void StartWave()
        {
            SpawnEnemies();
        }

        private void SpawnEnemies()
        {
            // try
            // {
            EnemyPartySO enemyParty = _enemyParty;
            if(_enemyParty == null) Debug.LogError($"Enemy Party SO is Null Please Change Null SO");
            List<SpawnData> spawnDatas = enemyParty.UnitDatas[CurrentWave - 1].spawnData;
            List<GameObject> copyOfSpawnPoints = _spawnPoints.ToList();
            if(spawnDatas.Count <= 0) Debug.LogError($"Current SpawnDatas Count is Zero. Please Add Value");
            for (byte i = 0; i < spawnDatas.Count; i++)
            {
                SpawnData spawnData = spawnDatas[i];
                if(spawnData.spawnAmount <= 0) Debug.LogError($"Current SpawnAmount is Zero Please Add Value");
                for (byte j = 0; j < spawnData.spawnAmount; j++)
                {
                    UnitSO unit = spawnData.spawnUnit;
                    Agent enemy = _poolManager.Pop(unit.UnitPoolType) as Agent;
                    Debug.Log(enemy);
                    int rnd = Random.Range(0, copyOfSpawnPoints.Count);
                    Transform spawnPoint = copyOfSpawnPoints[rnd].transform;
                    enemy.transform.position = spawnPoint.position;
                    _enemyList.Add(enemy);
                    enemy.HealthCompo.OnDeath += () =>
                    {
                        Debug.Log(EnemyList.Count);
                        RemoveEnemy(enemy); 
                    };
                }
            }

            StartCombatEvent evt = GameEvents.StartCombat;
            _gameEventChannel.RaiseEvent(evt);
            // }
            // catch(Exception e)
            // {
            //     Debug.LogError($"SpawnEnemies is failed, reason is : {e}");
            //     Debug.LogError("Please Fix this exception.");
            // }
        }

        // private int[] DivideSpawnCount(int spawnCount, int spawnPoint)
        // {
        //     int[] spawnCountArray = new int[spawnPoint];
        //     if (spawnCount % spawnPoint == 0)
        //     {   
        //         
        //     }
        //     return spawnCount % spawnPoint == 0 ? new int[spawnCount / spawnPoint];
        // }

        private void RemoveEnemy(Agent enemy)
        {
            if (_enemyList.Remove(enemy))
            {
                if (_enemyList.Count == 0)
                {
                    if (CurrentWave >= _maxWave)
                    {
                        CurrentWave = 1;
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