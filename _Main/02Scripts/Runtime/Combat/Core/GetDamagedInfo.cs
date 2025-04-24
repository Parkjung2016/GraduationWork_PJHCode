using Animancer;
using Main.Shared;
using UnityEngine;

namespace Main.Runtime.Combat.Core
{
    public struct GetDamagedInfo
    {
        public Vector3 hitPoint;
        public float damage;
        public float increaseMomentumGauge;
        public ClipTransition getDamagedAnimationClip;
        public IAgent attacker;
        public bool isKnockDown;
        public bool isForceAttack;
        public float knockDownTime;
        public ClipTransition getUpAnimationClip;
    }
}