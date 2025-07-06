using System;
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

        public event Action OnEquipped, OnUnEquipped;

        public bool IsCloned { get; private set; }

        private bool _inited;

        public virtual void EquipPiece(IPlayer player)
        {
            if (!_inited)
            {
                _inited = true;
                Init(player);
            }

            OnEquipped?.Invoke();
        }

        public virtual void Init(IPlayer player)
        {
        }

        public virtual void UnEquipPiece()
        {
            OnUnEquipped?.Invoke();
        }

        public T Clone<T>() where T : PlayerPieceSO
        {
            if (IsCloned) return this as T;
            return Instantiate(this) as T;
        }
    }
}