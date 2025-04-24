using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Main.Runtime.Combat
{
    [CreateAssetMenu(fileName = "CombatDataList", menuName = "SO/Combat/CombatDataList", order = 0)]
    public class CombatDataListSO : ScriptableObject
    {
        public List<CombatDataSOClass> combatDataList = new List<CombatDataSOClass>();
    }

    [Serializable]
    public class CombatDataSOClass
    {
        [InlineEditor] public List<CombatDataSO> combatDatas = new List<CombatDataSO>();
    }
}