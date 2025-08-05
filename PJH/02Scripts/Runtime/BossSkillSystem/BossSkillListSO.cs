using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using YTH.Boss;
using ZLinq;

namespace PJH.Runtime.BossSkill
{
    [CreateAssetMenu(menuName = "SO/BossSkill/SkillList")]
    public class BossSkillListSO : SerializedScriptableObject
    {
        [SerializeField] private List<BossSkillSO> _skills;

        public BossSkillSO GetSkill(string skillName) =>
            _skills.AsValueEnumerable().FirstOrDefault(skill => skill.name.Contains(skillName));

        public void Init(Boss owner) => _skills.ForEach(skill => skill.Init(owner));

        public BossSkillListSO Clone()
        {
            BossSkillListSO instance = Instantiate(this);
            instance.name = instance.name.Replace("(Clone)", "");
            instance._skills = instance._skills.AsValueEnumerable().Select(skill =>
            {
                skill = Instantiate(skill);
                skill.name = skill.name.Replace("(Clone)", "");
                return skill;
            }).ToList();
            return instance;
        }
    }
}