using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace PJH.SceneSwitcher
{
    [CreateAssetMenu(fileName = "UserSceneSwitcherList", menuName = "SO/Scene Switcher/UserList")]
    public class UserSceneSwitcherSOList : ScriptableObject
    {
        public List<UserSceneSwitcherSO> UserSceneSwitcherList = new();


        private void OnValidate()
        {
            UserSceneSwitcherList.Clear();
            string[] guids =
                AssetDatabase.FindAssets("", new string[] { "Assets/00Work/_Main/05SO/Editor/SceneSwitcher" });
            for (int i = 0; i < guids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                UserSceneSwitcherSO userSceneSwitcher =
                    (UserSceneSwitcherSO)AssetDatabase.LoadAssetAtPath(assetPath, typeof(UserSceneSwitcherSO));
                UserSceneSwitcherList.Add(userSceneSwitcher);
            }
        }
    }
}