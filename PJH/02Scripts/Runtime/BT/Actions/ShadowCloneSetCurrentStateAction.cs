using Main.Runtime.Characters.StateMachine;
using Opsive.BehaviorDesigner.Runtime.Tasks;
using PJH.Runtime.BT.Actions;

public class ShadowCloneSetCurrentStateAction : ShadowCloneActionNode
{
    public ShadowCloneStateSO nextState;
    public bool isForceTransition = false;

    public override TaskStatus OnUpdate()
    {
        ShadowCloneStateSystem stateSystemCompo = _shadowClone.GetCompo<ShadowCloneStateSystem>();
        stateSystemCompo.ChangeState(nextState, isForceTransition);
        return TaskStatus.Success;
    }
}