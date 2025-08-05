using System;
using System.Threading;
using Main.Runtime.Core.Events;
using Main.Runtime.Core.StatSystem;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace PJH.Runtime.Players
{
    public partial class Player
    {
        public event Action<Vector3, float> OnStartKnockBack;
        public event Action OnEndKnockBack;
        public event Action OnStartStun;
        public event Action OnEndStun;
        public event Action OnGrabbed;
        public event Action OnEndGrabbed;
        public event Action<bool> OnLockOn;
        public bool IsStunned { get; private set; }

        public bool IsGrabbed
        {
            get => _isGrabbed;
            set
            {
                _isGrabbed = value;
                if (_isGrabbed)
                {
                    OnGrabbed?.Invoke();
                }
                else
                {
                    OnEndGrabbed?.Invoke();
                }
            }
        }

        public bool IsLockOn
        {
            get => _isLockOn;
            private set
            {
                _isLockOn = value;
                OnLockOn?.Invoke(IsLockOn);
                var evt = GameEvents.LockOn;
                evt.isLockOn = IsLockOn;
                _gameEventChannel.RaiseEvent(evt);
            }
        }

        public bool IsAvoidingAttack { get; private set; }
        public SkinnedMeshRenderer ModelRenderer { get; private set; }
        public PlayerInputSO PlayerInput { get; private set; }
        public event Action<bool> OnChangedCanApplyPassive;

        public bool CanApplyPassive
        {
            get => _canApplyPassive;
            set
            {
                _canApplyPassive = value;
                OnChangedCanApplyPassive?.Invoke(value);
            }
        }

        [SerializeField] private StatSO _stunDurationStat;
        [SerializeField] private MMF_Player _avoidingAttackFeedback;
        [SerializeField] private GameObject[] _playerUIReferences;
        private GameEventChannelSO _gameEventChannel;
        private PlayerAttack _attackCompo;

        private CancellationTokenSource _knockBackTokenSource;
        private CancellationTokenSource _stunTokenSource;
        private CancellationTokenSource _applySilencePassiveTokenSource;

        private Renderer[] _meshRenderers;
        private bool _isGrabbed;
        private bool _isLockOn;
        [SerializeField] private bool _canApplyPassive;
    }
}