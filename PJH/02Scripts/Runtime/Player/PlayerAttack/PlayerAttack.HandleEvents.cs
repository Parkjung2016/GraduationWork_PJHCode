using Main.Runtime.Agents;
using Main.Runtime.Core.Events;


[assembly: ZLinq.ZLinqDropInAttribute("PJH.Runtime.Players", ZLinq.DropInGenerateTypes.Everything)]

namespace PJH.Runtime.Players
{
    public partial class PlayerAttack
    {
        private void HandleEvasion()
        {
            if (IsAttacking)
            {
                _isComboPossible = true;
                IsAttacking = false;
                PlayerMovement movementCompo = _player.GetCompo<PlayerMovement>();
                movementCompo.CanMove = true;
                ExitBattleAfterDelay();
            }
            else if (ComboCount != 0)
            {
                HandleEndCombo();
            }
        }

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
            IsAttacking = false;
            movementCompo.CanMove = true;
            ComboCount = 0;
            ExitBattleAfterDelay();
            animationTriggerCompo.OnDisableDamageCollider?.Invoke();
            CommandActionPieceSO commandActionPiece =
                _currentCommandActionData.ExecuteCommandActionPieces[_prevComboCount];
            commandActionPiece.DeactivePassive();
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
            _prevComboCount = ComboCount;
            ComboCount = (ComboCount + 1) % _maxComboCount;
        }

        private void HandleApplyDamaged(float damage)
        {
            HandleEndCombo();
        }

        private void HandleUseCommandAction(CommandActionData commandActionData)
        {
            if (commandActionData == null) return;
            if (commandActionData.ExecuteCommandActionPieces.Count == 0) return;
            _currentCommandActionData = commandActionData;
            _prevComboCount = 0;
            ComboCount = 0;
            _currentCommandActionData.Init(_player);
        }

        private void HandleBlock(bool isBlocking)
        {
            if (isBlocking)
                HandleEndCombo();
        }
    }
}