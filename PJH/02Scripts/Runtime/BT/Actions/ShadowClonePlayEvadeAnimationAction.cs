using Animancer;
using Main.Runtime.Combat.Core;
using Opsive.BehaviorDesigner.Runtime.Tasks;
using PJH.Runtime.BossSkill.BossSkills.ShadowClones;
using UnityEngine;

namespace PJH.Runtime.BT.Actions
{
    public class ShadowClonePlayEvadeAnimationAction : ShadowCloneActionNode
    {
        private bool _animationFinished;

        public TransitionAsset frontEvadeAnimation,
            backEvadeAnimation,
            rightEvadeAnimation,
            leftEvadeAnimation;

        public override void OnStart()
        {
            base.OnStart();

            _animationFinished = false;
            GetDamagedInfo getDamagedInfo = _shadowClone.HealthCompo.GetDamagedInfo;
            Vector3 hitPoint = getDamagedInfo.hitPoint;
            Vector3 forward = _shadowClone.transform.forward;
            Vector3 right = _shadowClone.transform.right;
            Vector3 position = _shadowClone.transform.position;

            Vector3 toHit = (hitPoint - position).normalized;

            float forwardDot = Vector3.Dot(forward, toHit); // 앞/뒤 판단
            float rightDot = Vector3.Dot(right, toHit); // 좌/우 판단

            TransitionAsset evadeAnimation;
            if (forwardDot > 0.7f)
                evadeAnimation = frontEvadeAnimation;
            else if (forwardDot < -0.7f)
                evadeAnimation = backEvadeAnimation;
            else if (rightDot > 0)
                evadeAnimation = rightEvadeAnimation;
            else
                evadeAnimation = leftEvadeAnimation;

            _shadowClone.GetCompo<ShadowCloneAnimator>()
                .PlayAnimationClip(evadeAnimation, () => { _animationFinished = true; });
        }

        public override TaskStatus OnUpdate()
        {
            return _animationFinished ? TaskStatus.Success : TaskStatus.Running;
        }
    }
}