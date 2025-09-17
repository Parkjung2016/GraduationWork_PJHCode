using System;
using Animancer;
using FIMSpace.FProceduralAnimation;
using FMODUnity;
using Main.Runtime.Core;
using Main.Runtime.Equipments.Datas;
using Main.Runtime.Equipments.Scripts;
using UnityEngine;

namespace Main.Runtime.Agents
{
    public class AgentSound : MonoBehaviour, IAgentComponent, IAfterInitable, LegsAnimator.ILegStepReceiver
    {
        private readonly string footstepPath = "event:/SFX/Footsteps/Footstep_Tile";
        private readonly string freezeSound = "event:/SFX/Ailment/Freeze";
        private readonly string freezeEndSound = "event:/SFX/Ailment/FreezeEnd";
        private readonly string bleedingSound = "event:/KHJ/PassiveSFX/Bleeding";
        private readonly string impactWallSound = "event:/SFX/ImpactWall2";
        private Agent _agent;

        public virtual void Initialize(Agent agent)
        {
            _agent = agent;
        }

        public virtual void AfterInitialize()
        {
            AgentAnimationTrigger animationTriggerCompo = _agent.GetCompo<AgentAnimationTrigger>(true);
            animationTriggerCompo.OnPlayAttackWhooshSound += HandlePlayAttackWhooshSound;
            animationTriggerCompo.OnPlayImpactWallSound += HandlePlayImpactWallSound;
            _agent.HealthCompo.OnAilmentChanged += HandleAilmentChanged;
            _agent.HealthCompo.ailmentStat.OnDotDamage += HandleDotDamageEvent;
        }

        protected virtual void OnDestroy()
        {
            AgentAnimationTrigger animationTriggerCompo = _agent.GetCompo<AgentAnimationTrigger>(true);
            animationTriggerCompo.OnPlayAttackWhooshSound -= HandlePlayAttackWhooshSound;
            animationTriggerCompo.OnPlayImpactWallSound -= HandlePlayImpactWallSound;
            if (_agent && _agent.HealthCompo && _agent.HealthCompo.ailmentStat != null)
            {
                _agent.HealthCompo.OnAilmentChanged -= HandleAilmentChanged;
                _agent.HealthCompo.ailmentStat.OnDotDamage -= HandleDotDamageEvent;
            }
        }

        private void HandlePlayImpactWallSound()
        {
            RuntimeManager.PlayOneShot(bleedingSound, _agent.transform.position);
        }

        private void HandleAilmentChanged(Ailment oldAilment, Ailment newAilment)
        {
            if ((newAilment & Ailment.Slow) > 0)
            {
                if ((oldAilment & Ailment.Slow) == 0)
                {
                    RuntimeManager.PlayOneShot(freezeSound, _agent.transform.position);
                }
            }
            else
            {
                if ((oldAilment & Ailment.Slow) > 0)
                {
                    RuntimeManager.PlayOneShot(freezeEndSound, _agent.transform.position);
                }
            }
        }

        private void HandleDotDamageEvent(Ailment ailmentType, float damage)
        {
            if (_agent.HealthCompo.IsDead)
            {
                return;
            }

            if ((ailmentType & Ailment.Dot) != 0)
            {
                RuntimeManager.PlayOneShot(bleedingSound, _agent.transform.position);
            }
        }

        public void PlayFootstepSound(Vector3 position)
        {
            RuntimeManager.PlayOneShot(footstepPath, position);
        }

        private void HandlePlayAttackWhooshSound()
        {
            Weapon currentWeapon = _agent.GetCompo<AgentWeaponManager>().CurrentWeapon;
            WeaponDataSO weaponData = currentWeapon.WeaponData;
            RuntimeManager.PlayOneShot(weaponData.attackWhooshSound, currentWeapon.transform.position);
        }

        public void LegAnimatorStepEvent(LegsAnimator.Leg leg, float power, bool isRight, Vector3 position,
            Quaternion rotation, LegsAnimator.EStepType type)
        {
            PlayFootstepSound(position);
        }
    }
}