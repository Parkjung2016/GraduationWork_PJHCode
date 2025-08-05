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
                if (_cameraViewConfigTween != null && _cameraViewConfigTween.IsActive())
                    _cameraViewConfigTween.Kill();

                var evt = GameEvents.CameraViewConfig;
                evt.isChangeConfig = true;
                _cameraViewConfigEventChannel.RaiseEvent(evt);
            }

            PlayerAnimationTrigger animationTriggerCompo = _player.GetCompo<PlayerAnimationTrigger>();
            PlayerEnemyDetection enemyDetectionCompo = _player.GetCompo<PlayerEnemyDetection>();
            if (_player.IsLockOn && enemyDetectionCompo.TryGetTargetEnemy(out Agent target))
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
                animationTriggerCompo.OnDisableDamageCollider?.Invoke();
                CommandActionPieceSO commandActionPiece =
                    _currentCommandActionData.ExecuteCommandActionPieces[_prevComboCount];
                commandActionPiece.DeActivePassive();
            }
        }

        private bool CanAttack()
        {
            PlayerBlock blockCompo = _player.GetCompo<PlayerBlock>();
            PlayerMovement movementCompo = _player.GetCompo<PlayerMovement>();
            PlayerWarpStrike warpStrikeCompo = _player.GetCompo<PlayerWarpStrike>();
            PlayerCounterAttack counterAttackCompo = _player.GetCompo<PlayerCounterAttack>();
            return !_player.IsGrabbed && _isComboPossible && !_player.IsStunned && !_player.IsHitting &&
                   !movementCompo.IsEvading &&
                   !blockCompo.IsBlocking &&
                   !warpStrikeCompo.Activating && !counterAttackCompo.IsCounterAttacking;
        }


        private void ExitBattleAfterDelay()
        {
            if (_cameraViewConfigTween != null && _cameraViewConfigTween.IsActive()) _cameraViewConfigTween.Kill();

            _cameraViewConfigTween = _player.DelayCallBack(_timeToSwitchToIdleAfterCombat, () =>
            {
                if (!IsInBattle)
                {
                    var evt = GameEvents.CameraViewConfig;
                    evt.isChangeConfig = false;
                    _cameraViewConfigEventChannel.RaiseEvent(evt);
                    OnExitBattle?.Invoke();
                }
            });
        }
    }
}