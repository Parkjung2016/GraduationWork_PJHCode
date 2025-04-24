using Main.Runtime.Core;
using Main.Runtime.Core.StatSystem;
using Sirenix.OdinInspector;
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
        public float GetBaseValue(StatSO stat) => GetStat(stat).BaseValue;
        public float IncreaseBaseValue(StatSO stat, float value) => GetStat(stat).BaseValue += value;
        public void AddModifier(StatSO stat, object key, float value) => GetStat(stat).AddModifyValue(key, value);
        public void RemoveModifier(StatSO stat, object key) => GetStat(stat).RemoveModifyValue(key);

        public void AddIncreaseValuePercent(StatSO stat, object key, float value) =>
            GetStat(stat).AddIncreaseValuePercent(key, value);

        public void RemoveIncreaseValuePercent(StatSO stat, object key) =>
            GetStat(stat).RemoveIncreaseValuePercent(key);

        public void AddReductionValuePercent(StatSO stat, object key, float value) =>
            GetStat(stat).AddReductionValuePercent(key, value);

        public void RemoveReductionValuePercent(StatSO stat, object key) =>
            GetStat(stat).RemoveReductionValuePercent(key);


        public void ClearAllStatModifier()
        {
            foreach (var stat in _stats)
            {
                stat.ClearModifier();
            }
        }

        public void ClearAllStatIncreaseValuePercent()
        {
            foreach (var stat in _stats)
            {
                stat.ClearIncreaseValuePercent();
            }
        }

        public void ClearAllStatReductionValuePercent()
        {
            foreach (var stat in _stats)
            {
                stat.ClearReductionValuePercent();
            }
        }
    }
}