using System;
using System.Collections.Generic;
using Main.Runtime.Core;
using UnityEngine;
using ZLinq;

namespace Main.Runtime.Agents
{
    public class ComponentManager
    {
        private Dictionary<Type, IAgentComponent> _components = new();

        public void AddComponentToDictionary(Agent owner)
        {
            owner.GetComponentsInChildren<IAgentComponent>(true)
                .ToList().ForEach(component => _components.Add(component.GetType(), component));
        }

        public void ComponentInitialize(Agent owner)
        {
            _components.Values.ToList().ForEach(component => component.Initialize(owner));
        }

        public void AfterInitialize()
        {
            _components.Values.OfType<IAfterInitable>()
                .ToList().ForEach(afterInitable => afterInitable.AfterInitialize());
        }

        public T GetCompo<T>(bool isDerived = false) where T : IAgentComponent
        {
            if (_components.TryGetValue(typeof(T), out IAgentComponent component))
            {
                return (T)component;
            }

            if (isDerived == false) return default;

            Type findType = _components.Keys.FirstOrDefault(type => type.IsSubclassOf(typeof(T)));
            if (findType != null)
                return (T)_components[findType];

            return default;
        }

        public bool TryGetCompo<T>(out T compo, bool isDerived = false) where T : IAgentComponent
        {
            compo = GetCompo<T>(isDerived);
            return compo != null;
        }

        public void EnableComponents(bool isEnabled)
        {
            _components.Values.OfType<MonoBehaviour>().ToList().ForEach(component =>
            {
                if (component is not AgentAnimator)
                    component.enabled = isEnabled;
            });
        }
    }
}