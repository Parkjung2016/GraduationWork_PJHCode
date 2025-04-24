namespace Main.Runtime.Combat.Core
{
    public interface IDamageable
    {
        public bool ApplyDamage(GetDamagedInfo getDamagedInfo);
    }
}