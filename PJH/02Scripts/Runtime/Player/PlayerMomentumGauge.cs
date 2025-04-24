using Main.Runtime.Agents;
using Main.Runtime.Core;

namespace PJH.Runtime.Players
{
    public class PlayerMomentumGauge : AgentMomentumGauge, IAfterInitable
    {
        private Player _player;

        public override void Initialize(Agent agent)
        {
            _player = agent as Player;
            base.Initialize(agent);
        }

        public void AfterInitialize()
        {
            PlayerHealth playerHealth = (_player.HealthCompo as PlayerHealth);
            playerHealth.OnParrying += HandleParrying;
            _player.OnStartStun += HandleStartStun;
        }

        private void HandleParrying()
        {
            CurrentMomentumGauge -= CurrentMomentumGauge * 10 * 0.01f;
        }

        private void HandleStartStun()
        {
            CurrentMomentumGauge = 0;
        }

        public override void IncreaseMomentumGauge(float value)
        {
            if (_player.IsStunned) return;
            base.IncreaseMomentumGauge(value);
        }
    }
}