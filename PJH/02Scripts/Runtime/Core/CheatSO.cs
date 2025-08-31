using BIS.Data;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PJH.Runtime.Core
{
    [CreateAssetMenu(menuName = "SO/Cheat")]
    public class CheatSO : ScriptableObject
    {
        public bool enableCheat = true;
        public int earnGold = 5000;
        [ReadOnly] public CurrencySO money;

        public void Update()
        {
            if (!enableCheat) return;
            if (Keyboard.current.pKey.wasPressedThisFrame)
                money.AddAmmount(earnGold);
        }
    }
}