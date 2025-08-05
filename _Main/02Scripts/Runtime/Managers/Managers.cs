namespace Main.Runtime.Manager
{
    public static class Managers
    {
        public static FMODManager FMODManager { get; } = new();
        public static VolumeManager VolumeManager { get; } = new();
        public static TimeManager TimeManager { get; } = new();
    }
}