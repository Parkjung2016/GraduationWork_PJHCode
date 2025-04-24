using Main.Runtime.Agents;
using Main.Runtime.Core;
using UnityEngine;

namespace PJH.Runtime.Players
{
    public class PlayerFullMountTargetDetection : MonoBehaviour, IAgentComponent, IAfterInitable
    {
        private Player _player;

        private Agent _fullMountTarget;

        public void Initialize(Agent agent)
        {
            _player = agent as Player;
        }


        public void AfterInitialize()
        {
            _player.GetCompo<PlayerEnemyDetection>().OnChangedTargetEnemy += HandleChangedTargetEnemy;
        }

        private void OnDestroy()
        {
            _player.GetCompo<PlayerEnemyDetection>().OnChangedTargetEnemy -= HandleChangedTargetEnemy;
        }

        private void HandleChangedTargetEnemy(Agent prevTarget, Agent currentTarget)
        {
            _fullMountTarget = currentTarget;
        }

        public bool GetFullMountTarget(out Agent target)
        {
            target = null;
            if (!_fullMountTarget)
                return false;
            if (_fullMountTarget.IsKnockDown)
                target = _fullMountTarget;

            return target != null;
        }
    }
}