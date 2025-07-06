using Opsive.BehaviorDesigner.Runtime.Tasks;

namespace PJH.Runtime.BT.Actions
{
    public class ShadowCloneDisappearAction : ShadowCloneActionNode
    {
        public override TaskStatus OnUpdate()
        {
            _shadowClone.Disappear();
            return TaskStatus.Success;
        }
    }
}