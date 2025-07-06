using Main.Shared;
using PJH.Runtime.Players;
using Sirenix.Serialization;
using UnityEngine;

namespace PJH.Runtime.PlayerPassive.Passives
{
    [CreateAssetMenu(menuName = "SO/Passive/Buff/TestBuffPassive")]
    public class TestBuffPassiveSO : PassiveSO, IBuffPassive, ICooldownPassive
    {
        [field: SerializeField, OdinSerialize] public BuffPassiveInfo BuffPassiveInfo { get; set; }
        [field: SerializeField, OdinSerialize] public CooldownPassiveInfo CooldownPassiveInfo { get; set; }

        [SerializeField] private PoolTypeSO _poolType;


        public override void EquipPiece(IPlayer player)
        {
            base.EquipPiece(player);
            BuffPassiveInfo.ApplyBuffEvent?.Invoke();
        }

        public void StartBuff()
        {
            // _player.GetCompo<PlayerEffect>().PlayEffectAttachedToBody(_poolType, HumanBodyBones.Head);
        }

        public void EndBuff()
        {
            CooldownPassiveInfo.StartCooldownEvent?.Invoke();

            // _player.GetCompo<PlayerEffect>().StopEffectAttachedToBody(_poolType, HumanBodyBones.Head);
        }
    }
}