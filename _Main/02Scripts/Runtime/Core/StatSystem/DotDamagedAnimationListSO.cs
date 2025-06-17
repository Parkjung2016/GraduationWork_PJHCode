using Animancer;
using UnityEngine;

namespace Main.Runtime.Core.StatSystem
{
    [CreateAssetMenu(menuName = "SO/DotDamagedAnimationList")]
    public class DotDamagedAnimationListSO : ScriptableObject
    {
        public ClipTransition[] getDamagedAnimations;
    }
}