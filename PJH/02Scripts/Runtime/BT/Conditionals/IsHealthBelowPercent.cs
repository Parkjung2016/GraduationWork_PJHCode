using Main.Runtime.Agents;
using Main.Runtime.Combat;
using Opsive.BehaviorDesigner.Runtime.Tasks;
using Opsive.BehaviorDesigner.Runtime.Tasks.Conditionals;
using UnityEngine;

namespace Main.Runtime.BT.Conditionals
{
    public class IsHealthBelowPercent : ConditionalNode
    {
        [Range(0, 100)] public float thresholdPercent = 30f;
        private Health _healthCompo;

        public override void OnAwake()
        {
            _healthCompo = m_GameObject.GetComponent<Agent>().HealthCompo;
        }

        public override TaskStatus OnUpdate()
        {
            float currentHealthPercent = _healthCompo.CurrentHealth / _healthCompo.MaxHealth;
            return currentHealthPercent <= thresholdPercent ? TaskStatus.Success : TaskStatus.Failure;
        }
    }
}