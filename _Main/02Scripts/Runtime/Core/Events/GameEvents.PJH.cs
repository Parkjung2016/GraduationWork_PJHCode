using UnityEngine;
using UnityEngine.Playables;

namespace Main.Runtime.Core.Events
{
    public static partial class GameEvents
    {
        public static readonly CameraViewConfig CameraViewConfig = new CameraViewConfig();
        public static readonly EnemyFinisherSequence EnemyFinisherSequence = new EnemyFinisherSequence();
        public static readonly FinishTimeline FinishTimeline = new FinishTimeline();
        public static readonly DeadFinisherTarget DeadFinisherTarget = new DeadFinisherTarget();
        public static readonly PlayerDeath PlayerDeath = new PlayerDeath();
        public static readonly TimeSlowByPlayer TimeSlowByPlayer = new TimeSlowByPlayer();
        public static readonly ReOffsetPlayer ReOffsetPlayer = new ReOffsetPlayer();
        public static readonly PlayerStunned PlayerStunned = new PlayerStunned();
        public static readonly ClearWave ClearWave = new ClearWave();
        public static readonly StartWave StartWave = new StartWave();
        public static readonly FinishAllWave FinishAllWave = new FinishAllWave();
        public static readonly EndBattle EndBattle = new EndBattle();
        public static readonly DestroyDeadEnemy DestroyDeadEnemy = new DestroyDeadEnemy();
        public static readonly CameraInvertInput CameraInvertInput = new CameraInvertInput();
        public static readonly ChangeCameraFOV ChangeCameraFOV = new ChangeCameraFOV();
        public static readonly ChangeCameraUpdate ChangeCameraUpdate = new ChangeCameraUpdate();
    }

    public class DestroyDeadEnemy : GameEvent
    {
    }

    public class ClearWave : GameEvent
    {
    }

    public class EndBattle : GameEvent
    {
    }

    public class StartWave : GameEvent
    {
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
        public PlayableAsset sequenceAsset;
        public Animator enemyAnimator;
    }

    public class PlayerStunned : GameEvent
    {
        public bool isStunned;
    }

    public class ReOffsetPlayer : GameEvent
    {
    }

    public class FinishTimeline : GameEvent
    {
    }

    public class DeadFinisherTarget : GameEvent
    {
    }

    public class PlayerDeath : GameEvent
    {
    }
}