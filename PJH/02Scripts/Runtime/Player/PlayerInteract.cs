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
        private PlayerInteractableObjectDetection _interactableObjectDetectionCompo;

        public void Initialize(Agent agent)
        {
            _player = agent as Player;
            _interactableObjectDetectionCompo = _player.GetCompo<PlayerInteractableObjectDetection>();
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
            if (_player.IsHitting || !_interactableObjectDetectionCompo.GetNearTarget(out IInteractable target)) return;
            target.Interact(_player.transform);
            OnInteract?.Invoke(target);
        }
    }
}