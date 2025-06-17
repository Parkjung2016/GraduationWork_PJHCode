using System;
using System.Collections.Generic;
using System.Linq;
using PJH.Runtime.Players;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PJH.Runtime.PlayerPassive.Passives
{
    [Serializable]
    public struct CloneAttackCombatData
    {
        public PlayerCombatDataSO combatData;
        public float additionalCloneOffset;
    }

    [CreateAssetMenu(menuName = "SO/Passive/Active/CloneAttackCombatDatabase")]
    public class CloneAttackCombatDatabase : SerializedScriptableObject
    {
        [SerializeField] private Dictionary<PlayerCombatDataSO, CloneAttackCombatData> _attackCombats = new();

        public CloneAttackCombatData this[PlayerCombatDataSO key] =>
            _attackCombats.FirstOrDefault(pair => key.name.Contains(pair.Key.name)).Value;
    }
}