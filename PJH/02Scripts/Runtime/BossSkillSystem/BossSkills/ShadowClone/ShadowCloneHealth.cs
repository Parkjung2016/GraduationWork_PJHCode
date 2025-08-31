using Main.Runtime.Characters.StateMachine;
using Main.Runtime.Combat;
using Main.Runtime.Combat.Core;
using Main.Runtime.Core.StatSystem;
using Main.Shared;

namespace PJH.Runtime.BossSkill.BossSkills.ShadowClones
{
    public class ShadowCloneHealth : Health
    {
        private ShadowClone _shadowClone;

        public override void Init(IAgent agent, StatSO maxHealthStat, StatSO maxShieldStat)
        {
            base.Init(agent, maxHealthStat, maxShieldStat);
            _shadowClone = _agent as ShadowClone;
        }

        protected override bool CanApplyDamage(GetDamagedInfo getDamagedInfo)
        {
            _shadowClone.GetCompo<ShadowCloneStateSystem>().ChangeState("AvoidState");
            return false;
        }
    }
}