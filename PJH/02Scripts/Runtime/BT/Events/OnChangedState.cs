using Main.Core;
using Main.Runtime.Characters.StateMachine;
using Opsive.GraphDesigner.Editor.Elements;
using Opsive.GraphDesigner.Runtime;
using Opsive.GraphDesigner.Runtime.Variables;
using EventNode = Opsive.BehaviorDesigner.Runtime.Tasks.Events.EventNode;

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

            Debug.Log(_currentState.Value);
            m_BehaviorTree.StartBranch(this);
        }
    }
}