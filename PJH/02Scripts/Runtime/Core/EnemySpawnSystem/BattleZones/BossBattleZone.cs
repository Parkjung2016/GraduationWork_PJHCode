using System.Collections.Generic;
using DG.Tweening;
using Main.Runtime.Core.Events;
using UnityEngine;
using YTH.Boss;

namespace PJH.Runtime.Core.EnemySpawnSystem
{
    public class BossBattleZone : BattleZone
    {
        [SerializeField] private float _openDuration = 1f, _openY = 120;
        [SerializeField] private GameObject _leftDoor, _rightDoor;
        [SerializeField] private GameObject[] _hideObjectsInEnter;
        [SerializeField] private PoolTypeSO _bossPoolType;
        [SerializeField] private Transform _bossDeathTimelinePoint;

        public List<Transform> StaffSpawnPoints { get; private set; }
        private EnemySpawnPoint _bossSpawnPoint;
        private Boss _boss;

        public Transform GetBossDeathTimelinePoint() => _bossDeathTimelinePoint;
        public Boss GetBoss() => _boss;

        protected override void Awake()
        {
            base.Awake();
            _bossSpawnPoint = transform.Find("BossSpawnPoint").GetComponent<EnemySpawnPoint>();
            _leftDoor.transform.localRotation = Quaternion.identity;
            _rightDoor.transform.localRotation = Quaternion.identity;

            Transform staffSpawnPointsParent = transform.Find("StaffSpawnPoints");
            StaffSpawnPoints = new List<Transform>();
            if (staffSpawnPointsParent)
            {
                foreach (Transform childTrm in staffSpawnPointsParent)
                {
                    StaffSpawnPoints.Add(childTrm);
                }
            }
        }

        public override void Init()
        {
            _boss = _bossSpawnPoint.SpawnEnemy(_bossPoolType) as Boss;
        }

        public override void Dispose()
        {
            if (GameEvents.DestroyDeadEnemy.isPlayingBossDeathTimeline) return;
            _boss.ReturnPool();
        }

        private void OnEnable()
        {
            if (!TryGetGameEventChannel()) return;
            _gameEventChannel.AddListener<ActiveNextSession>(HandleActiveNextSession);
            _gameEventChannel.AddListener<StartBossBattle>(HandleStartBossBattle);
        }

        private void OnDisable()
        {
            if (!TryGetGameEventChannel()) return;

            _gameEventChannel.RemoveListener<ActiveNextSession>(HandleActiveNextSession);
            _gameEventChannel.RemoveListener<StartBossBattle>(HandleStartBossBattle);
        }

        private void HandleStartBossBattle(StartBossBattle evt)
        {
            _bossSpawnPoint.PrepareForBattle();
            _gameEventChannel.RaiseEvent(GameEvents.StartWave);
        }

        private void HandleActiveNextSession(ActiveNextSession obj)
        {
            OpenDoor(true);
        }

        private void OpenDoor(bool opened)
        {
            Vector3 angle = !opened ? Vector3.zero : Vector3.up * _openY;
            _leftDoor.transform.DOLocalRotate(-angle, _openDuration);
            _rightDoor.transform.DOLocalRotate(angle, _openDuration);
        }

        protected override void EnterZone()
        {
            for (int i = 0; i < _hideObjectsInEnter.Length; i++)
                _hideObjectsInEnter[i].gameObject.SetActive(false);

            var changeCurrentEnemyEvt = GameEvents.ChangeCurrentEnemy;
            changeCurrentEnemyEvt.enemyCount = 0;
            _gameEventChannel.RaiseEvent(changeCurrentEnemyEvt);
            OpenDoor(false);
            _gameEventChannel.RaiseEvent(GameEvents.EnterBossRoom);
        }
    }
}