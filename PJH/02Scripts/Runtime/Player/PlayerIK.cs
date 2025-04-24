using FIMSpace;
using FIMSpace.FLook;
using Main.Runtime.Agents;
using UnityEngine;

namespace PJH.Runtime.Players
{
    public class PlayerIK : AgentIK
    {
        public FLookAnimator LookAnimator { get; private set; }
        public LeaningAnimator LeaningAnimator { get; private set; }

        private Player _player;
        private PlayerMovement _movementCompo;
        private PlayerAnimator _animatorCompo;
        private PlayerEnemyDetection _enemyDetectionCompo;

        public override void Initialize(Agent agent)
        {
            base.Initialize(agent);
            _player = agent as Player;
            LookAnimator = GetComponent<FLookAnimator>();
            LeaningAnimator = GetComponent<LeaningAnimator>();
            _movementCompo = _player.GetCompo<PlayerMovement>();
            _animatorCompo = _player.GetCompo<PlayerAnimator>();
            _enemyDetectionCompo = _player.GetCompo<PlayerEnemyDetection>();
        }

        public override void AfterInitialize()
        {
            base.AfterInitialize();
            _player.HealthCompo.OnApplyDamaged += HandleApplyDamaged;

            _movementCompo.OnEvasion += HandleEvasion;
            _movementCompo.OnMovement += HandleMovement;
            PlayerAttack attackCompo = _player.GetCompo<PlayerAttack>();
            attackCompo.OnAttack += HandleAttack;
            attackCompo.OnEnterBattle += HandleEnterBattle;
            attackCompo.OnExitBattle += HandleExitBattle;

            PlayerAnimationTrigger animationTriggerCompo = _player.GetCompo<PlayerAnimationTrigger>();
            animationTriggerCompo.OnEndCombo += HandleEndCombo;
            animationTriggerCompo.OnBlockEnd += HandleEndCombo;


            _player.GetCompo<PlayerEnemyFinisher>().OnFinisherTimeline += HandleFinisherTimeline;
            LookAnimator.SwitchLooking(false);
        }

        private void OnDestroy()
        {
            _player.HealthCompo.OnApplyDamaged -= HandleApplyDamaged;
            _movementCompo.OnEvasion -= HandleEvasion;
            _movementCompo.OnMovement -= HandleMovement;


            PlayerAttack attackCompo = _player.GetCompo<PlayerAttack>();
            attackCompo.OnAttack -= HandleAttack;
            attackCompo.OnEnterBattle -= HandleEnterBattle;
            attackCompo.OnExitBattle -= HandleExitBattle;

            PlayerAnimationTrigger animationTriggerCompo = _player.GetCompo<PlayerAnimationTrigger>();
            animationTriggerCompo.OnEndCombo -= HandleEndCombo;
            animationTriggerCompo.OnBlockEnd -= HandleEndCombo;

            _player.GetCompo<PlayerEnemyFinisher>().OnFinisherTimeline -= HandleFinisherTimeline;
        }

        private void HandleMovement(float velocity)
        {
            LegsAnimator.User_SetIsMoving(velocity > 0);
        }

        private void HandleEvasion()
        {
            HandleEndCombo();
        }

        private void HandleApplyDamaged(float obj)
        {
            HandleEndCombo();
        }


        private void HandleEnterBattle()
        {
            Agent targetEnemy = _enemyDetectionCompo.GetTargetEnemyNoInput();
            if (!targetEnemy) return;
            LookAnimator.SetLookTarget(targetEnemy.HeadTrm);
            LookAnimator.SwitchLooking(true);
        }

        private void HandleExitBattle()
        {
            LookAnimator.SetLookTarget(null);

            LookAnimator.SwitchLooking(false);
        }

        private void HandleFinisherTimeline(bool isPlayingTimeline)
        {
            LegsAnimator.enabled = !isPlayingTimeline;
            LookAnimator.enabled = !isPlayingTimeline;
        }

        private void HandleEndCombo()
        {
            LegsAnimator.User_SetIsMoving(false);
            LookAnimator.SwitchLooking(true);
        }

        private void HandleAttack()
        {
            LegsAnimator.User_SetIsMoving(true);
            LookAnimator.SwitchLooking(false);
        }
        
        
        protected override void HandleTriggerRagdoll()
        {
            base.HandleTriggerRagdoll();
            LegsAnimator.enabled = false;
            LookAnimator.enabled = false;
            LeaningAnimator.enabled = false;
        }

        private void Update()
        {
            if (_movementCompo.IsRunning)
            {
                if (LookAnimator.ObjectToFollow != null)
                {
                    if (_player.PlayerInput.Input.sqrMagnitude > 0)
                    {
                        LookAnimator.SwitchLooking(false);
                    }
                    else
                        LookAnimator.SwitchLooking(true);
                }

                LegsAnimator.User_SetDesiredMovementDirection(Vector3.zero, false);

                return;
            }

            Transform cameraTrm = Camera.main.transform;
            var right = cameraTrm.right;
            right.y = 0;
            var forward = Quaternion.AngleAxis(-90, Vector3.up) * right;
            Vector3 input = _player.PlayerInput.Input;
            Vector3 dir = (input.x * right) + (input.z * forward);
            dir = _animatorCompo.IsRootMotion || !_movementCompo.CC.enabled ? Vector3.zero : dir;
            LegsAnimator.User_SetDesiredMovementDirection(dir, false);
        }
    }
}