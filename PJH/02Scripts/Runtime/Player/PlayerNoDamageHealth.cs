using Main.Runtime.Combat.Core;

namespace PJH.Runtime.Players
{
    public class PlayerNoDamageHealth : PlayerHealth
    {
        protected override bool CanApplyDamage(GetDamagedInfo getDamagedInfo)
        {
            if (!base.CanApplyDamage(getDamagedInfo)) return false;
            getDamagedInfo.damage = 0;

            return true;
        }
    }
}