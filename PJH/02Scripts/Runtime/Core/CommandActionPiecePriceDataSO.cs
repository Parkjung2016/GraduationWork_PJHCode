using System.Collections.Generic;
using PJH.Runtime.PlayerPassive;
using PJH.Runtime.Players;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PJH.Runtime.Core
{
    [CreateAssetMenu(menuName = "SO/CommandAction/PriceCondig")]
    public class CommandActionPiecePriceConfigSO : SerializedScriptableObject
    {
        public int defaultPrice = 500;

        public int additionalPriceForKnockDown = 200;
        public Dictionary<PassiveRankType, int> additionalPriceByRank = new();

        public int GetPrice(CommandActionPieceSO piece)
        {
            int price = defaultPrice;
            int addtionalPrice = 0;
            for (int i = 0; i < piece.Passives.Count; i++)
            {
                addtionalPrice += additionalPriceByRank[piece.Passives[i].RankType];
            }

            if (piece.combatData.isKnockDown)
            {
                addtionalPrice += additionalPriceForKnockDown;
            }

            price += addtionalPrice;
            return price;
        }
    }
}