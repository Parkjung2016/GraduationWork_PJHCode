using Animancer;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PJH.Runtime.Players
{
    [CreateAssetMenu(menuName = "SO/Combat/PlayerCounterAttackData")]
    public class PlayerCounterAttackDataSO : SerializedScriptableObject
    {
        public ClipTransition attackAnimationClip;
        public ClipTransition getDamagedAnimationClip;
        public float damageMultiplier = 1.0f;
    }
}