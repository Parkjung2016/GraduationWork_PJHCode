using Main.Runtime.Core.StatSystem;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace Main.Runtime.Core.Events
{
    public static partial class GameEvents
    {
        public static readonly OnRetreatEvent OnRetreat = new OnRetreatEvent();
    }
    
        
    public class OnRetreatEvent : GameEvent
    {
    }
}