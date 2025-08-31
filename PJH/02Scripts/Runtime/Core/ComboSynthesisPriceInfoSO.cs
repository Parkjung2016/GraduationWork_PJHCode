using UnityEngine;

namespace PJH.Runtime.Core
{
    [CreateAssetMenu(menuName = "SO/ComboSynthesisPriceInfo")]
    public class ComboSynthesisPriceInfoSO : ScriptableObject
    {
        public int defaultSynthesisPrice = 2000;
        public float synthesisPriceIncreasePercent = 20;

        public int increaseLevel = 0;

        public int GetPrice()
        {
            int price = defaultSynthesisPrice;
            if (increaseLevel > 0)
            {
                float increaseMultiplier = 1 + (synthesisPriceIncreasePercent / 100f) * increaseLevel;
                price = Mathf.RoundToInt(defaultSynthesisPrice * increaseMultiplier);
            }

            return price;
        }

        public void IncreaseLevel()
        {
            increaseLevel++;
        }

        public void DecreaseLevel()
        {
            increaseLevel--;
        }

        public void ResetIncreaseLevel()
        {
            increaseLevel = 0;
        }
    }
}