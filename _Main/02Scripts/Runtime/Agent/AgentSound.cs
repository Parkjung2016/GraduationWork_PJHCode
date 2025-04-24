using FMODUnity;
using Main.Runtime.Core;
using UnityEngine;

namespace Main.Runtime.Agents
{
    public class AgentSound : MonoBehaviour, IAgentComponent
    {
        private readonly string footstepPath = "event:/SFX/Footsteps/Footstep_Tile";
        private Agent _agent;

        public virtual void Initialize(Agent agent)
        {
            _agent = agent;
        }

        public void PlayFootstepSound(Vector3 position)
        {
            RuntimeManager.PlayOneShot(footstepPath, position);
        }
    }
}