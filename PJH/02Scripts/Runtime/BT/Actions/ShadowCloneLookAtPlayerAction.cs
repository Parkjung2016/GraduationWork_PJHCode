using Opsive.BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace PJH.Runtime.BT.Actions
{
    public class ShadowCloneLookAtPlayerAction : ShadowCloneActionNode
    {
        public override TaskStatus OnUpdate()
        {
            Vector3 dir = (_player.transform.position - transform.position).normalized;
            dir.y = 0;
            Quaternion look = Quaternion.LookRotation(dir);
            Quaternion rotation = Quaternion.Slerp(transform.rotation, look, 5f * Time.deltaTime);
            transform.rotation = rotation;
            return TaskStatus.Running;
        }
    }
}