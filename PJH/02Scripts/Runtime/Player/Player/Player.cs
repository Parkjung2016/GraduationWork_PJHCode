using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Main.Core;
using Main.Runtime.Agents;
using Main.Runtime.Combat.Core;
using Main.Runtime.Core.Events;
using Main.Shared;
using UnityEngine;

namespace PJH.Runtime.Players
{
    public partial class Player : Agent, IPlayer, IKnockBackable
    {
        protected override void Awake()
        {
            base.Awake();
            _gameEventChannel = AddressableManager.Load<GameEventChannelSO>("GameEventChannel");
            base.ModelTrm = transform.Find("Model");
            _meshRenderers = ModelTrm.GetComponentsInChildren<Renderer>();
            ModelRenderer = _meshRenderers[0] as SkinnedMeshRenderer;

            _attackCompo = GetCompo<PlayerAttack>();
            _stunDurationStat = GetCompo<PlayerStat>().GetStat(_stunDurationStat);
            SubscribeEvents();
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
            for (int i = 0; i < _meshRenderers.Length; i++)
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

                try
                {
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