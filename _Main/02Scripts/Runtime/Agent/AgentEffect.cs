using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Magio;
using Main.Runtime.Core;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Main.Runtime.Agents
{
    public class AgentEffect : SerializedMonoBehaviour, IAgentComponent, IAfterInitable
    {
        protected Agent _agent;
        public MagioObjectMaster MagioObjectMaster { get; private set; }

        [SerializeField, ReadOnly] private Dictionary<string, MagioObjectEffect> _magioObjectEffects;

        public virtual async void Initialize(Agent agent)
        {
            _agent = agent;
            MagioObjectMaster = _agent.GetComponentInChildren<MagioObjectMaster>();
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
            _agent.HealthCompo.ailmentStat.OnAilmentChanged += HandleAilmentChanged;
        }

        protected virtual void OnDestroy()
        {
            if (_agent.HealthCompo.ailmentStat != null)
                _agent.HealthCompo.ailmentStat.OnAilmentChanged -= HandleAilmentChanged;
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