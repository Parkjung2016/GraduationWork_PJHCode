#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PJH.SceneSwitcher
{
    [CreateAssetMenu(fileName = "UserSceneSwitcherSO", menuName = "SO/Scene Switcher/User")]
    public class UserSceneSwitcherSO : ScriptableObject
    {
        public string userName;
        public List<SceneAsset> userSceneNames = new();
    }
}
#endif