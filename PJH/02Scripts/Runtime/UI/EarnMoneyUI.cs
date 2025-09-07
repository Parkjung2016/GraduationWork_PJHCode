using System;
using BIS.Data;
using DamageNumbersPro;
using PJH.Utility.Managers;
using UnityEngine;

namespace PJH.Runtime.UI
{
    public class EarnMoneyUI : MonoBehaviour
    {
        private CurrencySO _money;

        [SerializeField] private DamageNumber _earnMoneyText;

        private RectTransform _earnMoneyTextPoint;

        private void Awake()
        {
            _earnMoneyTextPoint = transform.Find("EarnMoneyTextPoint") as RectTransform;
            _money = AddressableManager.Load<CurrencySO>("Money");
            _money.AddAmountEvent += HandleAddAmount;
        }

        private void OnDestroy()
        {
            _money.AddAmountEvent -= HandleAddAmount;
        }

        private void HandleAddAmount(int amount)
        {
            _earnMoneyText.SpawnGUI(_earnMoneyTextPoint, Vector2.zero, amount);
        }
    }
}