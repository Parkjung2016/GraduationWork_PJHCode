using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Main.Runtime.Characters.StateMachine
{
    [CreateAssetMenu(menuName = "SO/ShadowCloneStateMachine/StateList")]
    public class ShadowCloneStateListSO : SerializedScriptableObject
    {
        public List<ShadowCloneStateSO> states = new();

#if UNITY_EDITOR
        private void OnValidate()
        {
            for (int i = 0; i < states.Count; i++)
            {
                if (!states[i])
                    states.RemoveAt(i);
            }
        }
#endif
    }
}