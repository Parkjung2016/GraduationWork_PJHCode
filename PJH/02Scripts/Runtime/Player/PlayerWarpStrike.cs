using System;
using Animancer;
using DG.Tweening;
using Main.Runtime.Agents;
using Main.Runtime.Combat;
using Main.Runtime.Combat.Core;
using Main.Runtime.Core;
using Main.Runtime.Core.Events;
using Main.Runtime.Manager;
using PJH.Runtime.Core;
using PJH.Runtime.Core.PlayerCamera;
using PJH.Utility.Managers;
using UnityEngine;

namespace PJH.Runtime.Players
{
    public class WarpStrikeAttackInfo
    {
        public GetDamagedAnimationClipInfo getDamagedAnimationClipInfo;
        public TransitionAsset attackAnimation;
    }

    public class PlayerWarpStrike : MonoBehaviour, IAgentComponent, IAfterInitable
    {
        private event Action OnTriggeredWarpStrike;
        public event Action OnHitWarpStrikeTarget;
        public event Action<ITransition> OnWarpStrikeAttack;
        public bool Activating { get; private set; }
        [SerializeField] private LayerMask _whatIsWarpStrikeTarget;
        [SerializeField] private PoolTypeSO _playerMotionTrailPoolType;
        private PoolManagerSO _poolManager;
        private Player _player;
        private PlayerCamera _playerCamera;
        private GameEventChannelSO _uiEventChannel;
        private Agent _warpStrikeTarget;
        private WarpStrikeAttackInfo _currentWarpStrikeAttackInfo;

        private float _power;

        public void Initialize(Agent agent)
        {
            _poolManager = AddressableManager.Load<PoolManagerSO>("PoolManager");
            _uiEventChannel = AddressableManager.Load<GameEventChannelSO>("UIEventChannelSO");
            _player = agent as Player;
            _playerCamera = PlayerManager.Instance.PlayerCamera;
            enabled = false;
        }

        public void AfterInitialize()
        {
            _player.GetCompo<PlayerAnimationTrigger>().OnWarpTargetAndAttack += HandleWarpTargetAndAttack;
        }

        private void OnDestroy()
        {
            _player.PlayerInput.AttackEvent -= TriggerWarpStrike;
            _player.GetCompo<PlayerAnimationTrigger>().OnWarpTargetAndAttack -= HandleWarpTargetAndAttack;
        }

        private void TriggerWarpStrike()
        {
            if (!_warpStrikeTarget) return;
            Activating = true;
            OnTriggeredWarpStrike?.Invoke();
            _player.GetCompo<PlayerEnemyDetection>().SetForceTargetEnemy(_warpStrikeTarget);
            _player.GetCompo<PlayerMovement>().CC.enabled = false;
            DisableWarpStrike();
            _player.ModelTrm.DOKill();
            _player.ModelTrm.DOLookAt(_warpStrikeTarget.transform.position, .2f, AxisConstraint.Y).OnComplete(() =>
            {
                OnWarpStrikeAttack?.Invoke(_currentWarpStrikeAttackInfo.attackAnimation);
            });
        }

        private void HandleWarpTargetAndAttack()
        {
            (_poolManager.Pop(_playerMotionTrailPoolType) as MotionTrail)?.SnapshotMesh(_player.ModelRenderer);
            _player.EnableMeshRenderers(false);
            PlayerAnimator animatorCompo = _player.GetCompo<PlayerAnimator>();
            animatorCompo.Animancer.States[_currentWarpStrikeAttackInfo.attackAnimation].EffectiveSpeed = 0;
            Vector3 warpPoint = Vector3.Lerp(_player.transform.position, _warpStrikeTarget.transform.position, .95f);
            warpPoint.y = _player.transform.position.y;

            Tweener moveTweener = _player.transform.DOMove(warpPoint, .4f).SetEase(Ease.InOutQuint);
            moveTweener.OnComplete(() =>
            {
                _player.GetCompo<PlayerMovement>().CC.enabled = true;
                _player.EnableMeshRenderers(true);

                GetDamagedInfo info = new GetDamagedInfo()
                    .SetAttacker(_player)
                    .SetDamage(_power)
                    .SetGetDamagedAnimationClip(_currentWarpStrikeAttackInfo.getDamagedAnimationClipInfo)
                    .SetHitPoint(warpPoint)
                    .SetIncreaseMomentumGauge(0);
                OnHitWarpStrikeTarget?.Invoke();
                _warpStrikeTarget.HealthCompo.ApplyDamage(info);
                Activating = false;
                animatorCompo.Animancer.States[_currentWarpStrikeAttackInfo.attackAnimation]
                    .EffectiveSpeed = 1;
                _player.GetCompo<PlayerAnimationTrigger>().OnEndCombo();
            });
        }

        private void Update()
        {
            if (Activating) return;
            Agent target = _playerCamera.CameraTargetDetectionCompo.Target;
            if (target != null)
            {
                var evt = UIEvents.ShowWarpStrikeTargetUI;
                evt.isShowUI = true;
                _warpStrikeTarget = target;
                evt.target = _warpStrikeTarget;
                _uiEventChannel.RaiseEvent(evt);
            }
            else
            {
                var evt = UIEvents.ShowWarpStrikeTargetUI;
                if (evt.isShowUI)
                {
                    _warpStrikeTarget = null;
                    evt.isShowUI = false;
                    _uiEventChannel.RaiseEvent(evt);
                }
            }
        }

        private void OnDisable()
        {
            var evt = UIEvents.ShowWarpStrikeTargetUI;
            if (evt.isShowUI)
            {
                if (!Activating)
                    _warpStrikeTarget = null;
                evt.isShowUI = false;
                _uiEventChannel.RaiseEvent(evt);
            }
        }

        public void EnableWarpStrike(float power, WarpStrikeAttackInfo warpStrikeAttackInfo,
            Action triggeredWarpStrikeEvent)
        {
            _power = power;
            _currentWarpStrikeAttackInfo = warpStrikeAttackInfo;
            OnTriggeredWarpStrike = triggeredWarpStrikeEvent;
            enabled = true;
            _player.PlayerInput.AttackEvent += TriggerWarpStrike;
        }

        public void DisableWarpStrike()
        {
            enabled = false;
            OnTriggeredWarpStrike = null;
            _player.PlayerInput.AttackEvent -= TriggerWarpStrike;
        }
    }
}