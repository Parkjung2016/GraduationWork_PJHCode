using System;
using Main.Runtime.Agents;

namespace PJH.Runtime.BossSkill.BossSkills.ShadowClones
{
    public class ShadowCloneAnimationTrigger : AgentAnimationTrigger
    {
        public event Action OnComboPossible;
        public event Action OnLookPlayer;
        private ShadowClone _shadowClone;

        public override void Initialize(Agent agent)
        {
            base.Initialize(agent);
            _shadowClone = agent as ShadowClone;
        }

        private void ComboPossible()
        {
            OnComboPossible?.Invoke();
        }
        private void LookPlayer()
        {
            OnLookPlayer?.Invoke();
        }
    }
}