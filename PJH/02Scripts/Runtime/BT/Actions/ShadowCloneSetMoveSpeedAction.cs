using Main.Runtime.Agents;
using Main.Runtime.Core.StatSystem;
using Opsive.BehaviorDesigner.Runtime.Tasks;
using PJH.Runtime.BossSkill.BossSkills.ShadowClones;

namespace PJH.Runtime.BT.Actions
{
    public class ShadowCloneSetMoveSpeedAction : ShadowCloneActionNode
    {
        public StatSO moveStat;

        public override void OnAwake()
        {
            base.OnAwake();
            moveStat = _shadowClone.GetCompo<AgentStat>().GetStat(moveStat);
        }

        public override TaskStatus OnUpdate()
        {
            _shadowClone.GetCompo<ShadowCloneMovement>().AIPathCompo.maxSpeed = moveStat.Value;
            return TaskStatus.Success;
        }
    }
}