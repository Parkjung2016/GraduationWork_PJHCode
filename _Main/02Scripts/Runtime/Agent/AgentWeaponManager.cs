using Main.Runtime.Combat;
using Main.Runtime.Core;
using Main.Runtime.Equipments.Scripts;
using Main.Shared;
using UnityEngine;

namespace Main.Runtime.Agents
{
    public class AgentWeaponManager : MonoBehaviour, IAgentComponent, IAfterInitable
    {
        private Agent _agent;

        protected AgentEquipmentSystem _equipmentSystemCompo;

        public CombatDataSO CurrentCombatData { get; set; }

        public Weapon CurrentWeapon { get; private set; }

        private Define.ESocketType _prevEnableDamageColliderSocket;

        public virtual void Initialize(Agent agent)
        {
            _agent = agent;
            _equipmentSystemCompo = _agent.GetCompo<AgentEquipmentSystem>();
            CurrentWeapon = _equipmentSystemCompo.GetSocket(Define.ESocketType.RightHand).GetItem<Weapon>();
        }

        public virtual void AfterInitialize()
        {
            AgentAnimationTrigger animationTriggerCompo = _agent.GetCompo<AgentAnimationTrigger>(true);
            animationTriggerCompo.OnEnableDamageCollider += HandleEnableDamageCollider;
            animationTriggerCompo.OnSetGetDamagedAnimationIndex += HandleSetGetDamagedAnimationIndex;
            animationTriggerCompo.OnDisableDamageCollider += HandleDisableDamageCollider;
        }

        private void OnDestroy()
        {
            AgentAnimationTrigger animationTriggerCompo = _agent.GetCompo<AgentAnimationTrigger>(true);
            animationTriggerCompo.OnEnableDamageCollider -= HandleEnableDamageCollider;
            animationTriggerCompo.OnSetGetDamagedAnimationIndex -= HandleSetGetDamagedAnimationIndex;

            animationTriggerCompo.OnDisableDamageCollider -= HandleDisableDamageCollider;
        }

        private void HandleSetGetDamagedAnimationIndex(int idx)
        {
            CurrentCombatData.currentGetDamagedAnimationClipIndex = idx;
        }

        private void HandleEnableDamageCollider(Define.ESocketType socketType)
        {
            Weapon weapon = _equipmentSystemCompo.GetSocket(_prevEnableDamageColliderSocket)?.GetItem<Weapon>();
            CurrentWeapon = weapon;
            if (weapon)
            {
                weapon.DisableDamageCollider();
            }

            weapon = _equipmentSystemCompo.GetSocket(socketType)?.GetItem<Weapon>();
            if (!weapon) return;
            weapon.TriggerDamageCollider(CurrentCombatData);
            _prevEnableDamageColliderSocket = socketType;
        }

        private void HandleDisableDamageCollider()
        {
            Weapon weapon = _equipmentSystemCompo.GetSocket(_prevEnableDamageColliderSocket)?.GetItem<Weapon>();
            if (weapon)
            {
                weapon.DisableDamageCollider();
            }
        }
    }
}