using System.Collections.Generic;
using Main.Shared;
using PJH.Runtime.PlayerPassive;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace PJH.Runtime.Players
{
    [CreateAssetMenu(menuName = "SO/CommandAction/Piece")]
    public class CommandActionPieceSO : SerializedScriptableObject
    {
        [Title("📌 기본 정보", titleAlignment: TitleAlignments.Centered, bold: true)]
        [HorizontalGroup("Top", Width = 80)]
        [PreviewField(70), HideLabel]
        public Sprite pieceSprite;

        [VerticalGroup("Top/Right")] [OnValueChanged("OnChangeActionName"), Delayed] [LabelText("📝 이름")]
        public string actionName;

        [VerticalGroup("Top/Right")] [OnValueChanged("OnChangeActionName"), Delayed] [LabelText("📝 디스플레이 이름")]
        public string actionDisplay;

        [VerticalGroup("Top/Right")] [MultiLineProperty(3)] [LabelText("📖 설명")]
        public string actionDescription;

        [VerticalGroup("Top/Right")] [LabelText("⚔ 전투 데이터")]
        public PlayerCombatDataSO combatData;

        [BoxGroup("⚙ 패시브 설정", showLabel: true)] [LabelText("🌟 메인 패시브")] [SerializeField]
        public PassiveSO passive;

        [BoxGroup("⚙ 패시브 설정")]
        [LabelText("📚 중첩 패시브들")]
        [SerializeField, ReadOnly]
        [ListDrawerSettings(
            Expanded = true,
            DraggableItems = false,
            ShowPaging = false,
            ShowItemCount = true,
            NumberOfItemsPerPage = 5,
            ListElementLabelName = "name"
        )]
        public List<PassiveSO> overlappingPassives;

        public void InitPassive(IPlayer player)
        {
            (passive)?.Init(player);
        }

        public void ActivePassive()
        {
            (passive)?.ActivePassive();
            foreach (var passive in overlappingPassives)
            {
                (passive)?.ActivePassive();
            }
        }

        public void DeactivePassive()
        {
            (passive)?.DeactivePassive();
            foreach (var passive in overlappingPassives)
            {
                (passive).DeactivePassive();
            }

            overlappingPassives.Clear();
        }
#if UNITY_EDITOR
        private void OnChangeActionName()
        {
            string path = AssetDatabase.GetAssetPath(this);
            AssetDatabase.RenameAsset(path, $"Command Action Piece_{actionName}");
        }
#endif
    }
}