using System;
using Main.Runtime.Agents;
using Main.Runtime.Core;
using Main.Shared;
using UnityEngine;

namespace PJH.Runtime.Players
{
    public class PlayerInteract : MonoBehaviour, IAgentComponent, IAfterInitable
    {
        public event Action<IInteractable> OnInteract;

        private Player _player;

        public void Initialize(Agent agent)
        {
            _player = agent as Player;
        }

        public void AfterInitialize()
        {
            _player.PlayerInput.InteractEvent += HandleInteract;
        }

        private void OnDestroy()
        {
            _player.PlayerInput.InteractEvent -= HandleInteract;
        }

        private void HandleInteract()
        {
            PlayerInteractableObjectDetection interactableObjectDetectionCompo =
                _player.GetCompo<PlayerInteractableObjectDetection>();

            if (_player.IsHitting || !interactableObjectDetectionCompo.GetNearTarget(out IInteractable target)) return;
            target.Interact(_player.transform);
            OnInteract?.Invoke(target);
        }
    }
}