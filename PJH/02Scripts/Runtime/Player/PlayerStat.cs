using Main.Runtime.Agents;
using Main.Runtime.Core;
using Main.Runtime.Core.Events;
using PJH.Utility.Managers;

namespace PJH.Runtime.Players
{
    public class PlayerStat : AgentStat, IAfterInitable
    {
        private GameEventChannelSO _increasePlayerStatEventChannel;

        private Player _player;

        public override void Initialize(Agent agent)
        {
            base.Initialize(agent);
            _increasePlayerStatEventChannel = AddressableManager.Load<GameEventChannelSO>("GameEventChannel");

            _player = agent as Player;
        }

        public void AfterInitialize()
        {
            _increasePlayerStatEventChannel.AddListener<ModifyPlayerStat>(HandleIncreasePlayerStat);
        }

        private void OnDestroy()
        {
            _increasePlayerStatEventChannel.RemoveListener<ModifyPlayerStat>(HandleIncreasePlayerStat);
        }

        private void HandleIncreasePlayerStat(ModifyPlayerStat evt)
        {
            if (evt.isIncreaseStat)
                AddValueModifier(evt.modifyPlayerStat, evt.modifyKey, evt.modifyPlayerStatValue);
            else
                RemoveValueModifier(evt.modifyPlayerStat, evt.modifyKey);
        }
    }
}