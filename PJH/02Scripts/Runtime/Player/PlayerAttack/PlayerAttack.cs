using DG.Tweening;
using Main.Core;
using Main.Runtime.Agents;
using Main.Runtime.Core;
using Main.Runtime.Core.Events;
using Main.Runtime.Equipments.Scripts;
using Main.Shared;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PJH.Runtime.Players
{
    public partial class PlayerAttack : SerializedMonoBehaviour, IAgentComponent, IAfterInitable
    {
        public void Initialize(Agent agent)
        {
            _cameraViewConfigEventChannel = AddressableManager.Load<GameEventChannelSO>("GameEventChannel");
            _player = agent as Player;
            _blockCompo = _player.GetCompo<PlayerBlock>();
            _movementCompo = _player.GetCompo<PlayerMovement>();
            _weaponManagerCompo = _player.GetCompo<AgentWeaponManager>();
            _enemyDetectionCompo = _player.GetCompo<PlayerEnemyDetection>();
            _playerAnimationTriggerCompo = _player.GetCompo<PlayerAnimationTrigger>();
            _warpStrikeCompo = _player.GetCompo<PlayerWarpStrike>();
            _isComboPossible = true;
        }

        public void AfterInitialize()
        {
            SubscribeEvents();
            Weapon beginWeapon = _player.GetCompo<AgentEquipmentSystem>().GetSocket(Define.ESocketType.LeftHand)
                .GetItem<Weapon>();
            // _combatDataList = _combatDatabase.database[beginWeapon.EquipmentData].combatDataList;
        }

        private void OnDestroy()
        {
            UnSubscribeEvents();
        }

        private void PrevAttackProcess()
        {
            OnEnterBattle?.Invoke();
            if (!IsAttacking)
            {
                if (_cameraViewConfigTokenSource is { IsCancellationRequested: false })
                {
                    _cameraViewConfigTokenSource.Cancel();
                    _cameraViewConfigTokenSource.Dispose();
                    _cameraViewConfigTokenSource = null;
                }

                var evt = GameEvents.CameraViewConfig;
                evt.isChangeConfig = true;
                _cameraViewConfigEventChannel.RaiseEvent(evt);
            }

            if (_player.IsLockOn && _enemyDetectionCompo.TryGetTargetEnemy(out Agent target))
            {
                Vector3 dir = (target.transform.position - _player.transform.position).normalized;
                dir.y = 0;
                Quaternion look = Quaternion.LookRotation(dir);
                _player.ModelTrm.DORotateQuaternion(look, .25f);
            }
            else
            {
                Vector3 input = _player.PlayerInput.Input;
                if (input.sqrMagnitude > 0)
                {
                    var right = Camera.main.transform.right;
                    right.y = 0;
                    var forward = Quaternion.AngleAxis(-90, Vector3.up) * right;
                    Vector3 dir = (input.x * right) + (input.z * forward);
                    dir.y = 0;
                    Quaternion look = Quaternion.LookRotation(dir);
                    _player.ModelTrm.DORotateQuaternion(look, .5f);
                }
            }

            IsAttacking = true;
            _isComboPossible = false;
            if (CurrentCombatData)
            {
                _playerAnimationTriggerCompo.OnDisableDamageCollider?.Invoke();
                CommandActionPieceSO commandActionPiece =
                    _currentCommandActionData.ExecuteCommandActionPieces[_prevComboCount];
                commandActionPiece.DeactivePassive();
            }
        }

        private bool CanAttack() => _isComboPossible && !_player.IsStunned && !_player.IsHitting &&
                                    _isComboPossible && !_movementCompo.IsEvading && !_blockCompo.IsBlocking &&
                                    !_warpStrikeCompo.Activating;
    }
}