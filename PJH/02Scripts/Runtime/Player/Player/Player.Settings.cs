using Main.Runtime.Agents;
using Main.Runtime.Core.Events;

namespace PJH.Runtime.Players
{
    public partial class Player
    {
        private void SubscribeEvents()
        {
            OnEndGrabbed += HandleEndGrabbed;
            PlayerInput.LockOnToggleEvent += HandleLockOnToggle;
            PlayerMovement movementCompo = GetCompo<PlayerMovement>();
            movementCompo.OnRun += HandleRun;
            movementCompo.OnEvasionWhileHitting += HandleEvasionWhileHitting;
            PlayerEnemyFinisher enemyFinisher = GetCompo<PlayerEnemyFinisher>();
            enemyFinisher.OnFinisher += HandleFinisher;
            enemyFinisher.OnFinisherEnd += HandleFinisherEnd;
            GetCompo<PlayerFullMount>().OnFullMount += HandleFullMount;
            GetCompo<PlayerAnimationTrigger>().OnEndFullMount += HandleEndFullMount;
            GetCompo<AgentMomentumGauge>(true).OnMomentumGaugeFull += HandleMomentumGaugeFull;
            HealthCompo.OnDeath += HandleDeath;
        }

        private void UnSubscribeEvents()
        {
            OnEndGrabbed -= HandleEndGrabbed;

            PlayerInput.LockOnToggleEvent -= HandleLockOnToggle;
            PlayerMovement movementCompo = GetCompo<PlayerMovement>();
            movementCompo.OnRun -= HandleRun;
            movementCompo.OnEvasionWhileHitting -= HandleEvasionWhileHitting;
            PlayerEnemyFinisher enemyFinisher = GetCompo<PlayerEnemyFinisher>();
            enemyFinisher.OnFinisher -= HandleFinisher;
            enemyFinisher.OnFinisherEnd -= HandleFinisherEnd;

            GetCompo<PlayerFullMount>().OnFullMount -= HandleFullMount;
            GetCompo<PlayerAnimationTrigger>().OnEndFullMount -= HandleEndFullMount;
            GetCompo<AgentMomentumGauge>(true).OnMomentumGaugeFull -= HandleMomentumGaugeFull;

            HealthCompo.OnDeath -= HandleDeath;
        }
    }
}