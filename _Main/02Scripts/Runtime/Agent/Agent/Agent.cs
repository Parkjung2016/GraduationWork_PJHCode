using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Magio;
using Main.Core;
using Main.Runtime.Combat;
using Main.Runtime.Core;
using Main.Runtime.Core.StatSystem;
using Main.Shared;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Main.Runtime.Agents
{
    [RequireComponent(typeof(Health))]
    public abstract partial class Agent : SerializedMonoBehaviour, IAgent
    {
        public GameObject GameObject => gameObject;
        public Transform ModelTrm { get; protected set; }
        public Transform HeadTrm { get; protected set; }
        public Action<HitInfo> OnHitTarget { get; set; }

        public Health HealthCompo { get; private set; }
        public bool IsHitting { get; protected set; }
        public bool IsKnockDown { get; set; }

        [InfoBox("Agent must have an \"AgentStat\" component", InfoMessageType.Warning)]
        protected StatSO _maxHealthStat;

        public StatSO WalkSpeedStat { get; private set; }
        public StatSO RunSpeedStat { get; private set; }
        protected ComponentManager _componentManager;


        protected virtual void Awake()
        {
            WalkSpeedStat = AddressableManager.Load<StatSO>("WalkSpeedStat");
            RunSpeedStat = AddressableManager.Load<StatSO>("RunSpeedStat");
            _maxHealthStat = AddressableManager.Load<StatSO>("MaxHealthStat");
            ModelTrm = transform;
            HealthCompo = GetComponent<Health>();
            _componentManager = new ComponentManager();

            _componentManager.AddComponentToDictionary(this);
            _componentManager.ComponentInitialize(this);
            if (HealthCompo)
            {
                StatSO maxHealthStat = GetCompo<AgentStat>(true).GetStat(_maxHealthStat);
                HealthCompo.Init(maxHealthStat);
                HealthCompo.OnApplyDamaged += HandleApplyDamaged;
                HealthCompo.ailmentStat.OnAilmentChanged += HandleAilmentChanged;
            }

            _componentManager.AfterInitialize();

            AgentStat statCompo = GetCompo<AgentStat>(true);
            WalkSpeedStat = statCompo.GetStat(WalkSpeedStat);
            RunSpeedStat = statCompo.GetStat(RunSpeedStat);
            _maxHealthStat = statCompo.GetStat(_maxHealthStat);
            AgentAnimator animatorCompo = GetCompo<AgentAnimator>(true);
            if (animatorCompo != null) animatorCompo.OnEndHitAnimation += HandleEndHitAnimation;
        }


        protected virtual void Start()
        {
            HeadTrm = GetCompo<AgentAnimator>(true)?.Animancer?
                .GetBoneTransform(HumanBodyBones.Head);
        }

        private void HandleAilmentChanged(Ailment oldAilment, Ailment newAilment)
        {
            if ((newAilment & Ailment.Slow) > 0)
            {
                float percent = HealthCompo.ailmentStat.GetAilmentValue(Ailment.Slow);
                WalkSpeedStat.AddModifyValuePercent("SlowAilment", percent);
                RunSpeedStat.AddModifyValuePercent("SlowAilment", percent);
            }
            else
            {
                WalkSpeedStat.RemoveModifyValuePercent("SlowAilment");
                RunSpeedStat.RemoveModifyValuePercent("SlowAilment");
            }
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
        }

        public T GetCompo<T>(bool isDerived = false) where T : IAgentComponent
        {
            return _componentManager.GetCompo<T>(isDerived);
        }

        public bool TryGetCompo<T>(out T compo, bool isDerived = false) where T : IAgentComponent
        {
            return _componentManager.TryGetCompo(out compo, isDerived);
        }

        public CancellationTokenSource DelayCallBack(GameObject target, float delay, Action callBack = null)
        {
            CancellationTokenSource cts = new();
            cts.RegisterRaiseCancelOnDestroy(target);
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