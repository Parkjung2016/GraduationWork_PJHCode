using Main.Runtime.Agents;
using Main.Runtime.Core;
using UnityEngine;
using YTH.Boss;
using YTH.Enemies;

namespace PJH.Runtime.BossSkill
{
    public class BossSkillManager : MonoBehaviour, IAgentComponent, IAfterInitable
    {
        [SerializeField] private BossSkillListSO _skillList;
        private Boss _boss;

        public void Initialize(Agent agent)
        {
            _boss = agent as Boss;
            _skillList = _skillList.Clone();
            _skillList.Init(_boss);
        }

        public void AfterInitialize()
        {
            _boss.GetCompo<BossAnimationTrigger>().OnSilencePlayerPassive += HandleSilencePlayerPassive;
        }

        private void OnDestroy()
        {
            _boss.GetCompo<BossAnimationTrigger>().OnSilencePlayerPassive -= HandleSilencePlayerPassive;
        }

        private void HandleSilencePlayerPassive()
        {
            _skillList.GetSkill("SilencePlayerPassiveSkill")?.ActivateSkill();
        }

        public BossSkillSO GetSKill(string skillName) => _skillList.GetSkill(skillName);
        public BossSkillSO GetSKill(BossSkillSO skillSO) => _skillList.GetSkill(skillSO.name);
    }
}