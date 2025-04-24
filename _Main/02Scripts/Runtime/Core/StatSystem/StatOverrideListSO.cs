using System.Collections.Generic;
using UnityEngine;

namespace Main.Runtime.Core.StatSystem
{
    [CreateAssetMenu(fileName = "StatOverrideListSO", menuName = "SO/StatSystem/StatOverrideList")]
    public class StatOverrideListSO : ScriptableObject
    {
        public List<StatOverride> StatOverrides = new();
    }
}