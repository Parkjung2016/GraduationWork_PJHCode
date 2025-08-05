using System.Collections.Generic;
using PJH.Runtime.Players.FinisherSequence;
using UnityEngine;
using UnityEngine.Playables;
using YTH.Shared;

namespace Main.Runtime.Core.Events
{
    public static partial class GameEvents
    {
        public static readonly CameraViewConfig CameraViewConfig = new CameraViewConfig();
        public static readonly EnemyFinisherSequence EnemyFinisherSequence = new EnemyFinisherSequence();
        public static readonly FinishEnemyFinisher FinishEnemyFinisher = new FinishEnemyFinisher();
        public static readonly DeadFinisherTarget DeadFinisherTarget = new DeadFinisherTarget();
        public static readonly PlayerDeath PlayerDeath = new PlayerDeath();
        public static readonly TimeSlowByPlayer TimeSlowByPlayer = new TimeSlowByPlayer();
        public static readonly PlayerStunned PlayerStunned = new PlayerStunned();
        public static readonly ClearWave ClearWave = new ClearWave();
        public static readonly StartWave StartWave = new StartWave();
        public static readonly FinishAllWave FinishAllWave = new FinishAllWave();
        public static readonly DestroyDeadEnemy DestroyDeadEnemy = new DestroyDeadEnemy();
        public static readonly CameraInvertInput CameraInvertInput = new CameraInvertInput();
        public static readonly ChangeCameraFOV ChangeCameraFOV = new ChangeCameraFOV();
        public static readonly ChangeCameraUpdate ChangeCameraUpdate = new ChangeCameraUpdate();
        public static readonly LockOn LockOn = new LockOn();
        public static readonly BossDead BossDead = new BossDead();
        public static readonly EnemyDead EnemyDead = new EnemyDead();
        public static readonly EnableCameraMovement EnableCameraMovement = new EnableCameraMovement();
        public static readonly EnterBossRoom EnterBossRoom = new EnterBossRoom();
        public static readonly StartBossBattle StartBossBattle = new StartBossBattle();
    }
    public class EnemyDead : GameEvent
    {
    }
    public class StartBossBattle : GameEvent
    {
    }
    public class EnterBossRoom : GameEvent
    {
    }
    public class LockOn : GameEvent
    {
        public bool isLockOn;
    }
    public class EnableCameraMovement : GameEvent
    {
        public bool enableCameraMovmement;
    }
    public class BossDead : GameEvent
    {
    }
    public class DestroyDeadEnemy : GameEvent
    {
        public bool isPlayingBossDeathTimeline;
    }

    public class ClearWave : GameEvent
    {
    }

    public class StartWave : GameEvent
    {
        public ScriptableObject enemyPartySO;
        public GameObject[] spawnPoints;
        public List<IEnemy> enemies;
    }

    public class FinishAllWave : GameEvent
    {
    }

    public class TimeSlowByPlayer : GameEvent
    {
        public bool isEnabledEffect;
    }

    public class CameraViewConfig : GameEvent
    {
        public bool isChangeConfig;
    }

    public class ChangeCameraFOV : GameEvent
    {
        public bool resetFOV;
        public bool ignoreTimeScale;
        public float fovValue;
        public float changeDuration;
    }

    public class ChangeCameraUpdate : GameEvent
    {
        public bool updateIgnoreTimeScale;
    }

    public class CameraInvertInput : GameEvent
    {
        public bool isInvertXAxis;
        public bool isInvertYAxis;
    }

    public class EnemyFinisherSequence : GameEvent
    {
        public Animator playerAnimator;
        public Animator targetAnimator;
        public FinisherDataSO sequenceAsset;
    }

    public class PlayerStunned : GameEvent
    {
        public bool isStunned;
    }
    public class FinishEnemyFinisher : GameEvent
    {
    }

    public class DeadFinisherTarget : GameEvent
    {
    }

    public class PlayerDeath : GameEvent
    {
    }
}