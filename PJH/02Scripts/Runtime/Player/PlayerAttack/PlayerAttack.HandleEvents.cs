using Main.Runtime.Agents;
using Main.Runtime.Core.Events;
using PJH.Runtime.PlayerPassive;

[assembly: ZLinq.ZLinqDropInAttribute("PJH.Runtime.Players", ZLinq.DropInGenerateTypes.Everything)]

namespace PJH.Runtime.Players
{
    public partial class PlayerAttack
    {
        private void HandleChangedTargetEnemy(Agent prevTarget, Agent currentTarget)
        {
            IsInBattle = currentTarget != null;
            if (!currentTarget)
            {
                var evt = GameEvents.CameraViewConfig;
                evt.isChangeConfig = false;
                _cameraViewConfigEventChannel.RaiseEvent(evt);
                OnExitBattle?.Invoke();
            }
            else
            {
                if (IsInBattle)
                {
                    var evt = GameEvents.CameraViewConfig;
                    if (!evt.isChangeConfig)
                    {
                        evt.isChangeConfig = true;
                        _cameraViewConfigEventChannel.RaiseEvent(evt);
                    }

                    OnEnterBattle?.Invoke();
                }
            }
        }

        private void HandleComboPossible()
        {
            PlayerCombatDataSO lastCombatData = _currentCommandActionData.ExecuteCommandActionPieces.Last().combatData;
            if (CurrentCombatData == lastCombatData) return;
            _isComboPossible = true;
        }

        private void HandleEndCombo()
        {
            PlayerMovement movementCompo = _player.GetCompo<PlayerMovement>();
            PlayerAnimationTrigger animationTriggerCompo = _player.GetCompo<PlayerAnimationTrigger>();
            _isComboPossible = true;
            movementCompo.CanMove = true;
            ComboCount = 0;
            ExitBattleAfterDelay();
            animationTriggerCompo.OnDisableDamageCollider?.Invoke();
            if (IsAttacking)
            {
                CommandActionPieceSO commandActionPiece =
                    _currentCommandActionData.ExecuteCommandActionPieces[_prevComboCount];
                commandActionPiece.DeActivePassive();
            }

            IsAttacking = false;
        }

        private void HandleAttack()
        {
            if (!CanAttack()) return;
            _maxComboCount = _currentCommandActionData.ExecuteCommandActionPieces.Count;
            PrevAttackProcess();
            CommandActionPieceSO commandActionPiece = _currentCommandActionData.ExecuteCommandActionPieces[ComboCount];
            CurrentCombatData = commandActionPiece.combatData;
            commandActionPiece.ActivePassive();
            AgentWeaponManager weaponManagerCompo = _player.GetCompo<AgentWeaponManager>();
            weaponManagerCompo.CurrentCombatData = CurrentCombatData;
            OnAttack?.Invoke();
            OnUseCommandActionPiece?.Invoke(commandActionPiece);
            _prevComboCount = ComboCount;
            ComboCount = (ComboCount + 1) % _maxComboCount;
        }

        private void HandleApplyDamaged(float damage)
        {
            HandleEndCombo();
        }

        private void HandleUseCommandAction(CommandActionData commandActionData)
        {
            if (_currentCommandActionData != null)
            {
                for (int i = 0; i < _currentCommandActionData.ExecuteCommandActionPieces.Count; i++)
                {
                    CommandActionPieceSO commandActionPiece = _currentCommandActionData.ExecuteCommandActionPieces[i];
                    if (commandActionPiece == null) continue;
                    commandActionPiece.EndAllBuffPassive();
                }

                _currentCommandActionData.UnEquip();
            }

            _currentCommandActionData = commandActionData;
            _currentCommandActionData.Equip(_player);
            _prevComboCount = 0;
            ComboCount = 0;
        }

        private void HandleBlock(bool isBlocking)
        {
            if (isBlocking)
                HandleEndCombo();
        }
    }
}