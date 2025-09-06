using Main.Runtime.Core.Events;
using Opsive.BehaviorDesigner.Runtime.Tasks;
using Opsive.BehaviorDesigner.Runtime.Tasks.Actions;
using PJH.Utility.Managers;

namespace PJH.Runtime.BT.Actions
{
    public class ShowTextDialogueAction : ActionNode
    {
        public string dialogueText;
        private GameEventChannelSO _uiEventChannel;

        public override void OnAwake()
        {
            _uiEventChannel = AddressableManager.Load<GameEventChannelSO>("UIEventChannelSO");
        }

        public override TaskStatus OnUpdate()
        {
            var evt = UIEvents.ShowTextDialogueUI;
            evt.dialogueText = dialogueText;
            _uiEventChannel.RaiseEvent(evt);
            return TaskStatus.Success;
        }
    }
}