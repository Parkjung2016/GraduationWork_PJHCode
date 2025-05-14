using System;
using System.Threading;
using Main.Runtime.Core.Events;
using Main.Runtime.Core.StatSystem;
using MoreMountains.Feedbacks;
using RayFire;
using Unity.Cinemachine;
using UnityEngine;

namespace PJH.Runtime.Players
{
    public partial class Player
    {
        public event Action<Vector3, float> OnStartKnockBack;
        public event Action OnEndKnockBack;
        public event Action OnStartStun;
        public event Action OnEndStun;
        public event Action<bool> OnLockOn;
        public bool IsStunned { get; private set; }
        public bool IsLockOn { get; private set; } = true;
        public bool IsAvoidingAttack { get; private set; }
        private Renderer[] _meshRenderers;

        public SkinnedMeshRenderer ModelRenderer { get; private set; }
        [field: SerializeField] public PlayerInputSO PlayerInput { get; private set; }
        [SerializeField] private StatSO _stunDurationStat;
        [SerializeField] private MMF_Player _avoidingAttackFeedback;
        private GameEventChannelSO _gameEventChannel;
        private PlayerAttack _attackCompo;

        private CancellationTokenSource _knockBackTokenSource;
        private CancellationTokenSource _stunTokenSource;
    }
}