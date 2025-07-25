﻿using Main.Runtime.Agents;

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
            enemyFinisher.OnFinisher += HandleFinisher;
            enemyFinisher.OnFinisherEnd += HandleFinisherEnd;
            GetCompo<PlayerFullMount>().OnFullMount += HandleFullMount;
            GetCompo<PlayerAnimationTrigger>().OnEndFullMount += HandleEndFullMount;
            GetCompo<AgentMomentumGauge>(true).OnMomentumGaugeFull += HandleMomentumGaugeFull;
            HealthCompo.OnDeath += HandleDeath;
        }

        private void UnSubscribeEvents()
        {
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