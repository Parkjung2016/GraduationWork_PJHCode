using Main.Runtime.Characters.StateMachine;
using Main.Runtime.Combat;
using Main.Runtime.Combat.Core;
using Main.Runtime.Core.StatSystem;

namespace PJH.Runtime.BossSkill.BossSkills.ShadowClones
{
    public class ShadowCloneHealth : Health
    {
        private ShadowClone _shadowClone;

        public override void Init(StatSO maxHealthStat, StatSO maxShieldStat)
        {
            base.Init(maxHealthStat, maxShieldStat);
            _shadowClone = _agent as ShadowClone;
        }

        protected override bool CanApplyDamage(GetDamagedInfo getDamagedInfo)
        {
            _shadowClone.GetCompo<ShadowCloneStateSystem>().ChangeState("AvoidState");
            return false;
        }
    }
}