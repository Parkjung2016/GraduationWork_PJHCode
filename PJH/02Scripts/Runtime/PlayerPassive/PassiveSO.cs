using PJH.Shared;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PJH.Runtime.PlayerPassive
{
    public enum PassiveRankType
    {
        Low,
        Middle,
        Advanced
    }

    public abstract class PassiveSO : PlayerPieceSO
    {
        [field: SerializeField]
        [field: LabelText("µî±Þ")]
        [field: EnumToggleButtons]
        public PassiveRankType RankType { get; private set; }
    }
}