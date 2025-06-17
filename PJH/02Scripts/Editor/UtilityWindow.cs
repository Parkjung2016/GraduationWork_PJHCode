#if UNITY_EDITOR
using System.IO;
using Main.Runtime.Combat;
using PJH.Runtime.Players;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace PJH.Editor
{
    public class UtilityWindow : OdinMenuEditorWindow
    {
        public static readonly string commandActionPieceSOPath = "Assets/00Work/PJH/05SO/CommandActionPieces";
        public static readonly string combatDataSOPath = "Assets/00Work/PJH/05SO/CommandActions/CombatData";

        private enum MainMenuTab
        {
            CommandActionPiece,
            CommandAction,
        }

        [MenuItem("Tools/PJH/Utility")]
        private static void OpenWindow()
        {
            var PassiveManagerEditorWindow =
                GetWindow<PlayerPassiveManagerWindow>();
            PassiveManagerEditorWindow.titleContent = new GUIContent("PassiveManager");
            PassiveManagerEditorWindow.Show();
            var utilityWindow = GetWindow<UtilityWindow>(desiredDockNextTo: typeof(PlayerPassiveManagerWindow));
            utilityWindow.titleContent = new GUIContent("Utility");
            utilityWindow.Show();
        }

        private MainMenuTab selectedTab = MainMenuTab.CommandActionPiece;
        private MainMenuTab prevTab;

        protected override void OnBeginDrawEditors()
        {
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
                case MainMenuTab.CommandActionPiece:
                    ShowCommandActionPieceSOCommandButton();
                    break;
                case MainMenuTab.CommandAction:
                    ShowCommandActionCommandButton();
                    break;
            }

            MenuTree.DrawSearchToolbar();
        }

        private void ShowCommandActionPieceSOCommandButton()
        {
            SirenixEditorGUI.BeginHorizontalToolbar();
            {
                GUILayout.FlexibleSpace();
                if (SirenixEditorGUI.ToolbarButton("Create Asset"))
                {
                    if (!Directory.Exists(commandActionPieceSOPath))
                    {
                        return;
                    }

                    var asset = CreateInstance<CommandActionPieceSO>();
                    string assetPath = Path.Combine(commandActionPieceSOPath, $"NewCommandActionPieceSO.asset");

                    AssetDatabase.CreateAsset(asset, assetPath);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    TrySelectMenuItemWithObject(asset);
                    Debug.Log($"생성 완료: {assetPath}");
                }

                if (MenuTree.Selection.SelectedValue is CommandActionPieceSO commandActionPieceSO)
                {
                    if (SirenixEditorGUI.ToolbarButton("Delete Asset"))
                    {
                        string assetPath = AssetDatabase.GetAssetPath(commandActionPieceSO);
                        AssetDatabase.DeleteAsset(assetPath);
                        AssetDatabase.Refresh();
                        Debug.Log("삭제 완료");
                    }
                }
            }
            SirenixEditorGUI.EndHorizontalToolbar();
        }

        private void ShowCommandActionCommandButton()
        {
            if (MenuTree == null) return;

            SirenixEditorGUI.BeginHorizontalToolbar();
            {
                GUILayout.FlexibleSpace();
                if (SirenixEditorGUI.ToolbarButton("Create Asset"))
                {
                    if (!Directory.Exists(combatDataSOPath))
                    {
                        return;
                    }

                    var asset = CreateInstance<PlayerCombatDataSO>();
                    string assetPath = Path.Combine(combatDataSOPath, $"NewCombatDataSO.asset");

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
                case MainMenuTab.CommandActionPiece:
                    tree.AddAllAssetsAtPath("SOList", commandActionPieceSOPath,
                            typeof(CommandActionPieceSO))
                        .AddIcons(item => (item.Value as CommandActionPieceSO)?.pieceIcon).ForEach(item =>
                        {
                            item.Name = item.Name.Replace("Command Action Piece_", "");
                        });
                    break;
                case MainMenuTab.CommandAction:
                    tree.AddAllAssetsAtPath("SOList", combatDataSOPath,
                        typeof(PlayerCombatDataSO));
                    break;
            }

            return tree;
        }
    }
}
#endif