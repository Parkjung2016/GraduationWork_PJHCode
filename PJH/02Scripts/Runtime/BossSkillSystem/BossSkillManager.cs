using FMODUnity;
using Main.Runtime.Agents;
using Main.Runtime.Core;
using Main.Runtime.Core.StatSystem;
using Main.Runtime.Manager;
using PJH.Runtime.BossSkill.BossSkills;
using PJH.Runtime.Players;
using UnityEngine;
using YTH.Boss;
using YTH.Enemies;

namespace PJH.Runtime.BossSkill
{
    public class BossSkillManager : MonoBehaviour, IAgentComponent, IAfterInitable
    {
        [SerializeField] private BossSkillListSO _skillList;
        [SerializeField] private StatSO _powerStat;
        private Player _player;
        private Boss _boss;

        public void Initialize(Agent agent)
        {
            _boss = agent as Boss;
            _skillList = _skillList.Clone();
            _skillList.Init(_boss);
            _powerStat = _boss.GetCompo<AgentStat>(true).GetStat(_powerStat);
        }

        public void AfterInitialize()
        {
            BossAnimationTrigger animationTriggerCompo = _boss.GetCompo<BossAnimationTrigger>();
            animationTriggerCompo.OnSilencePlayerPassive += HandleSilencePlayerPassive;
            animationTriggerCompo.OnHitPlayer += HandleHitPlayer;
        }

        private void OnDestroy()
        {
            BossAnimationTrigger animationTriggerCompo = _boss.GetCompo<BossAnimationTrigger>();
            animationTriggerCompo.OnSilencePlayerPassive -= HandleSilencePlayerPassive;
            animationTriggerCompo.OnHitPlayer -= HandleHitPlayer;
        }

        private void HandleHitPlayer()
        {
            Agent player = PlayerManager.Instance.Player;
            GrabAndAttackPlayerSkillSO grabSkill = GetSKill("GrabAndAttackPlayerSkill") as GrabAndAttackPlayerSkillSO;
            RuntimeManager.PlayOneShot(grabSkill.hitSound, player.transform.position);
            float power = _powerStat.Value * grabSkill.attackPowerMultiplier;
            player.HealthCompo.ApplyOnlyDamage(power);
        }

        private void HandleSilencePlayerPassive()
        {
            _skillList.GetSkill("SilencePlayerPassiveSkill")?.ActivateSkill();
        }

        public BossSkillSO GetSKill(string skillName) => _skillList.GetSkill(skillName);
        public BossSkillSO GetSKill(BossSkillSO skillSO) => _skillList.GetSkill(skillSO.name);
    }
}