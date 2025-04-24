using Main.Runtime.Combat;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace PJH.Runtime.Players
{
    [CreateAssetMenu(fileName = "PlayerCombatData", menuName = "SO/Combat/PlayerCombatData")]
    public class PlayerCombatDataSO : CombatDataSO
    {
        [Title("Manual Move")] [LabelText("공격 중에 앞으로 가는지(RootMotion이 아닌 애니메이션)")]
        public bool isManualMove;

        [ShowIf("isManualMove")] [LabelText("공격 중에 앞으로 가는 시간"), SuffixLabel("초", true)]
        public float manualMoveSpeed;


#if UNITY_EDITOR
        private void OnValidate()
        {
            if (attackAnimationClip.Name == name) return;
            string path = AssetDatabase.GetAssetPath(this);
            AssetDatabase.RenameAsset(path, attackAnimationClip.Name);
        }
#endif
    }
}