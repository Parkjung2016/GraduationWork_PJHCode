using System.Threading.Tasks;
using Main.Runtime.Characters.StateMachine;
using Opsive.BehaviorDesigner.Runtime.Tasks.Conditionals;
using Opsive.GraphDesigner.Runtime.Variables;
using TaskStatus = Opsive.BehaviorDesigner.Runtime.Tasks.TaskStatus;

namespace Main.Runtime.BT.Conditionals
{
    public class CheckCurrentState : ConditionalNode
    {
        public ShadowCloneStateSO matchingState;
        private SharedVariable<ShadowCloneStateSO> _currentState;

        public override void OnAwake()
        {
            _currentState =
                m_BehaviorTree.GetVariable < ShadowCloneStateSO>("CurrentState");
        }


        public override TaskStatus OnUpdate()
        {
            return _currentState.Value == matchingState ? TaskStatus.Success : TaskStatus.Failure;
        }
    }
}