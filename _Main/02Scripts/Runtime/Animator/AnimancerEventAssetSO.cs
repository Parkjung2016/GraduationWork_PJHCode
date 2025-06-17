using System;
using Animancer;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Serialization;

namespace Main.Runtime.Animators
{
    [CreateAssetMenu(menuName = "SO/Animancer/EventAsset")]
    public class AnimancerEventAssetSO : StringAsset
    {
        public bool hasParameterMethod;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!name.Contains("Event"))
            {
                string path = AssetDatabase.GetAssetPath(this);
                AssetDatabase.RenameAsset(path, $"{name}Event");
            }
        }
#endif
    }
}