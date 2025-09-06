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