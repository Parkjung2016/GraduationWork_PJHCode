using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace PJH.Runtime.Core.InputKeyIcon
{
    [CreateAssetMenu(menuName = "SO/InputKeyIcon/KeyIcon")]
    public class InputKeyIcon : ScriptableObject
    {
        [Delayed, OnValueChanged("ChangeAssetName")]
        public string keyName;
        public Sprite keyIcon;
        #if UNITY_EDITOR
        private void ChangeAssetName()
        {
            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(this),$"KeyIcon_{keyName}");
            
        }
        #endif
    }
}