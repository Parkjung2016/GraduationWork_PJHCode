using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Main.Runtime.Combat;
using Main.Runtime.Core;
using Main.Runtime.Core.StatSystem;
using Main.Shared;
using Sirenix.OdinInspector;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using Debug = Main.Core.Debug;
#endif

namespace Main.Runtime.Agents
{
    [RequireComponent(typeof(Health))]
    public abstract class Agent : MonoBehaviour, IAgent
    {
        public GameObject GameObject => gameObject;
        public Transform ModelTrm { get; protected set; }
        public Transform HeadTrm { get; protected set; }
        public Action<HitInfo> OnHitTarget { get; set; }

        public Health HealthCompo { get; private set; }
        public bool IsHitting { get; protected set; }
        public bool IsKnockDown { get; set; }

        [InfoBox("Agent must have an \"AgentStat\" component", InfoMessageType.Warning)] [SerializeField, Required]
        protected StatSO _maxHealthStat;

        protected ComponentManager _componentManager;


#if UNITY_EDITOR
        [BoxGroup("Utility", CenterLabel = true)]
        [GUIColor(0, 1, 0)]
        [HideIf("@this.GetComponentInChildren<AgentStat>() != null")]
        [ButtonGroup("Utility/Buttons")]
        private void AddAgentStatComponent()
        {
            AgentStat agentStatCompo = GetComponentInChildren<AgentStat>();
            if (agentStatCompo != null)
            {
                Debug.LogWarning("AgentStat already exists");
                return;
            }

            GameObject obj = new GameObject("AgentStat");
            obj.transform.SetParent(transform);
            obj.AddComponent<AgentStat>();
            Selection.activeGameObject = obj;
        }

        [HideIf("@this.GetComponentInChildren<AgentStat>() == null")]
        [GUIColor(1, 0, 0)]
        [ButtonGroup("Utility/Buttons")]
        private void RemoveAgentStatComponent()
        {
            AgentStat agentStatCompo = GetComponentInChildren<AgentStat>();
            if (agentStatCompo == null)
            {
                Debug.LogWarning("AgentStat does not exists");
                return;
            }

            if (agentStatCompo.gameObject.name == "AgentStat")
                DestroyImmediate(agentStatCompo.gameObject);
            else
                DestroyImmediate(agentStatCompo);
        }

        [HideIf("@this.GetComponentInChildren<AgentEquipmentSystem>() != null")]
        [GUIColor(0, 1, 0)]
        [ButtonGroup("Utility/Buttons")]
        private void AddAgentEquipmentSystemComponent()
        {
            AgentEquipmentSystem agentEquipmentSystemCompo = GetComponentInChildren<AgentEquipmentSystem>();
            if (agentEquipmentSystemCompo != null)
            {
                Debug.LogWarning("AgentEquipmentSystem already exists");
                return;
            }

            GameObject obj = new GameObject("AgentEquipmentSystem");
            obj.transform.SetParent(transform);
            obj.AddComponent<AgentEquipmentSystem>();
            Selection.activeGameObject = obj;
        }


        [HideIf("@this.GetComponentInChildren<AgentEquipmentSystem>() == null")]
        [GUIColor(1, 0, 0)]
        [ButtonGroup("Utility/Buttons")]
        private void RemoveAgentEquipmentSystemComponent()
        {
            AgentEquipmentSystem agentEquipmentSystemCompo = GetComponentInChildren<AgentEquipmentSystem>();
            if (agentEquipmentSystemCompo == null)
            {
                Debug.LogWarning("AgentEquipmentSystem does not exists");
                return;
            }

            if (agentEquipmentSystemCompo.gameObject.name == "AgentEquipmentSystem")
                DestroyImmediate(agentEquipmentSystemCompo.gameObject);
            else
                DestroyImmediate(agentEquipmentSystemCompo);
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
        protected virtual void Awake()
        {
            ModelTrm = transform;
            HealthCompo = GetComponent<Health>();
            _componentManager = new ComponentManager();

            _componentManager.AddComponentToDictionary(this);
            _componentManager.ComponentInitialize(this);
            _componentManager.AfterInitialize();


            AgentAnimator animatorCompo = GetCompo<AgentAnimator>(true);
            if (animatorCompo != null) animatorCompo.OnEndHitAnimation += HandleEndHitAnimation;
        }

        protected virtual void Start()
        {
            if (HealthCompo)
            {
                StatSO maxHealthStat = GetCompo<AgentStat>(true).GetStat(_maxHealthStat);
                HealthCompo.Init(maxHealthStat);
                HealthCompo.OnApplyDamaged += HandleApplyDamaged;
            }

            HeadTrm = GetCompo<AgentAnimator>(true)?.Animancer?
                .GetBoneTransform(HumanBodyBones.Head);
        }

        protected virtual void OnDestroy()
        {
            HealthCompo.OnApplyDamaged -= HandleApplyDamaged;
            AgentAnimator animatorCompo = GetCompo<AgentAnimator>(true);
            if (animatorCompo != null)
                animatorCompo.OnEndHitAnimation -= HandleEndHitAnimation;
        }

        protected void HandleEndHitAnimation()
        {
            IsHitting = false;
        }

        protected virtual void HandleApplyDamaged(float damage)
        {
            IsHitting = true;
            if (TryGetCompo(out AgentMomentumGauge momentumGauge, true))
            {
                float increaseMomentumGauge = HealthCompo.GetDamagedInfo.increaseMomentumGauge;
                momentumGauge.IncreaseMomentumGauge(increaseMomentumGauge);
            }
        }

        public T GetCompo<T>(bool isDerived = false) where T : IAgentComponent
        {
            return _componentManager.GetCompo<T>(isDerived);
        }

        public bool TryGetCompo<T>(out T compo, bool isDerived = false) where T : IAgentComponent
        {
            return _componentManager.TryGetCompo(out compo, isDerived);
        }

        public CancellationTokenSource DelayCallBack(float delay, Action callBack = null)
        {
            CancellationTokenSource cts = new();
            UniTask.Create(async () =>
            {
                try
                {
                    await UniTask.WaitForSeconds(delay, cancellationToken: cts.Token);
                    callBack?.Invoke();
                }
                catch (OperationCanceledException)
                {
                }
            }).Forget();


            return cts;
        }
    }
}