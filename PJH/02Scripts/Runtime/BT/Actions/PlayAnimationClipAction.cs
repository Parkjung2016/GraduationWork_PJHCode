using Animancer;
using Main.Runtime.Agents;
using Opsive.BehaviorDesigner.Runtime.Tasks;
using Opsive.BehaviorDesigner.Runtime.Tasks.Actions;

namespace PJH.Runtime.BT.Actions
{
    public class PlayAnimationClipAction : ActionNode
    {
        private Agent _agent;
        public TransitionAsset animationClip;
        private bool _animationFinished;


        public override void OnAwake()
        {
            base.OnAwake();
            _agent = GetComponent<Agent>();
        }

        public override void OnStart()
        {
            _animationFinished = false;
            AgentAnimator animatorCompo = _agent.GetCompo<AgentAnimator>(true);
            animatorCompo.PlayAnimationClip(animationClip, () => { _animationFinished = true; });
        }

        public override TaskStatus OnUpdate()
        {
            return _animationFinished ? TaskStatus.Success : TaskStatus.Running;
        }
    }
}