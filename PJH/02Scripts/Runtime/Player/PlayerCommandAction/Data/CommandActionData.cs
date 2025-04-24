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

        private int _maxActionCount = 5;

        public bool TryAddCommandActionPiece(CommandActionPieceSO commandActionPiece)
        {
            if (ExecuteCommandActionPieces.Count >= _maxActionCount) return false;
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

        public void Init(IPlayer player)
        {
            ExecuteCommandActionPieces.ForEach(x => x.InitPassive(player));
        }
    }
}