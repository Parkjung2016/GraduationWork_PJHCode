using Animancer;

namespace Main.Runtime.Combat.Core
{
    public interface IParryingable
    {
        public void ParryingSuccess(float increaseMomentumGauge, ITransition blockedAttackByParringAnimation);
    }
}