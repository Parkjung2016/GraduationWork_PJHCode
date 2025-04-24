using Animancer;
using UnityEngine;

namespace Main.Runtime.Animators
{
    public class AnimatorRenderer : MonoBehaviour
    {
        protected HybridAnimancerComponent _hybridAnimancer;
        public Animator Animator => _hybridAnimancer.Animator;
        public HybridAnimancerComponent Animancer => _hybridAnimancer;

        public void SetParam(AnimParamSO param, bool value) => _hybridAnimancer.SetBool(param.hashValue, value);

        public void SetParam(AnimParamSO param, float value, float dampTime) =>
            _hybridAnimancer.SetFloat(param.hashValue, value, dampTime, Time.deltaTime);

        public void SetParam(AnimParamSO param, float value, float dampTime, float deltaTime) =>
            _hybridAnimancer.SetFloat(param.hashValue, value, dampTime, deltaTime);

        public void SetParam(AnimParamSO param, float value) => _hybridAnimancer.SetFloat(param.hashValue, value);
        public void SetParam(AnimParamSO param, int value) => _hybridAnimancer.SetInteger(param.hashValue, value);
        public void SetParam(AnimParamSO param) => _hybridAnimancer.SetTrigger(param.hashValue);
        public float GetParamFloat(AnimParamSO param) => _hybridAnimancer.GetFloat(param.hashValue);
        public int GetParamInt(AnimParamSO param) => _hybridAnimancer.GetInteger(param.hashValue);
        public bool GetParamBool(AnimParamSO param) => _hybridAnimancer.GetBool(param.hashValue);

        protected virtual void Awake()
        {
            _hybridAnimancer = GetComponent<HybridAnimancerComponent>();
        }
    }
}