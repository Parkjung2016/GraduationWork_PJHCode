using BIS.Core.Utility;
using BIS.UI;
using Main.Runtime.Agents;
using Main.Runtime.Combat;
using YTH.Boss;

namespace PJH.Runtime.UI
{
    public class BossPostureProgressUI : PostureProgressUI
    {
        private Boss _boss;
        private void Start()
        {
            _boss = FindAnyObjectByType<Boss>();
            if (_boss != null)
            {
                Health health = _boss.GetComponent<Health>();
                health.OnDeath += HandleDeath;
                AgentMomentumGauge momentumGaugeCompo = _boss.GetCompo<AgentMomentumGauge>(true);
                momentumGaugeCompo.OnChangedMomentumGauge += SetUpProgress;
            }
        }

        private void HandleDeath()
        {
            Util.UIFadeOut(gameObject, true);
        }


        private void OnDisable()
        {
            if (_boss != null)
            {
                Health health = _boss.GetComponent<Health>();
                health.OnDeath += HandleDeath;
                _boss.GetCompo<AgentMomentumGauge>(true).OnChangedMomentumGauge -= SetUpProgress;
            }
        }

        private void OnEnable()
        {
            if (_boss != null)
            {
                Health health = _boss.GetComponent<Health>();
                health.OnDeath += HandleDeath;
                AgentMomentumGauge momentumGaugeCompo = _boss.GetCompo<AgentMomentumGauge>(true);
                momentumGaugeCompo.OnChangedMomentumGauge += SetUpProgress;
            }
        }
    }
}