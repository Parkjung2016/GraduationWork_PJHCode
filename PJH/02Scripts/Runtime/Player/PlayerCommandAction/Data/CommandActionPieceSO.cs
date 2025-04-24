using System.Collections.Generic;
using KHJ.Passive.Shared;
using Main.Shared;
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

        [VerticalGroup("Top/Right")] [MultiLineProperty(3)] [LabelText("📖 설명")]
        public string actionDescription;

        [VerticalGroup("Top/Right")] [LabelText("⚔ 전투 데이터")]
        public PlayerCombatDataSO combatData;

        [BoxGroup("⚙ 패시브 설정", showLabel: true)] [LabelText("🌟 메인 패시브")] [SerializeField]
        public ScriptableObject passive;

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
        public List<ScriptableObject> overlappingPassives;

        public void InitPassive(IPlayer player)
        {
            (passive as IPassive)?.Init(player);
        }

        public void ActivePassive()
        {
            (passive as IPassive)?.ActivePassive();
            foreach (var passive in overlappingPassives)
            {
                (passive as IPassive).ActivePassive();
            }
        }

        public void DeactivePassive()
        {
            (passive as IPassive)?.DeactivePassive();
            foreach (var passive in overlappingPassives)
            {
                (passive as IPassive).DeactivePassive();
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