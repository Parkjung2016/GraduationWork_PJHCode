using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Main.Runtime.Agents;
using Main.Runtime.Combat.Core;
using Main.Runtime.Core.Events;
using Main.Shared;
using PJH.Utility.Managers;
using UnityEngine;

namespace PJH.Runtime.Players
{
    public partial class Player : Agent, IPlayer, IKnockBackable
    {
        protected override void Awake()
        {
            ModelRenderer = transform.Find("Model").GetComponentInChildren<SkinnedMeshRenderer>();
            PlayerInput = AddressableManager.Load<PlayerInputSO>("PlayerInputSO");
            for (int i = 0; i < _playerUIReferences.Length; i++)
            {
                AddressableManager.Instantiate(_playerUIReferences[i].name);
            }

            base.Awake();

            _gameEventChannel = AddressableManager.Load<GameEventChannelSO>("GameEventChannel");
            ModelTrm = transform.Find("Model");
            _meshRenderers = ModelTrm.GetComponentsInChildren<Renderer>();
            _attackCompo = GetCompo<PlayerAttack>();
            _stunDurationStat = GetCompo<PlayerStat>().GetStat(_stunDurationStat);
            SubscribeEvents();
        }

        protected override void Start()
        {
            base.Start();
            IsLockOn = true;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (_stunTokenSource is { IsCancellationRequested: false })
            {
                _stunTokenSource.Cancel();
                _stunTokenSource.Dispose();
            }

            if (_knockBackTokenSource is { IsCancellationRequested: false })
            {
                _knockBackTokenSource.Cancel();
                _knockBackTokenSource.Dispose();
            }

            UnSubscribeEvents();
        }

        public void EnableMeshRenderers(bool enabled)
        {
            for (byte i = 0; i < _meshRenderers.Length; i++)
            {
                _meshRenderers[i].enabled = enabled;
            }
        }

        private void HandleMomentumGaugeFull()
        {
            if (IsStunned) return;
            Stun();
        }

        private async void Stun()
        {
            try
            {
                if (_stunTokenSource is { IsCancellationRequested: false })
                {
                    _stunTokenSource.Cancel();
                    _stunTokenSource.Dispose();
                }

                _stunTokenSource = new CancellationTokenSource();

                HandleEndHitAnimation();
                IsStunned = true;
                OnStartStun?.Invoke();
                await UniTask.WaitForSeconds(_stunDurationStat.Value, cancellationToken: _stunTokenSource.Token);
                IsStunned = false;
                OnEndStun?.Invoke();
            }
            catch (Exception e)
            {
            }
        }

        public async void ApplySilencePassive(float duration)
        {
            try
            {
                if (_applySilencePassiveTokenSource is { IsCancellationRequested: false })
                {
                    _applySilencePassiveTokenSource.Cancel();
                    _applySilencePassiveTokenSource.Dispose();
                }

                Debug.Log("4");
                _applySilencePassiveTokenSource = new();
                _applySilencePassiveTokenSource.RegisterRaiseCancelOnDestroy(gameObject);
                CanApplyPassive = false;
                await UniTask.WaitForSeconds(duration, cancellationToken: _applySilencePassiveTokenSource.Token);
                CanApplyPassive = true;
            }
            catch (Exception e)
            {
            }
        }

        public async void KnockBack(Vector3 knockBackDir, float knockBackPower, float knockBackDuration)
        {
            try
            {
                if (_knockBackTokenSource is { IsCancellationRequested: false })
                {
                    _knockBackTokenSource.Cancel();
                    _knockBackTokenSource.Dispose();
                }

                _knockBackTokenSource = new();
                OnStartKnockBack?.Invoke(knockBackDir, knockBackPower);
                await UniTask.WaitForSeconds(knockBackDuration, cancellationToken: _knockBackTokenSource.Token);
                OnEndKnockBack?.Invoke();
            }
            catch (Exception e)
            {
            }
        }
    }
}