using System.IO;
using PJH.Runtime.Players;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace PJH.Editor
{
    public class PlayerPassiveManagerWindow : OdinMenuEditorWindow
    {
        public static readonly string cloneAttackCombatDataSOPath =
            "Assets/00Work/PJH/05SO/PlayerPassives/CloneAttack/CombatDatas";

        public static readonly string cloneAttackCombatDataDatabasePath =
            "Assets/00Work/PJH/05SO/PlayerPassives/CloneAttack/CloneAttackCombatDatabase.asset";

        private enum MainMenuTab
        {
            CloneAttack,
        }

        private MainMenuTab selectedTab = MainMenuTab.CloneAttack;
        private MainMenuTab prevTab;

        protected override void OnBeginDrawEditors()
        {
            if (MenuTree == null) return;
            GUILayout.Space(10);
            SirenixEditorGUI.BeginHorizontalToolbar();
            {
                prevTab = (MainMenuTab)GUILayout.Toolbar((int)selectedTab,
                    System.Enum.GetNames(typeof(MainMenuTab)));
                GUILayout.Space(10);
                if (prevTab != selectedTab)
                {
                    selectedTab = prevTab;
                    ForceMenuTreeRebuild();
                }
            }
            SirenixEditorGUI.EndHorizontalToolbar();

            switch (selectedTab)
            {
                case MainMenuTab.CloneAttack:
                    ShowCloneAttackCombatDataSOCommandButton();
                    break;
            }

            MenuTree.DrawSearchToolbar();
        }

        private void ShowCloneAttackCombatDataSOCommandButton()
        {
            SirenixEditorGUI.BeginHorizontalToolbar();
            {
                GUILayout.FlexibleSpace();
                if (SirenixEditorGUI.ToolbarButton("Create Asset"))
                {
                    if (!Directory.Exists(cloneAttackCombatDataSOPath))
                    {
                        return;
                    }

                    var asset = CreateInstance<PlayerCombatDataSO>();
                    string assetPath = Path.Combine(cloneAttackCombatDataSOPath, $"NewCombatDataSO.asset");

                    AssetDatabase.CreateAsset(asset, assetPath);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    TrySelectMenuItemWithObject(asset);
                    Debug.Log($"생성 완료: {assetPath}");
                }

                if (MenuTree.Selection.SelectedValue is PlayerCombatDataSO combatData)
                {
                    if (SirenixEditorGUI.ToolbarButton("Delete Asset"))
                    {
                        string assetPath = AssetDatabase.GetAssetPath(combatData);
                        AssetDatabase.DeleteAsset(assetPath);
                        AssetDatabase.Refresh();

                        Debug.Log("삭제 완료");
                    }
                }
            }
            SirenixEditorGUI.EndHorizontalToolbar();
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            OdinMenuTree tree = new();
            tree.Selection.SupportsMultiSelect = false;
            switch (selectedTab)
            {
                case MainMenuTab.CloneAttack:
                    tree.AddAssetAtPath("Database", cloneAttackCombatDataDatabasePath);
                    tree.AddAllAssetsAtPath("SOList", cloneAttackCombatDataSOPath);
                    break;
            }

            return tree;
        }
    }
}