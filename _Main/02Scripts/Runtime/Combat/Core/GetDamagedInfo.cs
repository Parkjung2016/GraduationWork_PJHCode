using System;
using Animancer;
using Main.Shared;
using UnityEngine;

namespace Main.Runtime.Combat.Core
{
    public struct GetDamagedInfo
    {
        public Vector3 hitPoint;
        public Vector3 normal;
        public float damage;
        public float increaseMomentumGauge;
        public GetDamagedAnimationClipInfo getDamagedAnimationClip;
        public MonoBehaviour attacker;
        public bool isKnockDown;
        public bool isForceAttack;
        public bool ignoreDirection;
        public ITransition getDamagedAnimationClipOnIgnoreDirection;
        public float knockDownTime;
        public ITransition getUpAnimationClip;
    }
}