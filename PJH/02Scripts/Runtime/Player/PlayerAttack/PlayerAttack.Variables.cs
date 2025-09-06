using System;
using DG.Tweening;
using Main.Runtime.Core.Events;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PJH.Runtime.Players
{
    public partial class PlayerAttack
    {
        private PlayerCombatDataSO _playerCombatData;

        public PlayerCombatDataSO CurrentCombatData
        {
            get => _playerCombatData;
            private set { _playerCombatData = value; }
        }

        public event Action OnAttack;
        public event Action<CommandActionPieceSO> OnUseCommandActionPiece;
        public event Action OnEnterBattle;
        public event Action OnExitBattle;
        [field: SerializeField, ReadOnly] public bool IsInBattle { get; private set; }

        public bool IsAttacking { get; private set; }
        public int ComboCount { get; private set; }

        private GameEventChannelSO _cameraViewConfigEventChannel;
        [SerializeField] private float _timeToSwitchToIdleAfterCombat = 1;


        private Player _player;
        private Tween _cameraViewConfigTween;
        private CommandActionData _currentCommandActionData;
        private bool _isComboPossible;
        private int _maxComboCount;

        private int _prevComboCount;
    }
}