using Main.Runtime.Agents;
using Main.Shared;
using PJH.Runtime.Players;
using Sirenix.Serialization;
using UnityEngine;

namespace PJH.Runtime.PlayerPassive.Passives
{
    [CreateAssetMenu(menuName = "SO/Passive/Active/TestActivePassive")]
    public class TestActivePassive : PassiveSO, IActivePassive, ICooldownPassive
    {
        public float duration;
        public float damage;
        [field: SerializeField, OdinSerialize] public CooldownPassiveInfo CooldownPassiveInfo { get; set; }
        
        public void ActivePassive()
        {
            Debug.Log("Active");
            _player.HealthCompo.ailmentStat.ApplyAilments(Ailment.Slow, duration, -damage);
            CooldownPassiveInfo.StartCooldownEvent?.Invoke();
        }

        public void DeActivePassive()
        {
        }
    }
}