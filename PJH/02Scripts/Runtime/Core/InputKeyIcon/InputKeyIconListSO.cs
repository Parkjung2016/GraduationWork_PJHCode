using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PJH.Runtime.Core.InputKeyIcon
{
    [CreateAssetMenu(menuName = "SO/InputKeyIcon/List")]
    public class InputKeyIconListSO : ScriptableObject
    {
        [SerializeField, AssetList(AssetNamePrefix = "KeyIcon_", AutoPopulate = true)]
        private List<InputKeyIcon> _inputKeyIcons;

        public InputKeyIcon GetInputKeyIcon(string inputKeyName)
        {
            return _inputKeyIcons.Find(x => x.keyName == inputKeyName);
        }
    }
}