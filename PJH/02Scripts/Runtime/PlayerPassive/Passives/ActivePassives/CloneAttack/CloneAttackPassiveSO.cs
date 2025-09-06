using Main.Runtime.Agents;
using Main.Shared;
using PJH.Runtime.Players;
using PJH.Utility.Managers;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace PJH.Runtime.PlayerPassive.Passives
{
    [CreateAssetMenu(menuName = "SO/Passive/Active/CloneAttackPassive")]
    public class CloneAttackPassiveSO : PassiveSO, IActivePassive, ICooldownPassive
    {
        [VerticalGroup("Top/Right")] [LabelText("📂 분신 전투 데이터베이스")]
        public CloneAttackCombatDatabase cloneAttackCombatDatabase;

        [VerticalGroup("Top/Right")] [LabelText("👤 플레이어 분신 풀 타입")]
        public PoolTypeSO playerClonePoolType;

        private PoolManagerSO _poolManager;
        [field: SerializeField, OdinSerialize] public CooldownPassiveInfo CooldownPassiveInfo { get; set; }

        public override void EquipPiece(IPlayer player)
        {
            base.EquipPiece(player);
            _poolManager = AddressableManager.Load<PoolManagerSO>("PoolManager");
        }

        public void ActivePassive()
        {
            _player.OnHitTarget += HandleHitTarget;
        }

        public void DeActivePassive()
        {
            _player.OnHitTarget -= HandleHitTarget;
        }

        private void HandleHitTarget(HitInfo hitInfo)
        {
            CooldownPassiveInfo.StartCooldownEvent?.Invoke();

            PlayerAttack attackCompo = _player.GetCompo<PlayerAttack>();
            PlayerCombatDataSO key = attackCompo.CurrentCombatData;
            CloneAttackCombatData cloneAttackCombatData = cloneAttackCombatDatabase[key];

            PlayerClone playerClone = _poolManager.Pop(playerClonePoolType) as PlayerClone;

            Vector3 clonePosition = _player.transform.position +
                                    (_player.ModelTrm.forward * cloneAttackCombatData.additionalCloneOffset);
            Vector3 cloneDirection = (hitInfo.hitTarget.GameObject.transform.position - clonePosition).normalized;
            cloneDirection.y = 0;
            Quaternion cloneRotation = Quaternion.LookRotation(cloneDirection);
            playerClone.transform.SetPositionAndRotation(clonePosition, cloneRotation);

            playerClone.Attack(hitInfo.hitTarget as Agent, cloneAttackCombatData.combatData);
        }
    }
}