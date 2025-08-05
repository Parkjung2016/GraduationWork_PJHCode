using System;
using System.Collections.Generic;
using System.Reflection;
using Animancer;
using Main.Runtime.Animators;
using Main.Runtime.Core;
using Main.Shared;
using UnityEngine;

namespace Main.Runtime.Agents
{
    public class AgentAnimationTrigger : MonoBehaviour, IAgentComponent
    {
        [SerializeField] private List<AnimancerEventAssetSO> _animationEvents;
        public event Action OnAnimationEnd;
        public event Action OnPlayAttackWhooshSound;
        public Action OnDisableDamageCollider;
        public event Action<int> OnSetGetDamagedAnimationIndex;
        public event Action<Define.ESocketType> OnEnableDamageCollider;
        public Action OnTriggerRagdoll;
        public Action OnGetUp;


        private HybridAnimancerComponent _hybridAnimancer;


        private Dictionary<StringAsset, List<MethodInfo>> _eventMethodInfos;

        public virtual void Initialize(Agent agent)
        {
            _eventMethodInfos = new();

            _hybridAnimancer = GetComponent<HybridAnimancerComponent>();

            Type type = GetType();
            MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic);
            type = type.BaseType;
            MethodInfo[] parentMethods = type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic);
            type = type.BaseType;
            MethodInfo[] parent2Methods = type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (var eventAsset in _animationEvents)
            {
                RegisterEventMethodInfo(methods, parentMethods, parent2Methods, eventAsset);
                _hybridAnimancer.Events.AddTo<object>(eventAsset, value =>
                {
                    if (_eventMethodInfos.TryGetValue(eventAsset, out var methodList))
                    {
                        try
                        {
                            if (value != null)
                            {
                                var parameterType = methodList[0].GetParameters().First().ParameterType;
                                object convertedValue = Convert.ChangeType(value, parameterType);
                                methodList[0].Invoke(this, new[] { convertedValue });
                            }
                            else
                            {
                                if (methodList.Count > 1)
                                    methodList[1].Invoke(this, null);
                                else
                                    methodList[0].Invoke(this, null);
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.LogError($"{_hybridAnimancer.States.Current} : {eventAsset} : {e} : {e.StackTrace}");
                        }
                    }
                });
            }
        }

        private void RegisterEventMethodInfo(MethodInfo[] methods, MethodInfo[] parentMethods,
            MethodInfo[] parentMethods2,
            AnimancerEventAssetSO eventAsset)
        {
            MethodInfo method;
            string methodName = eventAsset.Name.String.Replace("Event", "");
            if (eventAsset.hasParameterMethod)
            {
                method = methods.FirstOrDefault(method =>
                    method.Name == methodName && method.GetParameters().Length == 1);
                if (method == null)
                    method = parentMethods.FirstOrDefault(method =>
                        method.Name == methodName && method.GetParameters().Length == 1);
                if (method == null)
                    method = parentMethods2.FirstOrDefault(method =>
                        method.Name == methodName && method.GetParameters().Length == 1);
                if (method != null)
                    _eventMethodInfos.Add(eventAsset, new List<MethodInfo>() { method });
            }

            method = methods.FirstOrDefault(method =>
                method.Name == methodName && method.GetParameters().Length == 0);
            if (method == null)
                method = parentMethods.FirstOrDefault(method =>
                    method.Name == methodName && method.GetParameters().Length == 0);
            if (method == null)
                method = parentMethods2.FirstOrDefault(method =>
                    method.Name == methodName && method.GetParameters().Length == 0);
            if (method != null)
            {
                if (_eventMethodInfos.ContainsKey(eventAsset))
                    _eventMethodInfos[eventAsset].Add(method);
                else
                    _eventMethodInfos.Add(eventAsset, new() { method });
            }
        }

        protected virtual void EnableDamageCollider(Define.ESocketType socketType)
        {
            OnEnableDamageCollider?.Invoke(socketType);
        }

        protected virtual void DisableDamageCollider()
        {
            OnDisableDamageCollider?.Invoke();
        }

        private void AnimationEnd()
        {
            OnAnimationEnd?.Invoke();
        }

        private void PlayAttackWhooshSound()
        {
            OnPlayAttackWhooshSound?.Invoke();
        }

        private void TriggerRagdoll()
        {
            OnTriggerRagdoll?.Invoke();
        }

        private void GetUp()
        {
            OnGetUp?.Invoke();
        }

        private void SetGetDamagedAnimationIndex(int idx)
        {
            OnSetGetDamagedAnimationIndex?.Invoke(idx);
        }
    }
}