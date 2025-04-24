namespace Main.Runtime.Core.Events
{
    public static partial class GameEvents
    {
        public static readonly SceneChangeEvent SceneChangeEvent = new SceneChangeEvent();
    }

    public class SceneChangeEvent : GameEvent
    {
        public string changeSceneName;
    }
}