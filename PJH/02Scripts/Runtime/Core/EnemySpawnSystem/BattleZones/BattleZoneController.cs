using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Main.Core;
using Main.Runtime.Core.Events;
using Main.Runtime.Manager;
using Main.Shared;
using UnityEngine;

namespace PJH.Runtime.Core.EnemySpawnSystem
{
    public class BattleZoneController : MonoBehaviour, IBattleZoneController
    {
        private AstarPath _astarPathCompo;
        [SerializeField] private ParticleSystem _nextPointIndicatorParticle;
        public event Action<int> OnChangedRemainingEnemy;
        public IBattleZone CurrentBattleZone { get; private set; }

        private int _remainingEnemy;

        public int RemainingEnemy
        {
            get => _remainingEnemy;
            private set
            {
                _remainingEnemy = value;
                OnChangedRemainingEnemy?.Invoke(value);
            }
        }

        private List<BattleZone> _battleZones;
        private GameEventChannelSO _gameEventChannel;

        private bool _isInitialized;

        private void Awake()
        {
            _astarPathCompo = FindAnyObjectByType<AstarPath>();
            _battleZones = transform.GetComponentsInChildren<BattleZone>().ToList();
            _battleZones.ForEach(zone =>
            {
                zone.OnEntered += SetCurrentBattleZone(zone);
                if (zone is EnemyBattleZone enemyBattleZone)
                {
                    enemyBattleZone.OnEnemyRemoved += HandleEnemyRemoved();
                    enemyBattleZone.OnEnemyAdded += HandleEnemyAdded();
                }
            });
        }

        private void Start()
        {
            _battleZones.ForEach(zone => { zone.Init(); });
            _isInitialized = true;
        }

        private async void OnEnable()
        {
            SubscribeEvents();
            await UniTask.WaitUntil(() => SceneManagerEx.Instance.CurrentScene != null,
                cancellationToken: gameObject.GetCancellationTokenOnDestroy());
            _astarPathCompo.Scan();
            await UniTask.WaitUntil(() => _isInitialized,
                cancellationToken: gameObject.GetCancellationTokenOnDestroy());
            await UniTask.WaitForSeconds(.15f,
     cancellationToken: gameObject.GetCancellationTokenOnDestroy());
            (SceneManagerEx.Instance.CurrentScene as IBattleScene).CurrentBattleZoneController = this;
            CheckActiveNextSession();
        }

        private void OnDisable()
        {
            UnSubscribeEvents();
        }

        private Action SetCurrentBattleZone(BattleZone battleZone)
        {
            return () => { CurrentBattleZone = battleZone; };
        }

        private Action HandleEnemyAdded()
        {
            return () => { RemainingEnemy++; };
        }

        private Action HandleEnemyRemoved()
        {
            return () => { RemainingEnemy--; };
        }

        protected bool TryGetGameEventChannel()
        {
            if (!_gameEventChannel)
                _gameEventChannel = AddressableManager.Load<GameEventChannelSO>("GameEventChannel");
            return _gameEventChannel;
        }

        private void SubscribeEvents()
        {
            if (!TryGetGameEventChannel()) return;

            _gameEventChannel.AddListener<StartWave>(HandleStartWave);
            _gameEventChannel.AddListener<DestroyDeadEnemy>(HandleDestroyDeadEnemy);
            _gameEventChannel.AddListener<FinishAllWave>(HandleFinishAllWave);
            _gameEventChannel.AddListener<EnterNextLevel>(HandleEnterNextLevel);
        }

        private void UnSubscribeEvents()
        {
            if (!TryGetGameEventChannel()) return;

            _gameEventChannel.RemoveListener<StartWave>(HandleStartWave);
            _gameEventChannel.RemoveListener<DestroyDeadEnemy>(HandleDestroyDeadEnemy);
            _gameEventChannel.RemoveListener<FinishAllWave>(HandleFinishAllWave);
            _gameEventChannel.RemoveListener<EnterNextLevel>(HandleEnterNextLevel);
        }

        private void HandleEnterNextLevel(EnterNextLevel evt)
        {
            _battleZones.ForEach(zone => { zone.Dispose(); });
        }

        private void HandleDestroyDeadEnemy(DestroyDeadEnemy evt)
        {
            _battleZones.ForEach(zone => { zone.Dispose(); });
        }

        private void HandleStartWave(StartWave evt)
        {
            for (int i = 0; i < _battleZones.Count; i++)
            {
                BattleZone battleZone = _battleZones[i];
                if (battleZone is BossBattleZone) continue;
                battleZone.LockZone();
            }
        }

        private void HandleFinishAllWave(FinishAllWave evt)
        {
            for (int i = 0; i < _battleZones.Count; i++)
            {
                BattleZone battleZone = _battleZones[i];
                if (battleZone is BossBattleZone) continue;
                battleZone.UnLockZone();
            }

            EnemyBattleZone enemyBattleZone = evt.battleZone as EnemyBattleZone;
            CurrentBattleZone = null;
            enemyBattleZone.OnEntered -= SetCurrentBattleZone(enemyBattleZone);
            enemyBattleZone.OnEnemyRemoved -= HandleEnemyRemoved();
            enemyBattleZone.OnEnemyAdded -= HandleEnemyAdded();


            CheckActiveNextSession();
        }

        private void CheckActiveNextSession()
        {
            if (!TryGetGameEventChannel()) return;
            if (RemainingEnemy <= 0)
            {
                if (_nextPointIndicatorParticle)
                    _nextPointIndicatorParticle.Play();
                _gameEventChannel.RaiseEvent(GameEvents.ActiveNextSession);
            }
        }
    }
}