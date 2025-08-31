#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
//using UnityToolbarExtender;

namespace PJH.SceneSwitcher
{
    static class ToolbarStyles
    {
        public static readonly GUIStyle commandButtonStyle;

        static ToolbarStyles()
        {
            commandButtonStyle = new GUIStyle("Command")
            {
                fontSize = 16,
                alignment = TextAnchor.MiddleCenter,
                imagePosition = ImagePosition.ImageAbove,
                fontStyle = FontStyle.Bold
            };
        }
    }

    [InitializeOnLoad]
    public class SceneSwitchLeftButton
    {
        private static UserSceneSwitcherSOList userSceneSwitcherList;

        private static SceneAsset mainScene;
        private static SceneAsset titleScene;
        static string selectedUser;
        static SceneAsset selectedScene;

        static SceneSwitchLeftButton()
        {
            mainScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(
                "Assets/00Work/_Main/01Scene/Lobby.unity");
            titleScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(
                "Assets/00Work/_Main/01Scene/Title.unity");
            userSceneSwitcherList =
                AssetDatabase.LoadAssetAtPath<UserSceneSwitcherSOList>(
                    "Assets/00Work/_Main/05SO/Editor/UserSceneSwitcherList.asset");
            //ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUI);
        }

        static void OnToolbarGUI()
        {
            GUILayout.FlexibleSpace();

            if (GUILayout.Button(new GUIContent("Title", "Title Scene")))
            {
                SceneHelper.OpenScene(titleScene);
            }

            if (GUILayout.Button(new GUIContent("MainGame", "MainGame Scene")))
            {
                SceneHelper.OpenScene(mainScene);
            }

            if (GUILayout.Button(selectedUser ?? "사용자 선택", EditorStyles.popup))
            {
                var menu = new GenericMenu();
                foreach (var user in userSceneSwitcherList.UserSceneSwitcherList)
                {
                    menu.AddItem(new GUIContent(user.userName), false, OnUserSelected, user.userName);
                }

                menu.ShowAsContext();
            }

            if (!string.IsNullOrEmpty(selectedUser) &&
                GUILayout.Button(
                    new GUIContent((selectedScene == null ? "씬 선택" : selectedScene.name), "Open User Scene"),
                    EditorStyles.popup))
            {
                var userScenesMenu = new GenericMenu();
                var userSceneNames = userSceneSwitcherList.UserSceneSwitcherList.Find(x => x.userName == selectedUser)
                    .userSceneNames;
                foreach (var scene in userSceneNames)
                {
                    userScenesMenu.AddItem(new GUIContent(scene.name), false, OnSceneSelected, scene);
                }

                userScenesMenu.ShowAsContext();
            }
        }

        static void OnUserSelected(object user)
        {
            selectedUser = (string)user;
            selectedScene = null;
        }

        static void OnSceneSelected(object scene)
        {
            selectedScene = (SceneAsset)scene;
            SceneHelper.OpenScene(selectedScene);
        }
    }

    static class SceneHelper
    {
        public static void OpenScene(SceneAsset scene)
        {
            var saved = EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            if (saved)
            {
                EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(scene));
            }
        }
    }
}
#endif