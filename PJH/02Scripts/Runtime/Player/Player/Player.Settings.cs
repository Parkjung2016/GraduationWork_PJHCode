using Main.Runtime.Agents;
using Main.Runtime.Core.Events;

namespace PJH.Runtime.Players
{
    public partial class Player
    {
        private void SubscribeEvents()
        {
            PlayerInput.LockOnToggleEvent += HandleLockOnToggle;
            PlayerMovement movementCompo = GetCompo<PlayerMovement>();
            movementCompo.OnRun += HandleRun;
            movementCompo.OnEvasionWhileHitting += HandleEvasionWhileHitting;
            PlayerEnemyFinisher enemyFinisher = GetCompo<PlayerEnemyFinisher>();
            enemyFinisher.OnFinisherTimeline += HandleFinisherTimeline;
            enemyFinisher.OnAdjustTimelineModelPosition += HandleAdjustTimelineModelPosition;
            GetCompo<PlayerFullMount>().OnFullMount += HandleFullMount;
            GetCompo<PlayerAnimationTrigger>().OnEndFullMount += HandleEndFullMount;
            GetCompo<AgentMomentumGauge>(true).OnMomentumGaugeFull += HandleMomentumGaugeFull;
            HealthCompo.OnDeath += HandleDeath;
            _gameEventChannel.AddListener<ReOffsetPlayer>(HandleReOffsetPlayer);
            _gameEventChannel.AddListener<FinishTimeline>(HandleFinishTimeline);
            _gameEventChannel.AddListener<FinishTimeline>(HandleFinishTimeline);
        }

        private void UnSubscribeEvents()
        {
            PlayerInput.LockOnToggleEvent -= HandleLockOnToggle;

            PlayerMovement movementCompo = GetCompo<PlayerMovement>();
            movementCompo.OnRun -= HandleRun;
            movementCompo.OnEvasionWhileHitting -= HandleEvasionWhileHitting;
            PlayerEnemyFinisher enemyFinisher = GetCompo<PlayerEnemyFinisher>();
            enemyFinisher.OnFinisherTimeline -= HandleFinisherTimeline;
            enemyFinisher.OnAdjustTimelineModelPosition -= HandleAdjustTimelineModelPosition;

            GetCompo<PlayerFullMount>().OnFullMount -= HandleFullMount;
            GetCompo<PlayerAnimationTrigger>().OnEndFullMount -= HandleEndFullMount;
            GetCompo<AgentMomentumGauge>(true).OnMomentumGaugeFull -= HandleMomentumGaugeFull;

            HealthCompo.OnDeath -= HandleDeath;

            _gameEventChannel.RemoveListener<ReOffsetPlayer>(HandleReOffsetPlayer);
            _gameEventChannel.RemoveListener<FinishTimeline>(HandleFinishTimeline);
        }
    }
}