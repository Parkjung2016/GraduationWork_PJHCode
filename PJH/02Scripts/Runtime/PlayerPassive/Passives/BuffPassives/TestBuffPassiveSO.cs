using System;
using Main.Shared;
using PJH.Runtime.Players;
using UnityEngine;

namespace PJH.Runtime.PlayerPassive.Passives
{
    [CreateAssetMenu(menuName = "SO/Passive/Buff/TestBuffPassive")]
    public class TestBuffPassiveSO : PassiveSO
    {
        private Player _player;


        public override void EquipPiece(IPlayer player)
        {
            base.EquipPiece(player);
            Debug.Log("TestBuffPassiveSO EquipPiece called");
        }
    }
}