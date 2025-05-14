using Main.Shared;
using UnityEngine;

namespace PJH.Runtime.PlayerPassive
{
    public abstract class PassiveSO : ScriptableObject
    {
        public string displayName;
        public string description;
        public Sprite icon;

        public abstract void Init(IPlayer player);

        public abstract void ActivePassive();

        public abstract void DeactivePassive();
    }
}