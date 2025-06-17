using Main.Shared;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PJH.Shared
{
    public abstract class PlayerPieceSO : SerializedScriptableObject
    {
        [Title("📌 기본 정보", titleAlignment: TitleAlignments.Centered, bold: true)]
        [HorizontalGroup("Top", Width = 80)]
        [PreviewField(70), HideLabel]
        public Sprite pieceIcon;

        [VerticalGroup("Top/Right")] [LabelText("📝 표시 이름")]
        public string pieceDisplayName;

        [VerticalGroup("Top/Right")] [MultiLineProperty(3)] [LabelText("📖 설명")]
        public string pieceDescription;


        public bool IsCloned { get; private set; }

        public virtual void EquipPiece(IPlayer player)
        {
            Debug.Log($"{name} 장착");
        }

        public virtual void UnEquipPiece()
        {
        }

        public T Clone<T>() where T : PlayerPieceSO
        {
            if (IsCloned) return this as T;
            return Instantiate(this) as T;
        }
    }
}