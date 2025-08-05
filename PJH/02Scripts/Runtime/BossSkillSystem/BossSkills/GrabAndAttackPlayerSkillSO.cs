using Animancer;
using DG.Tweening;
using FMODUnity;
using Kinemation.MotionWarping.Runtime.Core;
using Kinemation.MotionWarping.Runtime.Examples;
using Main.Runtime.Agents;
using PJH.Runtime.Players;
using UnityEngine;
using YTH.Enemies;

namespace PJH.Runtime.BossSkill.BossSkills
{
    [CreateAssetMenu(menuName = "SO/BossSkill/Skills/GrabAndAttackPlayerSkill")]
    public class GrabAndAttackPlayerSkillSO : BossSkillSO
    {
        public ClipTransition tryGrabAnimationClip;
        public MotionWarpingAsset attackerMotionWarpingAsset;
        public ClipTransition grabAndAttackVictimAnimationClip;
        public StringAsset tryGrabEvent;
        public EventReference grabSound;
        public EventReference hitSound;

        public float grabDistance = 1.3f;
        public float attackPowerMultiplier = 1.0f;
        private bool _animationFinished;

        private AnimancerState _tryGrabAnimancerState;

        public override void ActivateSkill()
        {
            _animationFinished = false;
            Player player = GetPlayer();
            AgentAnimator animatorCompo = _boss.GetCompo<AgentAnimator>(true);
            _boss.GetCompo<AgentAnimator>(true).lockedTransitionAnimation = false;
            _tryGrabAnimancerState = animatorCompo.PlayAnimationClip(tryGrabAnimationClip, () =>
            {
                if (!player.IsGrabbed)
                    _animationFinished = true;
            });
            _tryGrabAnimancerState.Events(animatorCompo).SetCallback(tryGrabEvent, GrabPlayer);
        }

        private void GrabPlayer()
        {
            Player player = GetPlayer();
            PlayerMovement movementCompo = player.GetCompo<PlayerMovement>();
            if (movementCompo.IsEvading)
            {
                // _animationFinished = true;
                return;
            }

            float distance = Vector3.Distance(player.transform.position, _boss.transform.position);
            if (distance > grabDistance)
            {
                // _animationFinished = true;
                return;
            }

            player.IsGrabbed = true;
            RuntimeManager.PlayOneShot(grabSound, _boss.transform.position);

            PlayerAnimator animatorCompo = player.GetCompo<PlayerAnimator>();
            animatorCompo.EnableRootMotion(true);
            _boss.GetCompo<AgentAnimator>(true).lockedTransitionAnimation = false;
            player.GetCompo<AgentAnimator>(true).lockedTransitionAnimation = false;
            player.ModelTrm.DOLookAt(_boss.transform.position, .2f, AxisConstraint.Y).OnComplete(() =>
            {
                AlignComponent alignComponent = player.GetComponent<AlignComponent>();
                alignComponent.targetAnim = grabAndAttackVictimAnimationClip;
                alignComponent.motionWarpingAsset = attackerMotionWarpingAsset;
                MotionWarping motionWarpingCompo = _boss.WarpingComponent;
                motionWarpingCompo.Interact(alignComponent);
                motionWarpingCompo.OnAnimationFinished = () =>
                {
                    animatorCompo.EnableRootMotion(false);
                    _animationFinished = true;
                    player.IsGrabbed = false;
                    _boss.GetCompo<AgentAnimator>(true).lockedTransitionAnimation = false;
                    player.GetCompo<AgentAnimator>(true).lockedTransitionAnimation = false;
                };
            });
        }

        public override bool IsSkillFinished()
        {
            return _animationFinished;
        }
    }
}