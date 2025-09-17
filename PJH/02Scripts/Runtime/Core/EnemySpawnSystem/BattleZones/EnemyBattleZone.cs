using System;
using System.Collections.Generic;
using BIS.Data;
using Main.Runtime.Core.Events;
using PJH.Utility.Managers;
using UnityEngine;
using YTH.Enemies;
using YTH.Shared;
using ZLinq;

namespace PJH.Runtime.Core.EnemySpawnSystem
{
    public class EnemyBattleZone : BattleZone
    {
        public event Action OnEnemyAdded;
        public event Action OnEnemyRemoved;
        [SerializeField] private EnemyPartyTableSO _enemyPartyTable;
        [SerializeField] private int _earnGold = 2000;
        private List<BaseEnemy> _enemies = new();
        private List<BaseEnemy> _deadEnemies = new();
        private EnemySpawnPoint[] _spawnPoints;
        private CurrencySO _money;
        private Dictionary<BaseEnemy, Action> _deathHandlers = new();

        protected override void Awake()
        {
            base.Awake();
            _money = AddressableManager.Load<CurrencySO>("Money");
            _spawnPoints = transform.GetComponentsInChildren<EnemySpawnPoint>();
        }

        public override void Init()
        {
            SpawnEnemies();
        }

        public override void Dispose()
        {
            _enemies.ForEach(enemy => { enemy.ReturnPool(); });
            _deadEnemies.ForEach(enemy => { enemy.ReturnPool(); });
        }

        protected override void EnterZone()
        {
            for (int i = 0; i < _spawnPoints.Length; i++)
            {
                _spawnPoints[i].PrepareForBattle();
            }

            var changeCurrentEnemyEvt = GameEvents.ChangeCurrentEnemy;
            changeCurrentEnemyEvt.enemyCount = _enemies.Count;
            _gameEventChannel.RaiseEvent(changeCurrentEnemyEvt);
            var evt = GameEvents.StartWave;
            evt.enemies = _enemies.AsValueEnumerable().Select(x => x as IEnemy).ToList();
            _gameEventChannel.RaiseEvent(GameEvents.StartWave);
            CheckEnemies();
        }


        private void SpawnEnemies()
        {
            EnemyPartySO enemyParty = _enemyPartyTable.GetRandomEnemyPartySO();
            if (!enemyParty) return;
            List<SpawnData> spawnDatas = enemyParty.UnitDatas[0].spawnData;
            int spawnDataIndex = 0;
            if (spawnDatas.Count <= 0) return;
            for (byte i = 0; i < _spawnPoints.Length;)
            {
                SpawnData spawnData;
                if (spawnDataIndex >= spawnDatas.Count)
                {
                    spawnDatas = spawnDatas.AsValueEnumerable().OrderBy(x => Guid.NewGuid()).ToList();
                    spawnDataIndex = 0;
                }

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
                    _enemies.Add(enemy);
                    Action handler = () => HandleDeathEnemy(enemy);
                    _deathHandlers.Add(enemy, handler);
                    enemy.HealthCompo.OnDeath += handler;
                    i++;
                    OnEnemyAdded?.Invoke();
                    if (i >= _spawnPoints.Length)
                        break;
                }
            }
        }

        private void OnDestroy()
        {
            UnSubscribeEvent();
        }

        private void HandleDeathEnemy(BaseEnemy enemy)
        {
            _enemies.Remove(enemy);
            _deadEnemies.Add(enemy);
            var deathHandler = _deathHandlers[enemy];
            _deathHandlers.Remove(enemy);
            enemy.HealthCompo.OnDeath -= deathHandler;
            OnEnemyRemoved?.Invoke();
            var changeCurrentEnemyEvt = GameEvents.ChangeCurrentEnemy;
            changeCurrentEnemyEvt.enemyCount = _enemies.Count;
            _gameEventChannel.RaiseEvent(changeCurrentEnemyEvt);
            CheckEnemies();
        }

        private void CheckEnemies()
        {
            if (_enemies.Count <= 0)
            {
                UnSubscribeEvent();

                _money.AddAmmount(_earnGold);
                var evt = GameEvents.FinishAllWave;
                evt.battleZone = this;
                var changeCurrentEnemyEvt = GameEvents.ChangeCurrentEnemy;
                changeCurrentEnemyEvt.enemyCount = 99;
                _gameEventChannel.RaiseEvent(changeCurrentEnemyEvt);
                _gameEventChannel.RaiseEvent(evt);
            }
        }

        private void UnSubscribeEvent()
        {
            foreach (var pair in _deathHandlers)
            {
                pair.Key.HealthCompo.OnDeath -= pair.Value;
            }

            _deathHandlers.Clear();
        }
    }
}