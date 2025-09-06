using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Kinemation.MotionWarping.Runtime.Examples;
using Main.Runtime.Agents;
using Main.Runtime.Core;
using Main.Runtime.Core.StatSystem;
using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PJH.Runtime.Players
{
    public class PlayerFullMount : MonoBehaviour, IAgentComponent, IAfterInitable
    {
        public event Action OnFullMount;
        public event Action OnHitFullMountTarget;
        public bool IsFullMounting { get; private set; }
        [SerializeField] private StatSO _powerStat;
        [SerializeField] private MMF_Player _hitFeedback;
        [SerializeField, InlineEditor] private PlayerFullMountAnimationDatabaseSO _fullMountAnimationDatabase;
        private Player _player;
        private PlayerFullMountTargetDetection _fullMountTargetDetection;
        private PlayerMovement _movementCompo;
        private Agent _fullMountTarget;

        public void Initialize(Agent agent)
        {
            _player = agent as Player;
            _fullMountTargetDetection = _player.GetCompo<PlayerFullMountTargetDetection>();
            _movementCompo = _player.GetCompo<PlayerMovement>();
        }

        public void AfterInitialize()
        {
            _powerStat = _player.GetCompo<AgentStat>(true).GetStat(_powerStat);
            _player.PlayerInput.FullMountEvent += HandleFullMount;
            PlayerAnimationTrigger animationTriggerCompo = _player.GetCompo<PlayerAnimationTrigger>();
            animationTriggerCompo.OnHitFullMountTarget += HandleHitFullMountTarget;
            animationTriggerCompo.OnEndFullMount += HandleEndFullMount;
        }


        private void OnDestroy()
        {
            _player.PlayerInput.FullMountEvent -= HandleFullMount;
            PlayerAnimationTrigger animationTriggerCompo = _player.GetCompo<PlayerAnimationTrigger>();
            animationTriggerCompo.OnHitFullMountTarget -= HandleHitFullMountTarget;
            animationTriggerCompo.OnEndFullMount -= HandleEndFullMount;
        }

        private async void HandleEndFullMount()
        {
            await UniTask.WaitForSeconds(.2f, cancellationToken: gameObject.GetCancellationTokenOnDestroy());
            IsFullMounting = false;
        }


        private void HandleHitFullMountTarget()
        {
            if (!_fullMountTarget) return;
            _hitFeedback.PlayFeedbacks();
            OnHitFullMountTarget?.Invoke();

            float power = _powerStat.Value;
            _fullMountTarget.HealthCompo.ApplyOnlyDamage(power);
        }

        private void HandleFullMount()
        {
            if (_player.IsStunned || _player.IsHitting ||
                _movementCompo.IsEvading) return;
            if (_fullMountTargetDetection.GetFullMountTarget(out Agent target))
            {
                IsFullMounting = true;
                _fullMountTarget = target;
                _player.transform.DOMove(target.transform.position, .2f);
                Vector3 dir = (_player.transform.position - target.transform.position).normalized;
                dir.y = 0;
                Quaternion look = Quaternion.LookRotation(dir);
                FullMountAnimationDataSO fullMountAnimation = _fullMountAnimationDatabase.fullMountAnimationDats[0];
                AlignComponent alignComponent = target.GetComponent<AlignComponent>();
                alignComponent.targetAnim = fullMountAnimation.fullMountedAnimation;
                alignComponent.motionWarpingAsset = fullMountAnimation.fullMountMotionWarping;

                Sequence seq = DOTween.Sequence();
                seq.Append(_player.ModelTrm.DOLookAt(target.transform.position, .2f, AxisConstraint.Y));
                seq.Join(target.transform.DORotateQuaternion(look, .2f));
                seq.OnComplete(() =>
                {
                    target.GetCompo<AgentFullMountable>().FullMounted();
                    _player.WarpingComponent.Interact(alignComponent);
                    _player.WarpingComponent.OnAnimationFinished += HandleFullMountEnd;
                    OnFullMount?.Invoke();
                });
            }
        }

        private void HandleFullMountEnd()
        {
            _player.WarpingComponent.OnAnimationFinished -= HandleFullMountEnd;
            _player.GetCompo<PlayerAnimationTrigger>().OnEndFullMount?.Invoke();
            _fullMountTarget.GetCompo<AgentAnimator>(true).PlayGetUpAnimation();
        }
    }
}