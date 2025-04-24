using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;

namespace PJH.Runtime.Players.FinisherSequence
{
    [CreateAssetMenu(menuName = "SO/Finisher/SequenceData")]
    public class FinisherSequenceDataSO : ScriptableObject
    {
        [InfoBox("Automatically changed according to SO name setting value")] [OnValueChanged("ChangeAssetName")]
        public PlayableAsset sequenceAsset;

        public float distanceFromEnemy;
#if UNITY_EDITOR

        private void ChangeAssetName()
        {
            string assetName = $"FinisherSequence_{sequenceAsset.name}";
            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(this), assetName);
        }
#endif
    }
}