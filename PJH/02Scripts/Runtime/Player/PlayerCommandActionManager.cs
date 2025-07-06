using System;
using System.Collections.Generic;
using BIS.Data;
using Main.Runtime.Agents;
using Main.Runtime.Core;
using Main.Core;
using UnityEngine;
using BIS.Shared.Interface;

namespace PJH.Runtime.Players
{
    public class PlayerCommandActionManager : MonoBehaviour, IAgentComponent, IAfterInitable
    {
        public bool IsUsingCommandAction { get; private set; }
        public event Action<CommandActionData> OnUseCommandAction;
        public event Action<int> OnUseCommandActionIndex;

        public IReadOnlyList<CommandActionData> CommandActions => _commandActions;


        [SerializeField] private List<CommandActionData> _commandActions = new();

        private Player _player;
        private int _currentUsingCommandActionDataIndex;
        private CurrentEquipComboSO _currentEquipComboSO;

        public void Initialize(Agent agent)
        {
            _player = agent as Player;
        }

        private void Start()
        {
            _currentEquipComboSO = AddressableManager.Load<CurrentEquipComboSO>("CurrentEquipComboSO");
            for (short i = 0; i < 3; ++i)
            {
                CommandActionData ActionData = new CommandActionData();
                for (short j = 0; j < 3; ++j)
                {
                    if (_currentEquipComboSO.CurrentEquipCommandSOs[i].CommandActionPieceSOs[j] == null)
                        break;
                    ActionData.TryAddCommandActionPiece(_currentEquipComboSO.CurrentEquipCommandSOs[i].CommandActionPieceSOs[j]);
                }

                ChangeCommandAction(i, ActionData);
            }
        }

        public void AfterInitialize()
        {
            _player.PlayerInput.CommandActionEvent += HandleCommandAction;
            _currentUsingCommandActionDataIndex = 0;
        }

        private void OnDestroy()
        {
            _player.PlayerInput.CommandActionEvent -= HandleCommandAction;
        }

        private void Update()
        {
            _commandActions.ForEach(commandAction =>
            {
                commandAction.ExecuteCommandActionPieces.ForEach(piece => piece.UpdatePassive());
            });
        }

        private void HandleCommandAction(int index)
        {
            if (_currentUsingCommandActionDataIndex == index) return;
            PlayerAttack attackCompo = _player.GetCompo<PlayerAttack>();
            if (attackCompo.IsAttacking) return;
            if (_commandActions.Count < index + 1) return;
            CommandActionData commandActionData = _commandActions[index];
            if (commandActionData == null) return;

            IsUsingCommandAction = true;
            _currentUsingCommandActionDataIndex = index;

            if (commandActionData.ExecuteCommandActionPieces.Count > 0)
            {
                OnUseCommandAction?.Invoke(commandActionData);
                OnUseCommandActionIndex?.Invoke(index);
            }
        }

        public void ChangeCommandAction(int index, CommandActionData commandActionData)
        {
            CommandActionData copyCommandActionData = new CommandActionData();
            foreach (var commandActionPiece in commandActionData.ExecuteCommandActionPieces)
            {
                CommandActionPieceSO copyCommandActionPiece = commandActionPiece;
                if (!commandActionPiece.name.Contains("(Clone)"))
                    copyCommandActionPiece = Instantiate(commandActionPiece);
                copyCommandActionData.ExecuteCommandActionPieces.Add(copyCommandActionPiece);
            }

            for (int i = 0; i < _commandActions[index].ExecuteCommandActionPieces.Count; i++)
            {
                _commandActions[index].ExecuteCommandActionPieces[i].UnEquipPiece();
            }

            _commandActions[index] = copyCommandActionData;

            if (copyCommandActionData.ExecuteCommandActionPieces.Count > 0 &&
                _currentUsingCommandActionDataIndex == index)
            {
                OnUseCommandAction?.Invoke(copyCommandActionData);
                OnUseCommandActionIndex?.Invoke(index);
            }
        }

    }
}