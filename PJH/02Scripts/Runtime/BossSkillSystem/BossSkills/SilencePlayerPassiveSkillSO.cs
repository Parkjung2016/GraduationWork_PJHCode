using System.Threading.Tasks;
using Animancer;
using FMODUnity;
using Main.Runtime.Combat.Core;
using Main.Runtime.Core.Events;
using PJH.Runtime.Players;
using UnityEngine;

namespace PJH.Runtime.BossSkill.BossSkills
{
    [CreateAssetMenu(menuName = "SO/BossSkill/Skills/SilencePlayerPassiveSkill")]
    public class SilencePlayerPassiveSkillSO : BossSkillSO
    {
        public float castRadius = 3f;
        public float silenceDuration = 3f;
        public float damage = 10;
        public EventReference silenceEffectSound, hitSound;
        public ClipTransition getDamagedAnimationClip;
        public PoolTypeSO silenceEffectPoolType;

        public override void ActivateSkill()
        {
            RuntimeManager.PlayOneShot(silenceEffectSound, _boss.transform.position);
            var evt = SpawnEvents.SpawnEffect;
            evt.effectType = silenceEffectPoolType;
            evt.position = _boss.transform.position + Vector3.up;
            evt.rotation = Quaternion.identity;
            _spawnEventChannel.RaiseEvent(evt);
            Player player = GetPlayer();
            float distance = Vector3.Distance(_boss.transform.position, player.transform.position);
            if (distance <= castRadius)
            {
                GetDamagedInfo getDamagedInfo = new GetDamagedInfo()
                    .SetAttacker(_boss)
                    .SetDamage(damage)
                    .SetGetDamagedAnimationClipOnIgnoreDirection(getDamagedAnimationClip)
                    .SetIgnoreDirection(true)
                    .SetHitPoint(_boss.transform.position);
                RuntimeManager.PlayOneShot(hitSound, _boss.transform.position);

                player.HealthCompo.ApplyDamage(getDamagedInfo);
                player.ApplySilencePassive(silenceDuration);
            }
        }
    }
}