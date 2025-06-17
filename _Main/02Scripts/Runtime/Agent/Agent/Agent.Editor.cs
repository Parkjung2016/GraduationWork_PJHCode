using Sirenix.OdinInspector;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Main.Runtime.Agents
{
    public partial class Agent
    {
#if UNITY_EDITOR
        [BoxGroup("Utility", CenterLabel = true)]
        [GUIColor(0, 1, 0)]
        [HideIf("@this.GetComponentInChildren<AgentStat>() != null")]
        [ButtonGroup("Utility/Buttons")]
        private void AddAgentStatComponent()
        {
            AgentStat agentStatCompo = GetComponentInChildren<AgentStat>();
            if (agentStatCompo != null)
            {
                Debug.LogWarning("AgentStat already exists");
                return;
            }

            GameObject obj = new GameObject("AgentStat");
            obj.transform.SetParent(transform);
            obj.AddComponent<AgentStat>();
            Selection.activeGameObject = obj;
        }

        [HideIf("@this.GetComponentInChildren<AgentStat>() == null")]
        [GUIColor(1, 0, 0)]
        [ButtonGroup("Utility/Buttons")]
        private void RemoveAgentStatComponent()
        {
            AgentStat agentStatCompo = GetComponentInChildren<AgentStat>();
            if (agentStatCompo == null)
            {
                Debug.LogWarning("AgentStat does not exists");
                return;
            }

            if (agentStatCompo.gameObject.name == "AgentStat")
                DestroyImmediate(agentStatCompo.gameObject);
            else
                DestroyImmediate(agentStatCompo);
        }

        [HideIf("@this.GetComponentInChildren<AgentEquipmentSystem>() != null")]
        [GUIColor(0, 1, 0)]
        [ButtonGroup("Utility/Buttons")]
        private void AddAgentEquipmentSystemComponent()
        {
            AgentEquipmentSystem agentEquipmentSystemCompo = GetComponentInChildren<AgentEquipmentSystem>();
            if (agentEquipmentSystemCompo != null)
            {
                Debug.LogWarning("AgentEquipmentSystem already exists");
                return;
            }

            GameObject obj = new GameObject("AgentEquipmentSystem");
            obj.transform.SetParent(transform);
            obj.AddComponent<AgentEquipmentSystem>();
            Selection.activeGameObject = obj;
        }


        [HideIf("@this.GetComponentInChildren<AgentEquipmentSystem>() == null")]
        [GUIColor(1, 0, 0)]
        [ButtonGroup("Utility/Buttons")]
        private void RemoveAgentEquipmentSystemComponent()
        {
            AgentEquipmentSystem agentEquipmentSystemCompo = GetComponentInChildren<AgentEquipmentSystem>();
            if (agentEquipmentSystemCompo == null)
            {
                Debug.LogWarning("AgentEquipmentSystem does not exists");
                return;
            }

            if (agentEquipmentSystemCompo.gameObject.name == "AgentEquipmentSystem")
                DestroyImmediate(agentEquipmentSystemCompo.gameObject);
            else
                DestroyImmediate(agentEquipmentSystemCompo);
        }

        [BoxGroup("Utility", CenterLabel = true)]
        [GUIColor(0, 1, 0)]
        [HideIf("@this.GetComponentInChildren<AgentMomentumGauge>() != null")]
        [ButtonGroup("Utility/Buttons")]
        private void AddAgentMomentumGaugeComponent()
        {
            AgentMomentumGauge compo = GetComponentInChildren<AgentMomentumGauge>();
            if (compo != null)
            {
                Debug.LogWarning("AgentMomentumGauge already exists");
                return;
            }

            GameObject obj = new GameObject("AgentMomentumGauge");
            obj.transform.SetParent(transform);
            obj.AddComponent<AgentMomentumGauge>();
            Selection.activeGameObject = obj;
        }

        [HideIf("@this.GetComponentInChildren<AgentMomentumGauge>() == null")]
        [GUIColor(1, 0, 0)]
        [ButtonGroup("Utility/Buttons")]
        private void RemoveAgentMomentumGaugeComponent()
        {
            AgentMomentumGauge compo = GetComponentInChildren<AgentMomentumGauge>();
            if (compo == null)
            {
                Debug.LogWarning("AgentMomentumGauge does not exists");
                return;
            }

            if (compo.gameObject.name == "AgentMomentumGauge")
                DestroyImmediate(compo.gameObject);
            else
                DestroyImmediate(compo);
        }
#endif
    }
}