using Animancer;
using Main.Runtime.Equipments.Datas;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace PJH.Runtime.Players
{
    [CreateAssetMenu(menuName = "SO/Player/FullMount/FullMountAnimationData")]
    public class FullMountAnimationDataSO : ScriptableObject
    {
        [InfoBox("Automatically changed according to SO name setting value")]
        [OnValueChanged("ChangeAssetName")]
        public EquipmentDataSO key;

        public ClipTransition fullMountAttackAnimation;
        public ClipTransition fullMountedAnimation;


#if UNITY_EDITOR

        private void ChangeAssetName()
        {
            string assetName = $"FullMountAnimation_{key.name.Replace("Data", "")}";
            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(this), assetName);
        }
#endif
    }
}