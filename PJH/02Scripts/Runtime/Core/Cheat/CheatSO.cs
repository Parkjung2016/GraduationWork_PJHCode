using BIS.Data;
using Main.Runtime.Agents;
using Main.Runtime.Core.StatSystem;
using Main.Runtime.Manager;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PJH.Runtime.Core.Cheat
{
    [CreateAssetMenu(menuName = "SO/Cheat")]
    public class CheatSO : ScriptableObject
    {
        public bool enableCheat = true;
        public int earnGold = 5000;
        public StatSO powerStat;
        [ReadOnly] public CurrencySO money;

        public void Update()
        {
            if (!enableCheat) return;
            if (Keyboard.current.pKey.wasPressedThisFrame)
                money.AddAmmount(earnGold);
            // if (Keyboard.current.lKey.wasPressedThisFrame)
            // {
            //     PlayerManager.Instance.Player.GetCompo<AgentStat>(true)
            //         .AddValuePercentModifier(powerStat, "Cheat", 200);
            // }

        }
    }
}