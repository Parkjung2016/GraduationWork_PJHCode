using Main.Runtime.Manager;
using Opsive.BehaviorDesigner.Runtime.Tasks;
using Opsive.BehaviorDesigner.Runtime.Tasks.Conditionals;
using UnityEngine;

namespace Main.Runtime.BT.Conditionals
{
    public class IsPlayerInRange : ConditionalNode
    {
        private Transform _playerTrm;
        public float range = 5f;

        public override void OnAwake()
        {
            base.OnAwake();
            _playerTrm = PlayerManager.Instance.Player.transform;
        }

        public override TaskStatus OnUpdate()
        {
            float distance = Vector3.Distance(transform.position, _playerTrm.position);

            return distance <= range ? TaskStatus.Success : TaskStatus.Failure;
        }
    }
}