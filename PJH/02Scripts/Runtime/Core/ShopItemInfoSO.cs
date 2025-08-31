using System;
using System.Collections.Generic;
using PJH.Runtime.PlayerPassive;
using PJH.Runtime.Players;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PJH.Runtime.Core
{
    [Serializable]
    public struct PassiveRankExtraPrice
    {
        public int minPrice, maxPrice;

        public PassiveRankExtraPrice(int minPrice, int maxPrice)
        {
            this.minPrice = minPrice;
            this.maxPrice = maxPrice;
        }

        public int GetPrice()
        {
            return Random.Range(minPrice, maxPrice + 1);
        }
    }

    [CreateAssetMenu(menuName = "SO/ShopItemInfo")]
    public class ShopItemInfoSO : SerializedScriptableObject
    {
        [Range(0, 100)] public float passiveChancePercent;
        public int defaultItemPriceMin = 3000, defaultItemPriceMax = 5000;
        public int knockDownExtraPrice = 1000;
        public int rerollPrice = 500;
        [ReadOnly] public bool opened;

        [ReadOnly] public List<CommandActionPieceSO> shopItems = new();
        [ReadOnly] public List<bool> isPurchased;
        [Range(0, 100)] public float appearMiddleRankPassivePercent = 30;
        [Range(0, 100)] public float appearLowRankPassivePercent = 50;

        public Dictionary<PassiveRankType, PassiveRankExtraPrice> passiveRankExtraPrices =
            new Dictionary<PassiveRankType, PassiveRankExtraPrice>
            {
                { PassiveRankType.Low, new PassiveRankExtraPrice(800, 1000) },
                { PassiveRankType.Middle, new PassiveRankExtraPrice(1300, 1800) },
                { PassiveRankType.High, new PassiveRankExtraPrice(2100, 2600) }
            };

        public void ResetInfo()
        {
            opened = false;
            ClearList();
        }

        public void ClearList()
        {
            shopItems.Clear();
            isPurchased.Clear();
        }

        public int GetPrice(CommandActionPieceSO commandActionPiece)
        {
            int price = Random.Range(defaultItemPriceMin, defaultItemPriceMax + 1);
            if (commandActionPiece.combatData.isKnockDown)
                price += knockDownExtraPrice;
            foreach (PassiveSO passive in commandActionPiece.Passives)
            {
                PassiveRankType rankType = passive.RankType;
                PassiveRankExtraPrice extraPrice = passiveRankExtraPrices[rankType];
                int passivePrice = extraPrice.GetPrice();
                price += passivePrice;
            }

            return price;
        }

        public bool CanHavePassive()
        {
            float value = Random.Range(0f, 100f);
            return value <= passiveChancePercent;
        }
    }
}