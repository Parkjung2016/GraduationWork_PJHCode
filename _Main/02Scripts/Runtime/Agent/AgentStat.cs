using System;
using Main.Runtime.Core;
using Main.Runtime.Core.StatSystem;
using Sirenix.OdinInspector;
using System.Linq;
using UnityEngine;
using ZLinq;

namespace Main.Runtime.Agents
{
    public class AgentStat : MonoBehaviour, IAgentComponent
    {
        [SerializeField, Required, InlineEditor]
        protected StatOverrideListSO _statOverrideList;

        protected StatSO[] _stats;
        protected Agent _agent;

        public virtual void Initialize(Agent agent)
        {
            _agent = agent;
            _stats = _statOverrideList.StatOverrides.Select(x => x.CreateStat()).ToArray();
        }

        public StatSO GetStat(StatSO stat)
        {
            Debug.Assert(stat != null, "Stats : GetStat - stat cannot be null");
            return _stats.FirstOrDefault(x => x.statName == stat.statName);
        }

        public bool TryGetStat(StatSO stat, out StatSO outStat)
        {
            Debug.Assert(stat != null, "Stats : GetStat - stat cannot be null");
            outStat = _stats.FirstOrDefault(x => x.statName == stat.statName);
            return outStat != null;
        }

        public void SetBaseValue(StatSO stat, float value) => GetStat(stat).BaseValue = value;

        public void AddBaseValuePercent(StatSO stat, float percent)
        {
            StatSO agentStat = GetStat(stat);
            agentStat.BaseValue *= (1 + percent * .01f);
        }

        public void AddBaseValue(StatSO stat, float value) => GetStat(stat).BaseValue += value;
        public float GetBaseValue(StatSO stat) => GetStat(stat).BaseValue;
        public float IncreaseBaseValue(StatSO stat, float value) => GetStat(stat).BaseValue += value;
        public void AddValueModifier(StatSO stat, object key, float value) => GetStat(stat).AddModifyValue(key, value);
        public void RemoveValueModifier(StatSO stat, object key) => GetStat(stat).RemoveModifyValue(key);

        public void AddValuePercentModifier(StatSO stat, object key, float value) =>
            GetStat(stat).AddModifyValuePercent(key, value);

        public void RemoveValuePercentModifier(StatSO stat, object key) =>
            GetStat(stat).RemoveModifyValuePercent(key);


        public void ClearAllStatModifier()
        {
            foreach (var stat in _stats)
            {
                stat.ClearModifier();
            }
        }

        public void ClearAllStatValueModifier()
        {
            foreach (var stat in _stats)
            {
                stat.ClearModifyValue();
            }
        }

        public void ClearAllStatValuePercentModifier()
        {
            foreach (var stat in _stats)
            {
                stat.ClearModifyValuePercent();
            }
        }
    }
}