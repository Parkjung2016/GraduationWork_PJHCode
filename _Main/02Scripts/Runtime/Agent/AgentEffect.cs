using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Magio;
using Main.Runtime.Core;
using Main.Runtime.Core.Events;
using Main.Shared;
using PJH.Utility.Managers;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Main.Runtime.Agents
{
    public class AgentEffect : SerializedMonoBehaviour, IAgentComponent, IAfterInitable
    {
        public MagioObjectMaster MagioObjectMaster { get; private set; }
        public DistanceFade DistanceFade { get; private set; }
        [SerializeField, ReadOnly] private Dictionary<string, MagioObjectEffect> _magioObjectEffects;
        [SerializeField, ReadOnly] Dictionary<Define.ESocketType, List<EffectPlayer>> _weaponEffectPlayers;


        protected Agent _agent;
        protected PoolManagerSO _poolManager;
        protected GameEventChannelSO _spawnEventChannel;
        private Define.ESocketType _currentSocketType;

        public virtual async void Initialize(Agent agent)
        {
            _agent = agent;
            _poolManager = AddressableManager.Load<PoolManagerSO>("PoolManager");
            _spawnEventChannel = AddressableManager.Load<GameEventChannelSO>("SpawnEventChannel");
            MagioObjectMaster = _agent.GetComponentInChildren<MagioObjectMaster>();
            DistanceFade = _agent.GetComponentInChildren<DistanceFade>();
            await UniTask.WaitUntil(() => MagioObjectMaster.didAwake,
                cancellationToken: gameObject.GetCancellationTokenOnDestroy());
            _magioObjectEffects = new();
            foreach (var magioObjectEffect in MagioObjectMaster.magioObjects)
            {
                _magioObjectEffects.Add(magioObjectEffect.gameObject.name, magioObjectEffect);
            }
        }

        public virtual void AfterInitialize()
        {
            AgentEquipmentSystem agentEquipmentSystem = _agent.GetCompo<AgentEquipmentSystem>();
            _weaponEffectPlayers = new();
            foreach (Define.ESocketType socketType in Enum.GetValues(typeof(Define.ESocketType)))
            {
                EffectPlayer[] effectPlayer =
                    agentEquipmentSystem.GetSocket(socketType).GetComponentsInChildren<EffectPlayer>();
                _weaponEffectPlayers.Add(socketType, effectPlayer.ToList());
            }

            _agent.HealthCompo.ailmentStat.OnAilmentChanged += HandleAilmentChanged;
            AgentAnimationTrigger animationTriggerCompo = _agent.GetCompo<AgentAnimationTrigger>(true);
            animationTriggerCompo.OnEnableDamageCollider += HandleEnableDamageCollider;
            animationTriggerCompo.OnDisableDamageCollider += HandleDisableDamageCollider;
        }

        protected virtual void OnDestroy()
        {
            if (_agent.HealthCompo.ailmentStat != null)
                _agent.HealthCompo.ailmentStat.OnAilmentChanged -= HandleAilmentChanged;

            AgentAnimationTrigger animationTriggerCompo = _agent.GetCompo<AgentAnimationTrigger>(true);
            animationTriggerCompo.OnEnableDamageCollider -= HandleEnableDamageCollider;
            animationTriggerCompo.OnDisableDamageCollider -= HandleDisableDamageCollider;
        }

        private void HandleEnableDamageCollider(Define.ESocketType socketType)
        {
            _currentSocketType = socketType;
            for (int i = 0; i < _weaponEffectPlayers[_currentSocketType].Count; i++)
            {
                _weaponEffectPlayers[_currentSocketType][i].PlayEffects();
            }
        }

        private void HandleDisableDamageCollider()
        {
            for (int i = 0; i < _weaponEffectPlayers[_currentSocketType].Count; i++)
            {
                _weaponEffectPlayers[_currentSocketType][i].StopEffects();
            }
        }

        private void HandleAilmentChanged(Ailment oldAilment, Ailment newAilment)
        {
            foreach (Ailment type in Enum.GetValues(typeof(Ailment)))
            {
                if (type == Ailment.None) continue;
                string effectName = $"{type.ToString()}Effect";
                if ((newAilment & type) > 0)
                {
                    PlayMagioEffect(effectName);
                }
                else
                {
                    StopMagioEffect(effectName);
                }
            }
        }

        public MagioObjectEffect GetMagioObjectEffect(string magioObjectName)
        {
            return _magioObjectEffects.GetValueOrDefault(magioObjectName);
        }

        public void PlayMagioEffect(string magioObjectName)
        {
            MagioObjectEffect effect = GetMagioObjectEffect(magioObjectName);
            effect.gameObject.SetActive(true);
            effect.TryToAnimateEffect(_agent.transform.position, 0.0f);
        }

        public void StopMagioEffect(string magioObjectName)
        {
            MagioObjectEffect effect = GetMagioObjectEffect(magioObjectName);
            effect.SetNullifyArea(_agent.transform.position, 5f);
            DOVirtual.DelayedCall(3f, () => effect.gameObject.SetActive(false));
        }
    }
}