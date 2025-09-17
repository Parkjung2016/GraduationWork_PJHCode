using Opsive.BehaviorDesigner.Runtime.Tasks;
using Opsive.BehaviorDesigner.Runtime.Tasks.Actions;
using Opsive.GraphDesigner.Runtime.Variables;
using PJH.Runtime.BossSkill;
using YTH;
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
            if (enemy.Value.TryGetCompo(out EnemyCounterCompo counterCompo))
            {
                counterCompo.CanCounter = false;
            }
        }

        public override TaskStatus OnUpdate()
        {
            bool isSkillFinished = _activeBossSkill.IsSkillFinished();
            if (isSkillFinished)
            {
                if (enemy.Value.TryGetCompo(out EnemyCounterCompo counterCompo))
                {
                    counterCompo.CanCounter = true;
                }

                return TaskStatus.Success;
            }

            return TaskStatus.Running;
        }
    }
}