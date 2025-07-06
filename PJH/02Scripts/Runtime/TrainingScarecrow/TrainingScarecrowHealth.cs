using Main.Runtime.Combat;
using Main.Runtime.Combat.Core;
using UnityEngine;

namespace PJH.Trainingscarecrow
{
    public class TrainingScarecrowHealth : Health
    {
        protected override bool CanApplyDamage(GetDamagedInfo getDamagedInfo)
        {
            if (!base.CanApplyDamage(getDamagedInfo)) return false;
            getDamagedInfo.damage = 0;
            return true;
        }
    }
}