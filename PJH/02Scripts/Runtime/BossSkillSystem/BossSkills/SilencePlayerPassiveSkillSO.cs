using Animancer;
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
        public ClipTransition getDamagedAnimationClip;
        public PoolTypeSO silenceEffectPoolType;

        public override void ActivateSkill()
        {
            var evt = SpawnEvents.SpawnEffect;
            evt.effectType = silenceEffectPoolType;
            evt.position = _boss.transform.position + Vector3.up;
            evt.rotation = Quaternion.identity;
            _spawnEventChannel.RaiseEvent(evt);
            Player player = GetPlayer();
            float distance = Vector3.Distance(_boss.transform.position, player.transform.position);
            if (distance <= castRadius)
            {
                GetDamagedInfo getDamagedInfo = new()
                {
                    attacker = _boss,
                    damage = damage,
                    getDamagedAnimationClipOnIgnoreDirection = getDamagedAnimationClip,
                    ignoreDirection = true,
                    hitPoint = _boss.transform.position,
                    increaseMomentumGauge = 0
                };
                player.HealthCompo.ApplyDamage(getDamagedInfo);
                player.ApplySilencePassive(silenceDuration);
            }
        }
    }
}