using System.Collections.Generic;

namespace Main.Runtime.Manager
{
    public static class Managers
    {
        public const int maxThemeCount = 2;
        public static FMODManager FMODManager { get; } = new();
        public static VolumeManager VolumeManager { get; } = new();
        public static TimeManager TimeManager { get; } = new();
        public static Dictionary<string, bool> clearedTheme = new Dictionary<string, bool>(maxThemeCount);
    }
}