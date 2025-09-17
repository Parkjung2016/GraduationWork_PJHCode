using System.Collections.Generic;
using PJH.Runtime.PlayerPassive;
using PJH.Runtime.Players;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PJH.Runtime.Core
{
    [CreateAssetMenu(menuName = "SO/SellComboPieceInfo")]
    public class SellComboPieceInfoSO : SerializedScriptableObject
    {
        public int baseSellPrice;
        public int knockdownBonusSellPrice;

        public Dictionary<PassiveRankType, int> rankSellPriceBonus;
        public List<float> passiveCountSellPricePercents;

        public int GetSellPrice(CommandActionPieceSO commandActionPiece)
        {
            int sellPrice = baseSellPrice;
            bool isKnockDown = commandActionPiece.combatData.isKnockDown;
            if (isKnockDown)
                sellPrice += knockdownBonusSellPrice;
            int passiveCount = commandActionPiece.Passives.Count;
            if (passiveCount > 0)
            {
                foreach (PassiveSO passive in commandActionPiece.Passives)
                {
                    PassiveRankType rankType = passive.RankType;
                    int bonus = rankSellPriceBonus[rankType];
                    sellPrice += bonus;
                }

                float increaseSellPricePercent = passiveCountSellPricePercents[passiveCount - 1];
                sellPrice = Mathf.RoundToInt(sellPrice * (1f + increaseSellPricePercent * 0.01f));
            }

            return sellPrice;
        }
    }
}