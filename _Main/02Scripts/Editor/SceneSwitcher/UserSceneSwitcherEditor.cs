#if UNITY_EDITOR
using PJH.SceneSwitcher;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using UnityToolbarExtender;
using OdinMenuTree = Sirenix.OdinInspector.Editor.OdinMenuTree;

namespace PJH
{
    [InitializeOnLoad]
    public class SceneSwitchRightButton
    {
        static SceneSwitchRightButton()
        {
            ToolbarExtender.RightToolbarGUI.Add(OnToolbarGUI);
        }

        static void OnToolbarGUI()
        {
            if (GUILayout.Button(new GUIContent("ManageUserScene", "manage user scene switcher SO")))
            {
                EditorWindow.GetWindow<UserSceneSwitcherEditor>().Show();
            }
        }
    }

    public class UserSceneSwitcherEditor : OdinMenuEditorWindow
    {
        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree();

            tree.Add("Create New", new CreateNewUserSceneSwitcherSO());
            tree.AddAllAssetsAtPath("User Scene Switcher", "Assets/00Work/_Main/05SO/Editor/SceneSwitcher",
                typeof(UserSceneSwitcherSO));
            return tree;
        }


        protected override void OnBeginDrawEditors()
        {
            OdinMenuTreeSelection selected = MenuTree.Selection;

            UserSceneSwitcherSO asset = selected.SelectedValue as UserSceneSwitcherSO;
            if (!asset) return;
            SirenixEditorGUI.BeginHorizontalToolbar();
            {
                GUILayout.FlexibleSpace();

                if (SirenixEditorGUI.ToolbarButton("Delete Current"))
                {
                    string path = AssetDatabase.GetAssetPath(asset);
                    AssetDatabase.DeleteAsset(path);
                    AssetDatabase.SaveAssets();
                }
            }
            SirenixEditorGUI.EndHorizontalToolbar();
        }
    }

    public class CreateNewUserSceneSwitcherSO
    {
        public CreateNewUserSceneSwitcherSO()
        {
            userSceneSwitcher = ScriptableObject.CreateInstance<UserSceneSwitcherSO>();
            userSceneSwitcher.userName = "NewUserSceneSwitcher";
        }

        [InlineEditor(InlineEditorObjectFieldModes.Hidden)]
        public UserSceneSwitcherSO userSceneSwitcher;

        [Button]
        private void CreateNewSO()
        {
            AssetDatabase.CreateAsset(userSceneSwitcher,
                "Assets/00Work/_Main/05SO/Editor/SceneSwitcher/" + userSceneSwitcher.userName + ".asset");
            AssetDatabase.SaveAssets();
        }
    }
}
#endif