using System;
using Main.Runtime.Animators;
using Main.Runtime.Core.StatSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PJH.Runtime.Players
{
    public partial class PlayerAnimator
    {
        public bool IsRootMotion { get; private set; }
        public bool IsEnabledInputWhileRootMotion { get; private set; }

        public event Action<Vector3, Quaternion> AnimatorMoveEvent;

        [BoxGroup("Animator Parameters"), Required] [SerializeField]
        private AnimParamSO
            _velocityParam,
            _horizontalParam,
            _verticalParam,
            _isGroundedParam,
            _isMovingParam,
            _isInBattleParam,
            _fadeOffLeaningParam,
            _isBlockingParam,
            _isStunnedParam;

        [SerializeField] private StatSO _attackSpeedStat;
        private PlayerAnimationTrigger _playerAnimationTriggerCompo;
        private PlayerAttack _attackCompo;
        private PlayerMovement _movementCompo;
        private PlayerBlock _blockCompo;
        private Player _player;
        private bool _isPlayingTimeline;
    }
}