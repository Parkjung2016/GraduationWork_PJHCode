using System;
using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using Main.Runtime.Core;
using Main.Runtime.Core.StatSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Main.Runtime.Agents
{
    public class AgentMomentumGauge : MonoBehaviour, IAgentComponent
    {
        public delegate void ChangedMomentumGaugeEvent(float currentMomentum, float maxMomentum);

        public event ChangedMomentumGaugeEvent OnChangedMomentumGauge;

        public event Action OnMomentumGaugeFull;

        public float CurrentMomentumGauge
        {
            get => _currentMomentumGauge;
            protected set
            {
                _currentMomentumGauge = Mathf.Clamp(value, 0, _maxMomentumGauge.Value);
                OnChangedMomentumGauge?.Invoke(_currentMomentumGauge, _maxMomentumGauge.Value);

                if (_currentMomentumGauge >= _maxMomentumGauge.Value)
                {
                    OnMomentumGaugeFull?.Invoke();
                }
            }
        }

        public StatSO MaxMomentumGauge => _maxMomentumGauge;
        [SerializeField, Required] private StatSO _maxMomentumGauge;
        [SerializeField] private float _decreaseMomentumGaugePerTick = 4;
        [SerializeField, ReadOnly] private float _currentMomentumGauge;
        private CancellationTokenSource _autoDecreaseMomentumGaugeTokenSource;
        private CancellationTokenSource _checkDecreaseMomentumGaugeTokenSource;
        private Agent _agent;


        public virtual void Initialize(Agent agent)
        {
            _agent = agent;
            AgentStat agentStat = _agent.GetCompo<AgentStat>(true);
            _maxMomentumGauge = agentStat.GetStat(_maxMomentumGauge);
        }

        private async void Start()
        {
            await UniTask.NextFrame();
            CurrentMomentumGauge = 0;
        }

        public virtual async void IncreaseMomentumGauge(float value)
        {
            if (value == 0) return;
            CurrentMomentumGauge += value;

            try
            {
                StopDecreaseMomentumGauge();
                DisposeTokenSource();

                _checkDecreaseMomentumGaugeTokenSource = new CancellationTokenSource();
                await UniTask.WaitForSeconds(2, cancellationToken: _checkDecreaseMomentumGaugeTokenSource.Token);
                StartDecreaseMomentumGauge();
            }
            catch (Exception e)
            {
            }
        }

        private void OnDestroy()
        {
            DisposeTokenSource();
            StopDecreaseMomentumGauge();
        }

        public void DisposeTokenSource()
        {
            if (_checkDecreaseMomentumGaugeTokenSource != null &&
                !_checkDecreaseMomentumGaugeTokenSource.IsCancellationRequested)
            {
                _checkDecreaseMomentumGaugeTokenSource.Cancel();
                _checkDecreaseMomentumGaugeTokenSource.Dispose();
            }
        }

        public void DecreaseMomentumGauge(float value)
        {
            CurrentMomentumGauge -= value;
        }

        public virtual async void StartDecreaseMomentumGauge()
        {
            _autoDecreaseMomentumGaugeTokenSource = new();
            try
            {
                while (!_autoDecreaseMomentumGaugeTokenSource.IsCancellationRequested && CurrentMomentumGauge > 0)
                {
                    await UniTask.WaitForSeconds(.05f,
                        cancellationToken: _autoDecreaseMomentumGaugeTokenSource.Token);
                    DecreaseMomentumGauge(_decreaseMomentumGaugePerTick);
                }
            }
            catch (Exception e)
            {
            }
        }

        public void StopDecreaseMomentumGauge()
        {
            if (_autoDecreaseMomentumGaugeTokenSource != null &&
                !_autoDecreaseMomentumGaugeTokenSource.IsCancellationRequested)
            {
                _autoDecreaseMomentumGaugeTokenSource.Cancel();
                _autoDecreaseMomentumGaugeTokenSource.Dispose();
            }
        }
    }
}