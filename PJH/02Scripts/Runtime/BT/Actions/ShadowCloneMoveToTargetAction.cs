using Opsive.BehaviorDesigner.Runtime.Tasks;
using PJH.Runtime.BossSkill.BossSkills.ShadowClones;
using UnityEngine;

namespace PJH.Runtime.BT.Actions
{
    public class ShadowCloneMoveToPlayerAction : ShadowCloneActionNode
    {
        private ShadowCloneMovement _movementCompo;

        [SerializeField] private float _moveUpdateTime = 0.1f;
        private float _currentMoveUpdateTime;

        public override void OnAwake()
        {
            base.OnAwake();
            _movementCompo = _shadowClone.GetCompo<ShadowCloneMovement>();
        }

        public override void OnStart()
        {
            base.OnStart();

            _movementCompo.SetCanMove(true);
            _movementCompo.SetRVOControllerLocked(false);
            _currentMoveUpdateTime = 0;
            _movementCompo.SetDestination(_player.transform.position);
        }

        public override TaskStatus OnUpdate()
        {
            if (_currentMoveUpdateTime <= _moveUpdateTime)
                _currentMoveUpdateTime += Time.deltaTime;
            else
            {
                _currentMoveUpdateTime = 0;
                _movementCompo.SetDestination(_player.transform.position);
            }

            return TaskStatus.Running;
        }

        public override void OnEnd()
        {
            _movementCompo.SetCanMove(false);
        }
    }
}