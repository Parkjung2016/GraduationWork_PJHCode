using Main.Core;
using Main.Runtime.Core.Events;
using Main.Runtime.Manager;
using PJH.Runtime.Players;
using Sirenix.OdinInspector;
using YTH.Boss;

namespace PJH.Runtime.BossSkill
{
    public abstract class BossSkillSO : SerializedScriptableObject
    {
        protected Boss _boss;
        protected GameEventChannelSO _spawnEventChannel;

        private Player _player;

        public virtual void Init(Boss owner)
        {
            _boss = owner;
            _spawnEventChannel = AddressableManager.Load<GameEventChannelSO>("SpawnEventChannel");
        }

        public abstract void ActivateSkill();

        public virtual bool IsSkillFinished()
        {
            return true;
        }

        public Player GetPlayer()
        {
            if (!_player)
            {
                _player = PlayerManager.Instance.Player as Player;
            }

            return _player;
        }
    }
}