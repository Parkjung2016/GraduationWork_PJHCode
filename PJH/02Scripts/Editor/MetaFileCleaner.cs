using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class MetaFileCleaner : EditorWindow
{
    // [MenuItem("Tools/문제 .meta 파일 삭제")]
    public static void DeleteBrokenMetaFiles()
    {
        string[] lines = new[]
       {
    "Assets/00Work/BIS/@Resources/Fonts/NanumSquareNeo-bRg SDF.asset.meta",
    "Assets/00Work/BIS/@Resources/Prefabs/ItemBox.prefab.meta",
    "Assets/00Work/BIS/@Resources/Prefabs/NPC/Trader.prefab.meta",
    "Assets/00Work/BIS/@Resources/Prefabs/Props.meta",
    "Assets/00Work/BIS/@Resources/Prefabs/Props/DropItem.prefab.meta",
    "Assets/00Work/BIS/@Resources/Prefabs/SaveHandler/SaveHandler.prefab.meta",
    "Assets/00Work/BIS/@Resources/Prefabs/UI/Combo/PickupPassiveFrame.prefab.meta",
    "Assets/00Work/BIS/@Resources/Prefabs/UI/ComboPickupPopupUI.meta",
    "Assets/00Work/BIS/@Resources/Prefabs/UI/ComboPickupPopupUI/DropItemPickupPopupUI.prefab.meta",
    "Assets/00Work/BIS/@Resources/Prefabs/UI/ComboPickupPopupUI/PickupPopupUI.prefab.meta",
    "Assets/00Work/BIS/@Resources/Prefabs/UI/CurrentEquipCombo.meta",
    "Assets/00Work/BIS/@Resources/Prefabs/UI/CurrentEquipCombo/ComboContain.prefab.meta",
    "Assets/00Work/BIS/@Resources/Prefabs/UI/EnemyPreviewPopupUI/Enemy_Idx1 1.prefab.meta",
    "Assets/00Work/BIS/@Resources/Prefabs/UI/EnemyPreviewPopupUI/Enemy_Idx1.prefab.meta",
    "Assets/00Work/BIS/@Resources/Prefabs/UI/EnemyPreviewPopupUI/EnemyPreviewChoiceUI.ver.prefab.meta",
    "Assets/00Work/BIS/@Resources/Prefabs/UI/EnemyPreviewPopupUI/EnemyPreviewPopupUI.ver.prefab.meta",
    "Assets/00Work/BIS/@Resources/Prefabs/UI/EnemyPreviewPopupUI/NewEnemyPreviewPopupUI.prefab.meta",
    "Assets/00Work/BIS/@Resources/Prefabs/UI/EnemyPreviewPopupUI/Star.prefab.meta",
    "Assets/00Work/BIS/@Resources/Prefabs/UI/EquipCombo.meta",
    "Assets/00Work/BIS/@Resources/Prefabs/UI/EquipCombo/CombinePieceOne_Image.prefab.meta",
    "Assets/00Work/BIS/@Resources/Prefabs/UI/EquipCombo/ComboSlotUI.prefab.meta",
    "Assets/00Work/BIS/@Resources/Prefabs/UI/HealthUI.meta",
    "Assets/00Work/BIS/@Resources/Prefabs/UI/Menu.meta",
    "Assets/00Work/BIS/@Resources/Prefabs/UI/Option.meta",
    "Assets/00Work/BIS/@Resources/Prefabs/UI/Tutorial/KeyInfoCanvas.prefab.meta",
    "Assets/00Work/BIS/@Resources/Prefabs/VFX.meta",
    "Assets/00Work/BIS/@Resources/Shaders.meta",
    "Assets/00Work/BIS/@Resources/SO/Acjoevement.meta",
    "Assets/00Work/BIS/@Resources/SO/Confirmation/CombineComboDifferent.asset.meta",
    "Assets/00Work/BIS/@Resources/SO/Confirmation/CombineNull.asset.meta",
    "Assets/00Work/BIS/@Resources/SO/Confirmation/CombineSame.asset.meta",
    "Assets/00Work/BIS/@Resources/SO/Currency.meta",
    "Assets/00Work/BIS/@Resources/SO/CurrentEquipComboSO.meta",
    "Assets/00Work/BIS/@Resources/SO/Dialogue/Lobby/IsGameFirstPlayer.asset.meta",
    "Assets/00Work/BIS/@Resources/SO/ENemyParty.meta",
    "Assets/00Work/BIS/@Resources/SO/ENemyParty/Test.asset.meta",
    "Assets/00Work/BIS/@Resources/SO/Inventory.meta",
    "Assets/00Work/BIS/@Resources/SO/SaveID.meta",
    "Assets/00Work/BIS/@Resources/SO/SaveID/Sound.meta",
    "Assets/00Work/BIS/@Resources/SO/TableSO.meta",
    "Assets/00Work/BIS/@Scenes/BaekGameScene.unity.meta",
    "Assets/00Work/BIS/@Scenes/BaekTItleScene.unity.meta",
    "Assets/00Work/BIS/@Scripts/Datas/Structs.meta",
    "Assets/00Work/BIS/@Scripts/UI/UGUI/PopupUI/ComboEquipPopupUI.meta",
    "Assets/00Work/BIS/@Scripts/UI/UGUI/PopupUI/ComboPopupUI.meta",
    "Assets/00Work/BIS/@Scripts/UI/UGUI/PopupUI/ComboSynthesisPopupUI.meta",
    "Assets/00Work/BIS/@Scripts/UI/UGUI/PopupUI/MenuPopupUI.meta",
    "Assets/00Work/BIS/@Scripts/UI/UGUI/PopupUI/Option.meta",
    "Assets/00Work/BIS/@Scripts/UI/UGUI/PopupUI/PickupPopupUI.meta",
    "Assets/00Work/KHJ/00.Scene/KHJDialogue.unity.meta",
    "Assets/00Work/KHJ/00.Scene/KHJInGameScene.unity.meta",
    "Assets/00Work/KHJ/00.Scene/KHJScene.unity.meta",
    "Assets/00Work/KHJ/01.Script.meta",
    "Assets/00Work/KHJ/01.Script/Core/KHJ.Core.asmdef.meta",
    "Assets/00Work/KHJ/11.UI/MiniMap.meta",
    "Assets/00Work/KHJ/CutScene Global Volume Profile 3_1.asset.meta",
    "Assets/00Work/KHJ/CutScenePP.asset.meta",
    "Assets/00Work/KHJ/LOVE_Inseong_PP.asset.meta",
    "Assets/00Work/LJS/00_Scene/BattleScene_LJS_1.unity.meta",
    "Assets/00Work/LJS/00_Scene/BattleScene_LJS_2/LightingData.asset.meta",
    "Assets/00Work/LJS/00_Scene/LJSExuteTestScene.unity.meta",
    "Assets/00Work/LJS/00_Scene/LJSMapTestScene.meta",
    "Assets/00Work/LJS/00_Scene/Test_LJSScene/NavMesh-Ground.asset.meta",
    "Assets/00Work/LJS/01_Script/Entity/Boss/FightSystem.meta",
    "Assets/00Work/LJS/01_Script/NPC.meta",
    "Assets/00Work/LJS/01_Script/UI.meta",
    "Assets/00Work/LJS/02_Animation/Boss/Clip/Kick.meta",
    "Assets/00Work/LJS/03.Prefab/BaseRoom.prefab.meta",
    "Assets/00Work/LJS/03.Prefab/BSP/BSPTest.prefab.meta",
    "Assets/00Work/LJS/03.Prefab/BSP/Room/Normal/BossRoom.prefab.meta",
    "Assets/00Work/LJS/03.Prefab/BSP/Room/Normal/TestRoom 4.prefab.meta",
    "Assets/00Work/LJS/03.Prefab/BSP/Room/Special/BossRoom.prefab.meta",
    "Assets/00Work/LJS/03.Prefab/BSP/Room/Special/ChoiceRoom.prefab.meta",
    "Assets/00Work/LJS/03.Prefab/BSP/Room/Special/HealingRoom.prefab.meta",
    "Assets/00Work/LJS/03.Prefab/BSP/Room/Special/ShopRoom.prefab.meta",
    "Assets/00Work/LJS/03.Prefab/NPC.meta",
    "Assets/00Work/LJS/05_SO/AnimParam.meta",
    "Assets/00Work/LJS/05_SO/AnimParam/Boolean_HayMakerParam.asset.meta",
    "Assets/00Work/LJS/05_SO/AnimParam/Boolean_HitParam.asset.meta",
    "Assets/00Work/LJS/05_SO/BSP.meta",
    "Assets/00Work/LJS/05_SO/BSP/SpecialRoomTableSO.asset.meta",
    "Assets/00Work/LJS/05_SO/Combat.meta",
    "Assets/00Work/LJS/05_SO/Combat/Hit.meta",
    "Assets/00Work/LJS/05_SO/Item/ItemEffect.meta",
    "Assets/00Work/LJS/05_SO/Item/ItemTable/AllItemDataTable.asset.meta",
    "Assets/00Work/LJS/05_SO/Item/RandomValue.meta",
    "Assets/00Work/LJS/05_SO/SpecialMovement.meta",
    "Assets/00Work/LJS/05_SO/Stat.meta",
    "Assets/00Work/LJS/99.Asset/Scenes/BSPTestScene.unity.meta",
    "Assets/00Work/LJS/Resources.meta",
    "Assets/00Work/PJH/02Scripts/Editor.meta",
    "Assets/00Work/PJH/02Scripts/Runtime/Core/MotionTrail.meta",
    "Assets/00Work/PJH/02Scripts/Runtime/PlayerPassive/Passives.meta",
    "Assets/00Work/PJH/02Scripts/Runtime/TrainingScarecrow.meta",
    "Assets/00Work/PJH/03Animations/Extra.meta",
    "Assets/00Work/PJH/05SO/AnimancerEvents/CloneHitTargetEvent.asset.meta",
    "Assets/00Work/PJH/05SO/AnimancerEvents/PlayArmBreakSoundEvent.asset.meta",
    "Assets/00Work/PJH/05SO/CommandActionPieces/Command Action Piece_Combo_02_1 1.asset.meta",
    "Assets/00Work/PJH/05SO/CommandActionPieces/Command Action Piece_Combo_02_1.asset.meta",
    "Assets/00Work/PJH/05SO/CommandActions.meta",
    "Assets/00Work/PJH/05SO/CommandActions/CombatData.meta",
    "Assets/00Work/PJH/07Prefabs/TrainingScarecrowGroup.prefab.meta",
    "Assets/00Work/PJH/07Prefabs/UI/EvasionInfoCanvas.prefab.meta",
    "Assets/00Work/PJH/07Prefabs/UI/PassiveInfo.meta",
    "Assets/00Work/PJH/08Textures/ComboPieceIcon.meta",
    "Assets/00Work/PJH/10Models.meta",
    "Assets/00Work/YTH/01Scripts/Runtime/BT/Action/Combat.meta",
    "Assets/00Work/YTH/01Scripts/Runtime/SO.meta",
    "Assets/00Work/YTH/01Scripts/Shared.meta",
    "Assets/00Work/YTH/02Animations/NormalEnemy_2.controller.meta",
    "Assets/00Work/YTH/02Animations/NormalEnemy_3.controller.meta",
    "Assets/00Work/YTH/03Prefabs/AnimationVisual.prefab.meta",
    "Assets/00Work/YTH/03Prefabs/Equipments.meta",
    "Assets/00Work/YTH/03Prefabs/TutoralEnemy.prefab.meta",
    "Assets/00Work/YTH/05SO/Animation/Param/Bool.meta",
    "Assets/00Work/YTH/05SO/ComatData_Old/BOXER/BOX_Squad_Punch.asset.meta",
    "Assets/00Work/YTH/05SO/ComatData_Old/BOXER/Finish.meta"
};

        List<string> deleted = new List<string>();

        foreach (var path in lines)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
                deleted.Add(path);
                Debug.Log($"?? 삭제됨: {path}");
            }
            else
            {
                Debug.LogWarning($"[건너뜀] 없음: {path}");
            }
        }

        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("완료", $"{deleted.Count}개의 .meta 파일이 삭제되었습니다.\nUnity가 다시 생성할 것입니다.", "확인");
    }
}
