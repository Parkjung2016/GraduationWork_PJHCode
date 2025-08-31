using System;
using Animancer;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Main.Core;
using Main.Runtime.Agents;
using Main.Runtime.Core;
using Main.Runtime.Core.Events;
using PJH.Runtime.Core;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PJH.Runtime.Players
{
    public partial class PlayerMovement : SerializedMonoBehaviour, IAgentComponent, IAfterInitable
    {
        public async void Initialize(Agent agent)
        {
            try
            {
                _uiEventChannel = AddressableManager.Load<GameEventChannelSO>("UIEventChannelSO");
                _player = agent as Player;
                _animatorCompo = _player.GetCompo<PlayerAnimator>();
                _warpStrikeCompo = _player.GetCompo<PlayerWarpStrike>();
                _enemyDetectionCompo = _player.GetCompo<PlayerEnemyDetection>();
                _attackCompo = _player.GetCompo<PlayerAttack>();
                CC = _player.GetComponent<CharacterController>();
                await UniTask.WaitUntil(() => _animatorCompo.Animancer);
                _rootMotionMultiplierCurve = new AnimatedFloat(_animatorCompo.Animancer, "RootMotionMultiplier");
            }
            catch (Exception e)
            {
            }
        }

        public void AfterInitialize()
        {
            SubscribeEvents();
        }

        private void OnDestroy()
        {
            UnSubscribeEvents();
        }

        private void EndManualMove()
        {
            if (!IsManualMove) return;
            IsManualMove = false;
        }

        private void HandleAttack()
        {
            _isRotatingTargetWhileAttack = true;
            if (!_attackCompo.CurrentCombatData.isManualMove) return;
            _manualMoveSpeed = _attackCompo.CurrentCombatData.manualMoveSpeed;
            IsManualMove = true;
        }

        private void Evasion()
        {
            PlayerCounterAttack counterAttackComp = _player.GetCompo<PlayerCounterAttack>();
            if (counterAttackComp.IsCounterAttacking) return;
            if ((_canEvading && _currentEvasionDelayTime + _evasionDelay > Time.time) || _player.IsStunned ||
                IsKnockBack ||
                IsEvading || _player.IsGrabbed) return;

            if (_player.IsHitting)
            {
                if (_isCooldownEvasionWhileHitting) return;
                _currentEvasionWhileHittingDelayTime = _evasionWhileHittingDelay;
                _isCooldownEvasionWhileHitting = true;
                IsEvadingWhileHitting = true;
                var evt = UIEvents.ShowEvasionWhileHittingInfUI;
                evt.player = _player;
                _uiEventChannel.RaiseEvent(evt);
                OnEvasionWhileHitting?.Invoke();
            }

            IsEvading = true;
            _canEvading = false;
            Vector3 evasionDir = _player.PlayerInput.Input;
            float yawCamera = Camera.main.transform.eulerAngles.y;
            if (evasionDir == Vector3.zero)
                evasionDir = Vector3.back;
            Quaternion targetRot = Quaternion.Euler(0, yawCamera, 0);
            _player.ModelTrm.DORotateQuaternion(targetRot, .5f).SetLink(_player.ModelTrm.gameObject);


            ClipTransition evasionClip = null;
            string direction = MUtil.GetClosestDirection(evasionDir);
            evasionClip = _evasionAnimations[direction];
            OnEvasionWithAnimation?.Invoke(evasionClip, _animationAfterEvasion);
            OnEvasion?.Invoke();
        }

        private void FixedUpdate()
        {
            CheckCooldownEvasionWhileHitting();
            if (!CC || _player.WarpingComponent.IsActive() || _player.IsStunned) return;
            ApplyRotation();
            CalculateMove();
            ApplyGravity();
            CharacterMove();
            CheckTurn();
        }

        private void CheckCooldownEvasionWhileHitting()
        {
            if (!_isCooldownEvasionWhileHitting) return;
            _currentEvasionWhileHittingDelayTime -= Time.deltaTime;
            OnUpdateCooldownEvasionWhileHitting?.Invoke(_currentEvasionWhileHittingDelayTime,
                _evasionWhileHittingDelay);
            if (_currentEvasionWhileHittingDelayTime <= 0)
            {
                _isCooldownEvasionWhileHitting = false;
            }
        }

        private void CheckTurn()
        {
            if (IsManualMove || IsKnockBack || _animatorCompo.IsRootMotion || !IsRunning) return;
            Vector3 input = _player.PlayerInput.Input;
            if (input.sqrMagnitude > 0 && CC.velocity.sqrMagnitude > 1.5f)
            {
                Vector3 camForward = Camera.main.transform.forward;
                input = Quaternion.LookRotation(new Vector3(camForward.x, 0, camForward.z)) * input;
                float turnAngle = Mathf.Abs(Vector3.SignedAngle(_player.ModelTrm.forward, input, Vector3.up));
                if (turnAngle >= 130)
                {
                    OnTurn?.Invoke(_turnAnimation);
                }
            }
        }

        private void CalculateMove()
        {
            if (IsManualMove || IsKnockBack ||
                (_animatorCompo.IsRootMotion && !_animatorCompo.IsEnabledInputWhileRootMotion)) return;
            if (!CanMove)
            {
                _desiredVelocity = Vector3.zero;
                return;
            }

            Transform cameraTrm = Camera.main.transform;
            var right = cameraTrm.right;
            right.y = 0;
            var forward = Quaternion.AngleAxis(-90, Vector3.up) * right;
            Vector3 input = _player.PlayerInput.Input;
            _desiredVelocity = (input.x * right) + (input.z * forward);
            _desiredVelocity *= Speed;
        }

        private void ApplyGravity()
        {
            if (IsGrounded && _yVelocity < 0)
                _yVelocity = -.02f;
            else
                _yVelocity += _gravity * Time.deltaTime;

            _velocity.y = _yVelocity;
        }

        private void ApplyRotation()
        {
            if (IsManualMove ||
                IsKnockBack || _warpStrikeCompo.Activating) return;
            Quaternion targetRot = Quaternion.identity;

            if ((_animatorCompo.IsRootMotion && !_isRotatingTargetWhileAttack &&
                 !_animatorCompo.IsEnabledInputWhileRootMotion))
            {
                PlayerBlock blockCompo = _player.GetCompo<PlayerBlock>();
                if (blockCompo.IsBlocking && _enemyDetectionCompo.TryGetTargetEnemyNoInput(out Agent target))
                {
                    Vector3 dir = (target.transform.position - transform.position).normalized;
                    dir.y = 0;
                    targetRot = Quaternion.LookRotation(dir);
                }
                else
                    return;
            }

            Vector3 input = _player.PlayerInput.Input;
            if (input == Vector3.zero && !_attackCompo.IsInBattle) return;
            Transform cameraTrm = Camera.main.transform;

            if (!_animatorCompo.IsRootMotion && input != Vector3.zero && (IsRunning))
            {
                var right = cameraTrm.right;
                right.y = 0;
                var forward = Quaternion.AngleAxis(-90, Vector3.up) * right;
                Vector3 dir = (input.x * right) + (input.z * forward);
                dir.y = 0;
                targetRot = Quaternion.LookRotation(dir);
            }
            else if (_player.IsLockOn && _attackCompo.IsInBattle &&
                     _enemyDetectionCompo.TryGetTargetEnemyNoInput(out Agent target))
            {
                Vector3 dir = (target.transform.position - transform.position).normalized;
                dir.y = 0;
                targetRot = Quaternion.LookRotation(dir);
            }
            else if (input != Vector3.zero || (!_player.IsLockOn && _attackCompo.IsInBattle))
            {
                float y = cameraTrm.eulerAngles.y;
                targetRot = Quaternion.Euler(0, y, 0);
            }

            Transform modelTrm = _player.ModelTrm;
            modelTrm.rotation = Quaternion.Slerp(modelTrm.rotation, targetRot, _rotationSpeed * Time.deltaTime);
        }

        private void CharacterMove()
        {
            if ((!IsKnockBack && _animatorCompo.IsRootMotion &&
                 !_animatorCompo.IsEnabledInputWhileRootMotion) ||
                !CanMove || !CC.enabled) return;
            if (IsManualMove)
            {
                _velocity = _player.ModelTrm.forward *
                            (_manualMoveSpeed * _rootMotionMultiplierCurve.Value);
            }
            else if (!IsKnockBack)
            {
                _velocity = Vector3.MoveTowards(_velocity, _desiredVelocity,
                    (_desiredVelocity.magnitude > _velocity.magnitude ? _acceleration : _deceleration) *
                    Time.deltaTime);
            }
            else
            {
                _velocity = _knockBackDir * _knockBackPower;
            }

            Vector3 movement = _velocity * Time.deltaTime;
            movement.y = _yVelocity;
            CC.Move(movement);
            OnMovement?.Invoke(CC.velocity.sqrMagnitude);
        }
    }
}