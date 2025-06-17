using Animancer;
using Main.Runtime.Core.StatSystem;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PJH.Runtime.Players
{
    public partial class PlayerMovement
    {
        public event Action<float> OnMovement;
        public event Action<bool> OnRun;
        public event Action<ITransition> OnTurn;
        public event Action<ITransition, ITransition> OnEvasionWithAnimation;
        public event Action OnEvasion, OnEvasionWhileHitting, OnEvasionEndWhileHitting;
        public bool CanMove { get; set; } = true;
        public bool IsGrounded => CC.isGrounded;
        public CharacterController CC { get; private set; }
        public bool IsRunning { get; private set; }
        public bool IsEvading { get; private set; }
        public bool IsEvadingWhileHitting { get; private set; }
        public bool IsManualMove { get; private set; }
        public bool IsKnockBack { get; private set; }
        [field: SerializeField] public float DecreaseMomentumGaugeWhenEvading { get; private set; } = 20;

        public float Speed
        {
            get
            {
                float movementSpeed = IsRunning ? _player.RunSpeedStat.Value : _player.WalkSpeedStat.Value;
                return movementSpeed;
            }
        }
        [SerializeField] private float _gravity;
        [SerializeField] private float _rotationSpeed;
        [SerializeField] private float _acceleration = 10f;
        [SerializeField] private float _deceleration = 8f;
        [SerializeField] private float _evasionDelay = 1f;
        [SerializeField] private float _evasionWhileHittingDelay = 1f;

        [BoxGroup("Animancer Clips")] [FoldoutGroup("Animancer Clips/Evasion"), SerializeField]
        private Dictionary<string, ClipTransition> _evasionAnimations = new();


        [BoxGroup("Animancer Clips"), SerializeField]
        private ClipTransition _animationAfterEvasion;

        [BoxGroup("Animancer Clips"), SerializeField]
        private TransitionAsset _turnAnimation;

        private Player _player;
        private PlayerAnimator _animatorCompo;
        private PlayerEnemyDetection _enemyDetectionCompo;
        private PlayerWarpStrike _warpStrikeCompo;
        private PlayerAttack _attackCompo;
        private AnimatedFloat _rootMotionMultiplierCurve;
        private Vector3 _velocity;
        private Vector3 _desiredVelocity = Vector3.zero;
        private Vector3 _knockBackDir;
        private bool _canEvading;
        private float _yVelocity;
        private float _knockBackPower;
        private float _manualMoveSpeed;
        private float _currentEvasionDelayTime, _currentEvasionWhileHittingDelayTime;
    }
}