using System.Collections.Generic;
using System.Threading.Tasks;
using BIS.Data;
using Main.Runtime.Agents;
using Main.Runtime.Core.Events;
using PJH.Utility.Extensions;
using PJH.Utility.Managers;
using UnityEngine;
using YTH.Enemies;
using Debug = UnityEngine.Debug;

namespace PJH.Runtime.Core.EnemySpawnSystem
{
    public class WaveSystem : MonoBehaviour
    {
        private GameEventChannelSO _gameEventChannel;
        private PoolManagerSO _poolManager;
        private EnemySpawnPoint[] _spawnPoints;
        private EnemyPartySO _enemyParty;

        private List<Agent> _enemyList = new();
        public List<Agent> EnemyList => _enemyList;

        public EnemyPartySO PartySO
        {
            get => _enemyParty;
            set => _enemyParty = value;
        }

        public EnemySpawnPoint[] SpawnPoints
        {
            get => _spawnPoints;
            set => _spawnPoints = value;
        }

        private void Awake()
        {
            _gameEventChannel = AddressableManager.Load<GameEventChannelSO>("GameEventChannel");
            _poolManager = AddressableManager.Load<PoolManagerSO>("PoolManager");

            _gameEventChannel.AddListener<ClearWave>(HandleClearWave);
        }

        private void OnDestroy()
        {
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

        public List<Agent> SpawnEnemies()
        {
            List<Agent> spawnList = new();
            EnemyPartySO enemyParty = _enemyParty;
            if (_enemyParty == null) Debug.LogError($"Enemy Party SO is Null Please Change Null SO");
            List<SpawnData> spawnDatas = enemyParty.UnitDatas[0].spawnData;
            if (spawnDatas.Count <= 0) Debug.LogError($"Current SpawnDatas Count is Zero. Please Add Value");
            int spawnDataIndex = 0;
            for (byte i = 0; i < _spawnPoints.Length;)
            {
                SpawnData spawnData;
                if (spawnDataIndex >= spawnDatas.Count)
                {
                    spawnData = spawnDatas.Random();
                }
                else
                    spawnData = spawnDatas[spawnDataIndex++];

                if (spawnData.spawnAmount <= 0)
                {
                    i++;
                    continue;
                }

                for (byte j = 0; j < spawnData.spawnAmount; j++)
                {
                    UnitSO unit = spawnData.spawnUnit;

                    EnemySpawnPoint spawnPoint = _spawnPoints[i];
                    BaseEnemy enemy = spawnPoint.SpawnEnemy(unit.UnitPoolType);
                    _enemyList.Add(enemy);
                    spawnList.Add(enemy);
                    i++;
                    if (i >= _spawnPoints.Length)
                        break;
                }
            }

            return spawnList;
        }

        // private void RemoveEnemy(Agent enemy)
        // {
        //     if (_enemyList.Remove(enemy))
        //     {
        //         if (_enemyList.Count == 0)
        //         {
        //             if (CurrentWave >= _maxWave)
        //             {
        //                 CurrentWave = 1;
        //                 var evt = GameEvents.FinishAllWave;
        //                 _gameEventChannel.RaiseEvent(evt);
        //             }
        //             else
        //             {
        //                 CurrentWave++;
        //                 var evt = GameEvents.ClearWave;
        //                 _gameEventChannel.RaiseEvent(evt);
        //             }
        //         }
        //     }
        // }
    }
}