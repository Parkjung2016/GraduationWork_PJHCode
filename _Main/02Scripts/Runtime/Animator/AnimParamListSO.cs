using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Main.Runtime.Animators
{
    [CreateAssetMenu(menuName = "SO/Animator/ParamList")]
    public class AnimParamListSO : ScriptableObject
    {
        public List<AnimParamSO> list = new();

        private List<AnimParamSO> _boolList;

        private void OnEnable()
        {
            _boolList = list.Where(param => param.paramType == ParamType.Boolean).ToList();
        }

        public void ClearBooleanParam(Animator animator)
        {
            foreach (var param in _boolList)
            {
                animator.SetBool(param.hashValue, false);
            }
        }
    }
}