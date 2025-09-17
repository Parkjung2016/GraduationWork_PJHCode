using Animancer;
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
        public bool isDotDamage;
        public ITransition getDamagedAnimationClipOnIgnoreDirection;
        public float knockDownTime;
        public ITransition getUpAnimationClip;

        public GetDamagedInfo SetHitPoint(Vector3 hitPoint)
        {
            this.hitPoint = hitPoint;
            return this;
        }

        public GetDamagedInfo SetNormal(Vector3 normal)
        {
            this.normal = normal;
            return this;
        }

        public GetDamagedInfo SetDamage(float damage)
        {
            this.damage = damage;
            return this;
        }

        public GetDamagedInfo SetIncreaseMomentumGauge(float increaseMomentumGauge)
        {
            this.increaseMomentumGauge = increaseMomentumGauge;
            return this;
        }

        public GetDamagedInfo SetGetDamagedAnimationClip(GetDamagedAnimationClipInfo clip)
        {
            this.getDamagedAnimationClip = clip;
            return this;
        }

        public GetDamagedInfo SetAttacker(MonoBehaviour attacker)
        {
            this.attacker = attacker;
            return this;
        }

        public GetDamagedInfo SetIsKnockDown(bool isKnockDown)
        {
            this.isKnockDown = isKnockDown;
            return this;
        }

        public GetDamagedInfo SetIsForceAttack(bool isForceAttack)
        {
            this.isForceAttack = isForceAttack;
            return this;
        }

        public GetDamagedInfo SetIgnoreDirection(bool ignoreDirection)
        {
            this.ignoreDirection = ignoreDirection;
            return this;
        }

        public GetDamagedInfo SetIsDotDamage(bool isDotDamage)
        {
            this.isDotDamage = isDotDamage;
            return this;
        }

        public GetDamagedInfo SetGetDamagedAnimationClipOnIgnoreDirection(ITransition transition)
        {
            this.getDamagedAnimationClipOnIgnoreDirection = transition;
            return this;
        }

        public GetDamagedInfo SetKnockDownTime(float knockDownTime)
        {
            this.knockDownTime = knockDownTime;
            return this;
        }

        public GetDamagedInfo SetGetUpAnimationClip(ITransition transition)
        {
            this.getUpAnimationClip = transition;
            return this;
        }
    }
}