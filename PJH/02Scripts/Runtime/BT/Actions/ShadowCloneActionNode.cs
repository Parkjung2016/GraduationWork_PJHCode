using Main.Runtime.Manager;
using Opsive.BehaviorDesigner.Runtime.Tasks.Actions;
using PJH.Runtime.BossSkill.BossSkills.ShadowClones;
using PJH.Runtime.Players;

namespace PJH.Runtime.BT.Actions
{
    public class ShadowCloneActionNode : ActionNode
    {
        protected ShadowClone _shadowClone;
        protected Player _player;

        public override void OnAwake()
        {
            _shadowClone = gameObject.GetComponent<ShadowClone>();
        }

        public override void OnStart()
        {
            _player = PlayerManager.Instance.Player as Player;
        }
    }
}