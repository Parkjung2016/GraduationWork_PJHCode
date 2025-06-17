using System;
using UnityEngine;

namespace PJH.Runtime.Players
{
    [CreateAssetMenu(menuName = "SO/CommandAction/DefaultAction", order = 0)]
    public class DefaultCommandActionDataSO : ScriptableObject
    {
        public CommandActionData commandActionData;
    }
}