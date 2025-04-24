using System;
using System.Collections.Generic;
using Main.Runtime.Agents;
using Main.Runtime.Core;
using UnityEngine;

namespace PJH.Runtime.Players
{
    public class PlayerCommandActionManager : MonoBehaviour, IAgentComponent, IAfterInitable
    {
        public bool IsUsingCommandAction { get; private set; }
        public event Action<CommandActionData> OnUseCommandAction;
        [SerializeField] private List<CommandActionData> _commandActions = new();
        [SerializeField] private DefaultCommandActionDataSO _defaultCommandAction;

        private CommandActionData _currentUsingCommandActionData;
        private Player _player;
        private PlayerAttack _attackCompo;
        private int _currentUsingCommandActionDataIndex;

        public void Initialize(Agent agent)
        {
            _player = agent as Player;
        }

        public void AfterInitialize()
        {
            _player.PlayerInput.CommandActionEvent += HandleCommandAction;
            _attackCompo = _player.GetCompo<PlayerAttack>();

            _currentUsingCommandActionDataIndex = 0;
            ChangeCommandAction(0, _defaultCommandAction.commandActionData);
        }

        private void OnDestroy()
        {
            _player.PlayerInput.CommandActionEvent -= HandleCommandAction;
        }

        private void HandleCommandAction(int index)
        {
            if (_attackCompo.IsAttacking) return;
            if (_commandActions.Count < index + 1) return;
            if (_commandActions[index] == null) return;

            IsUsingCommandAction = true;
            _currentUsingCommandActionDataIndex = index;
            OnUseCommandAction?.Invoke(_commandActions[index]);
        }

        public void ChangeCommandAction(int index, CommandActionData commandActionData)
        {
            CommandActionData copyCommandActionData = new CommandActionData();
            foreach (var commandActionPiece in commandActionData.ExecuteCommandActionPieces)
            {
                CommandActionPieceSO copyCommandActionPiece = Instantiate(commandActionPiece);
                copyCommandActionData.ExecuteCommandActionPieces.Add(copyCommandActionPiece);
            }

            _commandActions[index] = copyCommandActionData;

            if (_currentUsingCommandActionDataIndex == index)
            {
                OnUseCommandAction?.Invoke(copyCommandActionData);
            }
        }
    }
}