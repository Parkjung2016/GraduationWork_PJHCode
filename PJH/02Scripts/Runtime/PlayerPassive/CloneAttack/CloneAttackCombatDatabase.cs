using System.Collections.Generic;
using PJH.Runtime.Players;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PJH.Runtime.PlayerPassive
{
    [CreateAssetMenu(menuName = "SO/Passive/CloneAttackCombatDatabase")]
    public class CloneAttackCombatDatabase : SerializedScriptableObject
    {
        [SerializeField] private Dictionary<PlayerCombatDataSO, PlayerCombatDataSO> _attackCombats = new();

        public PlayerCombatDataSO this[PlayerCombatDataSO key] => _attackCombats[key];
    }
}