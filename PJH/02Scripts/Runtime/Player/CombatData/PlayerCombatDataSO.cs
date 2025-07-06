using Main.Runtime.Combat;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PJH.Runtime.Players
{
    [CreateAssetMenu(fileName = "PlayerCombatData", menuName = "SO/Combat/PlayerCombatData")]
    public class PlayerCombatDataSO : CombatDataSO
    {
        [Title("Manual Move")] [LabelText("공격 중에 앞으로 가는지(RootMotion이 아닌 애니메이션)")]
        public bool isManualMove;

        [ShowIf("isManualMove")] [LabelText("공격 중에 앞으로 가는 속도"), SuffixLabel("m/s", true)]
        public float manualMoveSpeed;

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            if (attackAnimationClip.Name == name) return;
            string path = UnityEditor.AssetDatabase.GetAssetPath(this);
            UnityEditor.AssetDatabase.RenameAsset(path, attackAnimationClip.Name);
        }
#endif
    }
}