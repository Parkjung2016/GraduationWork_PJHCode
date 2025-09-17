using System.Collections.Generic;
using UnityEngine;

namespace PJH.Runtime.Core
{
    [CreateAssetMenu(menuName = "SO/ThemeSetting")]
    public class ThemeSettingSO : ScriptableObject
    {
        public int maxThemeCount = 2;

        public Dictionary<string, bool> clearedTheme;
        public bool isShowedEnding;

        private void OnEnable()
        {
            clearedTheme = new Dictionary<string, bool>(maxThemeCount);
        }
    }
}