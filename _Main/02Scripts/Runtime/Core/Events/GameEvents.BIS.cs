using UnityEngine;

namespace Main.Runtime.Core.Events
{
    public static partial class GameEvents
    {
        public static readonly SceneChangeEvent SceneChangeEvent = new SceneChangeEvent();
        public static readonly SpawnDropItem SpawnDropItem = new SpawnDropItem();
    }

    public class SceneChangeEvent : GameEvent
    {
        public string changeSceneName;
    }

    public class SpawnDropItem : GameEvent
    {
        public PoolTypeSO itemType;
        public Vector3 position;
        public Quaternion rotation;
    }
}