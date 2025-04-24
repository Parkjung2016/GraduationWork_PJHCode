using UnityEngine;

namespace Main.Runtime.Combat.Core
{
    public interface IKnockBackable
    {
        public void KnockBack(Vector3 knockBackDir, float knockBackPower, float knockBackDuration);
    }
}