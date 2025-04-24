using KHJ.Passive;
using Main.Core;
using Main.Runtime.Agents;
using Main.Shared;
using PJH.Runtime.Players;
using UnityEngine;

namespace PJH.Runtime.PlayerPassive
{
    [CreateAssetMenu(menuName = "SO/Passive/CloneAttackPassive")]
    public class CloneAttackPassiveSO : PassiveSO
    {
        public CloneAttackCombatDatabase cloneAttackCombatDatabase;
        public PoolTypeSO playerClonePoolType;
        private PoolManagerSO _poolManager;
        private PlayerAttack _attackCompo;

        public override void Init(IPlayer player)
        {
            base.Init(player);
            _poolManager = AddressableManager.Load<PoolManagerSO>("PoolManager");
            _attackCompo = _player.GetCompo<PlayerAttack>();
        }

        public override void ActivePassive()
        {
            _player.OnHitTarget += HandleHitTarget;
        }

        public override void DeactivePassive()
        {
            _player.OnHitTarget -= HandleHitTarget;
        }

        private void HandleHitTarget(HitInfo hitInfo)
        {
            PlayerClone playerClone = _poolManager.Pop(playerClonePoolType) as PlayerClone;
            playerClone.transform.SetPositionAndRotation(_player.transform.position, _player.ModelTrm.rotation);
            PlayerCombatDataSO key = _attackCompo.CurrentCombatData;
            PlayerCombatDataSO value = cloneAttackCombatDatabase[key];

            playerClone.Attack(hitInfo.hitTarget as Agent, value);
        }
    }
}