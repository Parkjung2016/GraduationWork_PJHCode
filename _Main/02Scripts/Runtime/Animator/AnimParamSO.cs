using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Main.Runtime.Animators
{
    public enum ParamType
    {
        Boolean,
        Float,
        Trigger,
        Integer
    }

    [CreateAssetMenu(fileName = "AnimatorParamSO", menuName = "SO/Animator/Param")]
    public class AnimParamSO : ScriptableObject
    {
        [InfoBox("Automatically changed according to SO name setting value")] [OnValueChanged("ChangeAssetName")]
        public ParamType paramType;

        [Delayed, OnValueChanged("ChangeAssetName")]
        public string paramName;

        public int hashValue;

#if UNITY_EDITOR

        private void ChangeAssetName()
        {
            hashValue = Animator.StringToHash(paramName);
            string assetName = $"{paramType}_{paramName}Param";
            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(this), assetName);
        }
#endif
    }
}