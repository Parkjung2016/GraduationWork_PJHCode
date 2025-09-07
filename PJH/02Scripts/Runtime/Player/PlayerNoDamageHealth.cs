using Main.Runtime.Combat.Core;

namespace PJH.Runtime.Players
{
    public class PlayerNoDamageHealth : PlayerHealth
    {
        public bool IsApplyDamage { get; set; }

        protected override bool CanApplyDamage(GetDamagedInfo getDamagedInfo)
        {
            if (!base.CanApplyDamage(getDamagedInfo)) return false;
            if (!IsApplyDamage)
                getDamagedInfo.damage = 0;

            return true;
        }
    }
}