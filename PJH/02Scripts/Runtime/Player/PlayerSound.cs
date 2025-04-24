using FIMSpace.FProceduralAnimation;
using FMODUnity;
using Main.Runtime.Agents;
using Main.Runtime.Core;
using Main.Runtime.Equipments.Datas;
using UnityEngine;

namespace PJH.Runtime.Players
{
    public class PlayerSound : AgentSound, LegsAnimator.ILegStepReceiver, IAfterInitable
    {
        [SerializeField] private EventReference _evasionSound;
        private Player _player;

        private AgentWeaponManager _weaponManagerCompo;

        public override void Initialize(Agent agent)
        {
            base.Initialize(agent);
            _player = agent as Player;
            _weaponManagerCompo = _player.GetCompo<AgentWeaponManager>();
        }

        public void AfterInitialize()
        {
            _player.GetCompo<PlayerFullMount>().OnHitFullMountTarget += HandleHitFullMountTarget;
            PlayerAnimationTrigger animationTriggerCompo = _player.GetCompo<PlayerAnimationTrigger>();
            animationTriggerCompo.OnPlayAttackWhooshSound += HandlePlayAttackWhooshSound;
            animationTriggerCompo.OnPlayEvasionSound += HandlePlayEvasionSound;
            (_player.HealthCompo as PlayerHealth).OnBlockAttack += HandleBlockAttack;
        }

        private void OnDestroy()
        {
            _player.GetCompo<PlayerFullMount>().OnHitFullMountTarget -= HandleHitFullMountTarget;

            PlayerAnimationTrigger animationTriggerCompo = _player.GetCompo<PlayerAnimationTrigger>();
            animationTriggerCompo.OnPlayAttackWhooshSound -= HandlePlayAttackWhooshSound;
            animationTriggerCompo.OnPlayEvasionSound -= HandlePlayEvasionSound;

            (_player.HealthCompo as PlayerHealth).OnBlockAttack -= HandleBlockAttack;
        }

        private void HandleHitFullMountTarget()
        {
            WeaponDataSO weaponData = _weaponManagerCompo.CurrentWeapon.WeaponData;
            RuntimeManager.PlayOneShot( weaponData.hitImpactSound, Camera.main.transform.position);
        }


        private void HandlePlayEvasionSound()
        {
            RuntimeManager.PlayOneShot(_evasionSound, Camera.main.transform.position);
        }

        private void HandleBlockAttack()
        {
            WeaponDataSO weaponData = _weaponManagerCompo.CurrentWeapon.WeaponData;
            RuntimeManager.PlayOneShot(weaponData.attackBlockSound, Camera.main.transform.position);
        }


        private void HandlePlayAttackWhooshSound()
        {
            WeaponDataSO weaponData = _weaponManagerCompo.CurrentWeapon.WeaponData;
            RuntimeManager.PlayOneShot(weaponData.attackWhooshSound, Camera.main.transform.position);
        }


        public void LegAnimatorStepEvent(LegsAnimator.Leg leg, float power, bool isRight, Vector3 position,
            Quaternion rotation, LegsAnimator.EStepType type)
        {
            PlayFootstepSound(position);
        }
    }
}