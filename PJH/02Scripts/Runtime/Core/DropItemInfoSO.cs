using System.Collections.Generic;
using PJH.Runtime.PlayerPassive;
using UnityEngine;

namespace PJH.Runtime.Core
{
    [CreateAssetMenu(menuName = "SO/DropItemInfo")]
    public class DropItemInfoSO : ScriptableObject
    {
        [Range(0, 100)] public float passiveChancePercent;

        public float lowPassiveChancePercent = 10f, middlePassiveChancePercent = 50f;

        public bool CanHavePassive()
        {
            float value = Random.Range(0f, 100f);
            return value <= passiveChancePercent;
        }

        public PassiveRankType GetPassiveRank()
        {
            float randomValue = Random.Range(0f, 100f);

            if (randomValue <= lowPassiveChancePercent)
                return PassiveRankType.Low;
            if (randomValue <= lowPassiveChancePercent + middlePassiveChancePercent)
                return PassiveRankType.Middle;
            return PassiveRankType.High;
        }
    }
}