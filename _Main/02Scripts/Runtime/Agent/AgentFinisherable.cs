using System;
using FIMSpace.FProceduralAnimation;
using Main.Core;
using Main.Runtime.Core;
using Main.Runtime.Core.Events;
using Main.Runtime.Manager;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Main.Runtime.Agents
{
    public class AgentFinisherable : MonoBehaviour, IAgentComponent
    {
        public Agent Agent => _agent;
        public event Action OnSetToFinisherTarget; // 처형됬을때

        [InfoBox("AgentFinisherable must have an \"AgentIK\" component and \"AgentMomentumGauge\" component",
            InfoMessageType.Warning)]
        private GameEventChannelSO _deadFinisherTargetEventChannel;

        private Collider _collider;
        private Agent _agent;
        private AgentMomentumGauge _momentumGaugeCompo;
        private AgentFullMountable _fullMountableCompo;

        public void Initialize(Agent agent)
        {
            _agent = agent;
            _collider = _agent.GetComponent<Collider>();
            _deadFinisherTargetEventChannel = AddressableManager.Load<GameEventChannelSO>("GameEventChannel");
            _momentumGaugeCompo = _agent.GetCompo<AgentMomentumGauge>(true);
            _fullMountableCompo = _agent.GetCompo<AgentFullMountable>(true);
        }

        public bool CanFinisher()
        {
            return _momentumGaugeCompo.CurrentMomentumGauge >= _momentumGaugeCompo.MaxMomentumGauge.Value &&
                   !_fullMountableCompo.IsFullMounted && !_agent.IsKnockDown;
        }

        public void SetToFinisherTarget()
        {
            _collider.enabled = false;
            _deadFinisherTargetEventChannel.AddListener<DeadFinisherTarget>(HandleDeadFinisherTarget);
            OnSetToFinisherTarget?.Invoke();
        }

        private void HandleDeadFinisherTarget(DeadFinisherTarget evt)
        {
            _deadFinisherTargetEventChannel.RemoveListener<DeadFinisherTarget>(HandleDeadFinisherTarget);
            _agent.HealthCompo.SetDeath();
            AgentAnimationTrigger agentAnimationTrigger = _agent.GetCompo<AgentAnimationTrigger>(true);
            agentAnimationTrigger.OnTriggerRagdoll?.Invoke();
        }

#if UNITY_EDITOR
        [BoxGroup("Utility", CenterLabel = true)]
        [GUIColor(0, 1, 0)]
        [HideIf("@this.GetComponentInChildren<Animator>().GetComponent<AgentIK>() != null")]
        [ButtonGroup("Utility/Buttons")]
        private void AddAgentIKComponent()
        {
            Animator animator = GetComponentInChildren<Animator>();
            AgentIK compo = animator.GetComponent<AgentIK>();
            if (compo != null)
            {
                Debug.LogWarning("AgentIK already exists");
                return;
            }

            animator.gameObject.AddComponent<AgentIK>();
            Selection.activeGameObject = animator.gameObject;
        }

        [HideIf("@this.GetComponentInChildren<Animator>().GetComponent<AgentIK>() == null")]
        [GUIColor(1, 0, 0)]
        [ButtonGroup("Utility/Buttons")]
        private void RemoveAgentIKComponent()
        {
            Animator animator = GetComponentInChildren<Animator>();
            AgentIK compo = animator.GetComponent<AgentIK>();
            if (compo == null)
            {
                Debug.LogWarning("AgentIK does not exist");
                return;
            }

            DestroyImmediate(compo);
        }

        [BoxGroup("Utility", CenterLabel = true)]
        [GUIColor(0, 1, 0)]
        [HideIf("@this.GetComponentInChildren<AgentMomentumGauge>() != null")]
        [ButtonGroup("Utility/Buttons")]
        private void AddAgentMomentumGaugeComponent()
        {
            AgentMomentumGauge compo = GetComponentInChildren<AgentMomentumGauge>();
            if (compo != null)
            {
                Debug.LogWarning("AgentMomentumGauge already exists");
                return;
            }

            GameObject obj = new GameObject("AgentMomentumGauge");
            obj.transform.SetParent(transform);
            obj.AddComponent<AgentMomentumGauge>();
            Selection.activeGameObject = obj;
        }

        [HideIf("@this.GetComponentInChildren<AgentMomentumGauge>() == null")]
        [GUIColor(1, 0, 0)]
        [ButtonGroup("Utility/Buttons")]
        private void RemoveAgentMomentumGaugeComponent()
        {
            AgentMomentumGauge compo = GetComponentInChildren<AgentMomentumGauge>();
            if (compo == null)
            {
                Debug.LogWarning("AgentMomentumGauge does not exists");
                return;
            }

            if (compo.gameObject.name == "AgentMomentumGauge")
                DestroyImmediate(compo.gameObject);
            else
                DestroyImmediate(compo);
        }
#endif
    }
}