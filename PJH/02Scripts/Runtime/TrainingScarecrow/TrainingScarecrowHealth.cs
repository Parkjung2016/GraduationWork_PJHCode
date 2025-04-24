using Main.Runtime.Combat;
using Main.Runtime.Combat.Core;
using UnityEngine;

namespace PJH.Trainingscarecrow
{
    public class TrainingScarecrowHealth : Health
    {
        public override bool ApplyDamage(GetDamagedInfo getDamagedInfo)
        {
            getDamagedInfo.damage = 0;
            return base.ApplyDamage(getDamagedInfo);
        }
    }
}