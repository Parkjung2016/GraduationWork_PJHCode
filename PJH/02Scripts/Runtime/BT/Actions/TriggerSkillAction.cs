using Opsive.BehaviorDesigner.Runtime.Tasks;
using Opsive.BehaviorDesigner.Runtime.Tasks.Actions;
using Opsive.GraphDesigner.Runtime.Variables;
using PJH.Runtime.BossSkill;
using YTH.Enemies;

namespace PJH.Runtime.BT.Actions
{
    public class TriggerSkillAction : ActionNode
    {
        public SharedVariable<BaseEnemy> enemy;
        public BossSkillSO bossSkill;


        private BossSkillSO _activeBossSkill;

        public override void OnStart()
        {
            if (_activeBossSkill == null)
                _activeBossSkill = enemy.Value.GetCompo<BossSkillManager>().GetSKill(bossSkill);
            _activeBossSkill?.ActivateSkill();
        }

        public override TaskStatus OnUpdate()
        {
            return _activeBossSkill.IsSkillFinished() ? TaskStatus.Success : TaskStatus.Running;
        }
    }
}