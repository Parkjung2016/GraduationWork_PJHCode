using Main.Runtime.Characters.StateMachine;
using Opsive.BehaviorDesigner.Runtime.Tasks.Events;
using Opsive.GraphDesigner.Runtime;
using Opsive.GraphDesigner.Runtime.Variables;

namespace Main.Runtime.BT.Events
{
    [AllowMultipleTypes]
    public class OnChangedState : EventNode
    {
        private SharedVariable<ShadowCloneStateSO> _currentState;
        public ShadowCloneStateSO matchingHeroState;

        public override void Initialize(IGraph graph)
        {
            base.Initialize(graph);
            _currentState =
                m_BehaviorTree.GetVariable<ShadowCloneStateSO>("CurrentState");
            _currentState.OnValueChange += HandleStateChanged;
        }

        private void HandleStateChanged()
        {
            if (_currentState.Value != matchingHeroState) return;

            m_BehaviorTree.StartBranch(this);
        }
    }
}