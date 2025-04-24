using System;
using Animancer;
using Cysharp.Threading.Tasks;
using DG.Tweening;
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
        public event Action<ITransition> OnFullMount;
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
            await UniTask.WaitForSeconds(.2f);
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
            if (_player.IsStunned || _player.IsHitting  ||
                _movementCompo.IsEvading) return;
            if (_fullMountTargetDetection.GetFullMountTarget(out Agent target))
            {
                IsFullMounting = true;
                _fullMountTarget = target;
                _player.transform.DOMove(target.transform.position, .2f);
                Vector3 dir = (_player.transform.position - target.transform.position).normalized;
                dir.y = 0;
                Quaternion look = Quaternion.LookRotation(dir);
                _player.ModelTrm.DOLookAt(target.transform.position, .2f, AxisConstraint.Y);
                target.transform.DORotateQuaternion(look, .2f);
                FullMountAnimationDataSO fullMountAnimation = _fullMountAnimationDatabase.fullMountAnimationDats[0];
                OnFullMount?.Invoke(fullMountAnimation.fullMountAttackAnimation);
                target.GetCompo<AgentFullMountable>().FullMounted(fullMountAnimation.fullMountedAnimation);
            }
        }
    }
}