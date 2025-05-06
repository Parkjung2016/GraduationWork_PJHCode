using Animancer;
using UnityEngine;

namespace PJH.Runtime.Players
{
    [CreateAssetMenu(menuName = "SO/Combat/PlayerCounterAttackData")]
    public class PlayerCounterAttackDataSO : ScriptableObject
    {
        public ClipTransition attackAnimationClip;
        public ClipTransition getDamagedAnimationClip;
    }
}