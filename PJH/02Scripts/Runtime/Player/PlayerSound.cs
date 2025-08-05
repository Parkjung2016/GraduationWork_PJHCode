using FMODUnity;
using Main.Runtime.Agents;
using Main.Runtime.Equipments.Datas;
using Main.Runtime.Manager;
using UnityEngine;

namespace PJH.Runtime.Players
{
    public class PlayerSound : AgentSound
    {
        [SerializeField] private EventReference _evasionSound, _evasionWhileHitting;
        private Player _player;


        public override void Initialize(Agent agent)
        {
            base.Initialize(agent);
            _player = agent as Player;
        }

        public override void AfterInitialize()
        {
            base.AfterInitialize();
            _player.GetCompo<PlayerFullMount>().OnHitFullMountTarget += HandleHitFullMountTarget;
            PlayerAnimationTrigger animationTriggerCompo = _player.GetCompo<PlayerAnimationTrigger>();
            animationTriggerCompo.OnPlayEvasionSound += HandlePlayEvasionSound;
            _player.GetCompo<PlayerMovement>().OnEvasionWhileHitting += HandleEvasionWhileHitting;
            (_player.HealthCompo as PlayerHealth).OnBlockAttack += HandleBlockAttack;
            _player.GetCompo<PlayerInteract>().OnInteractWithoutParameter += HandleInteract;
        }

        private void OnDestroy()
        {
            _player.GetCompo<PlayerFullMount>().OnHitFullMountTarget -= HandleHitFullMountTarget;
            PlayerAnimationTrigger animationTriggerCompo = _player.GetCompo<PlayerAnimationTrigger>();
            animationTriggerCompo.OnPlayEvasionSound -= HandlePlayEvasionSound;
            _player.GetCompo<PlayerMovement>().OnEvasionWhileHitting -= HandleEvasionWhileHitting;
            (_player.HealthCompo as PlayerHealth).OnBlockAttack -= HandleBlockAttack;
            _player.GetCompo<PlayerInteract>().OnInteractWithoutParameter -= HandleInteract;
        }

        private void HandleEvasionWhileHitting()
        {
            RuntimeManager.PlayOneShot(_evasionWhileHitting, Camera.main.transform.position);
        }

        private void HandleInteract()
        {
            Managers.FMODManager.PlayButtonClick2Sound();
        }

        private void HandleHitFullMountTarget()
        {
            WeaponDataSO weaponData = _player.GetCompo<AgentWeaponManager>().CurrentWeapon.WeaponData;
            RuntimeManager.PlayOneShot(weaponData.hitImpactSound, Camera.main.transform.position);
        }


        private void HandlePlayEvasionSound()
        {
            RuntimeManager.PlayOneShot(_evasionSound, Camera.main.transform.position);
        }

        private void HandleBlockAttack()
        {
            WeaponDataSO weaponData = _player.GetCompo<AgentWeaponManager>().CurrentWeapon.WeaponData;
            RuntimeManager.PlayOneShot(weaponData.attackBlockSound, Camera.main.transform.position);
        }
    }
}