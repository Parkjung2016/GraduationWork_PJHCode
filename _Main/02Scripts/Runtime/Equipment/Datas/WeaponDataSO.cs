using Animancer;
using FMODUnity;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Main.Runtime.Equipments.Datas
{
    [CreateAssetMenu(menuName = "SO/Equipment/Data/Weapon")]
    public class WeaponDataSO : EquipmentDataSO
    {
        public EventReference attackBlockSound;
        public EventReference attackWhooshSound;
        public EventReference hitImpactSound;
        public float damageMultiplier = 1;
        public float damageMultiplierWhenThrow = 1;
        public float increaseMomentumGauge = 10;
        [ShowIf("isDurable"), SerializeField] public int maxDurability;
        public bool isDurable = true;
    }
}