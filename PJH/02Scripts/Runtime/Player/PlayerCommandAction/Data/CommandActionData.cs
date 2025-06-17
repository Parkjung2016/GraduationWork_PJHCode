using System;
using System.Collections.Generic;
using Main.Shared;
using UnityEngine;

namespace PJH.Runtime.Players
{
    [Serializable]
    public class CommandActionData
    {
        [field: SerializeField]
        public List<CommandActionPieceSO> ExecuteCommandActionPieces { get; private set; } = new();

        public int maxActionCount = 5;
        

        public bool TryAddCommandActionPiece(CommandActionPieceSO commandActionPiece)
        {
            if (ExecuteCommandActionPieces.Count >= maxActionCount) return false;
            ExecuteCommandActionPieces.Add(commandActionPiece);
            return true;
        }

        public bool TryRemoveCommandActionPiece(CommandActionPieceSO commandActionPiece)
        {
            if (ExecuteCommandActionPieces.Remove(commandActionPiece))
            {
                return true;
            }

            return false;
        }

        public void Equip(IPlayer player)
        {
            ExecuteCommandActionPieces.ForEach(x => x.EquipPiece(player));
        }

        public void UnEquip()
        {
            ExecuteCommandActionPieces.ForEach(x => x.UnEquipPiece());
        }

        public void ClearPieces()
        {
            UnEquip();
            ExecuteCommandActionPieces.Clear();
        }
    }
}