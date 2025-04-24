using UnityEngine;

namespace Main.Runtime.Core.Events
{
    public static class SpawnEvents
    {
        public static readonly SpawnEffect SpawnEffect = new();
    }

    public class SpawnEffect : GameEvent
    {
        public PoolTypeSO effectType;
        public Vector3 position;
        public Quaternion rotation;
    }
}