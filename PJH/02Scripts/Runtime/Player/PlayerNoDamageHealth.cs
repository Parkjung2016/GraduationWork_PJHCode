using Main.Runtime.Combat.Core;

namespace PJH.Runtime.Players
{
    public class PlayerNoDamageHealth : PlayerHealth
    {
        public override bool ApplyDamage(GetDamagedInfo getDamagedInfo)
        {
            getDamagedInfo.damage = 0;
            return base.ApplyDamage(getDamagedInfo);
        }

        public override bool ApplyOnlyDamage(float damage)
        {
            damage = 0;
            return base.ApplyOnlyDamage(damage);
        }
    }
}