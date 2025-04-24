using System;
using System.Collections.Generic;
using Animancer;
using DG.Tweening;
using Main.Core;
using Main.Runtime.Agents;
using Main.Runtime.Combat.Core;
using Main.Runtime.Core;
using Main.Runtime.Core.Events;
using Main.Runtime.Manager;
using MoreMountains.Feedbacks;
using PJH.Runtime.Core;
using PJH.Runtime.Core.PlayerCamera;
using UnityEngine;
using Debug = Main.Core.Debug;

namespace PJH.Runtime.Players
{
    public class PlayerWarpStrike : MonoBehaviour, IAgentComponent, IAfterInitable
    {
        private event Action OnTriggeredWarpStrike;
        public event Action<ITransition> OnWarpStrikeAttack;
        public bool Activating { get; private set; }
        [SerializeField] private LayerMask _whatIsWarpStrikeTarget;
        [SerializeField] private List<TransitionAsset> _warpStrikeAnimations;
        [SerializeField] private PoolTypeSO _playerMotionTrailPoolType;
        [SerializeField] private PoolManagerSO _poolManager;
        private Player _player;
        private PlayerCamera _playerCamera;
        private GameEventChannelSO _uiEventChannel;
        private Agent _warpStrikeTarget;
        private RaycastHit[] _warpStrikeTargets;
        private MMF_Player _hitFeedback;
        private ITransition _currentWarpStrikeAnimation;

        private float _power;

        public void Initialize(Agent agent)
        {
            _hitFeedback = transform.Find("HitFeedback").GetComponent<MMF_Player>();
            _warpStrikeTargets = new RaycastHit[10];
            _uiEventChannel = AddressableManager.Load<GameEventChannelSO>("UIEventChannelSO");
            _player = agent as Player;
            _playerCamera = (PlayerManager.Instance.PlayerCamera as PlayerCamera);
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
                _currentWarpStrikeAnimation = _warpStrikeAnimations.GetRandomElement();
                OnWarpStrikeAttack?.Invoke(_currentWarpStrikeAnimation);
            });
        }

        private void HandleWarpTargetAndAttack()
        {
            (_poolManager.Pop(_playerMotionTrailPoolType) as MotionTrail)?.SnapshotMesh(_player.ModelRenderer);
            _player.EnableMeshRenderers(false);
            PlayerAnimator animatorCompo = _player.GetCompo<PlayerAnimator>();
            animatorCompo.Animancer.States[_currentWarpStrikeAnimation].EffectiveSpeed = 0;
            Vector3 dir = _warpStrikeTarget.HeadTrm.transform.position - transform.position;
            Ray ray = new Ray(transform.position, dir.normalized);
            int cnt = Physics.RaycastNonAlloc(ray, _warpStrikeTargets, dir.magnitude, _whatIsWarpStrikeTarget);
            if (cnt > 0)
            {
                for (int i = 0; i < cnt; i++)
                {
                    if (_warpStrikeTargets[i].transform == _warpStrikeTarget.transform)
                    {
                        RaycastHit hitInfo = _warpStrikeTargets[i];
                        Vector3 warpPoint = Vector3.Lerp(_player.transform.position, hitInfo.point, .95f);
                        warpPoint.y = _player.transform.position.y;

                        _player.transform.DOMove(warpPoint, .4f).SetEase(Ease.InOutQuint).OnComplete(() =>
                        {
                            _player.GetCompo<PlayerMovement>().CC.enabled = true;
                            _player.EnableMeshRenderers(true);

                            _hitFeedback.PlayFeedbacks();
                            GetDamagedInfo info = new()
                            {
                                attacker = _player,
                                damage = _power,
                                getDamagedAnimationClip = null,
                                hitPoint = warpPoint,
                                increaseMomentumGauge = 0,
                                isForceAttack = false,
                                isKnockDown = false,
                            };

                            _warpStrikeTarget.HealthCompo.ApplyDamage(info);
                            Activating = false;
                            animatorCompo.Animancer.States[_currentWarpStrikeAnimation].EffectiveSpeed = 1;
                            _player.GetCompo<PlayerAnimationTrigger>().OnEndCombo();
                        });
                        break;
                    }
                }
            }
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

        public void EnableWarpStrike(float power, Action triggeredWarpStrikeEvent)
        {
            _power = power;
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