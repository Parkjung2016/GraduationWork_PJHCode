using DG.Tweening;
using FIMSpace.FLook;
using Kinemation.MotionWarping.Runtime.Core;
using Kinemation.MotionWarping.Runtime.Examples;
using Main.Runtime.Agents;
using Main.Runtime.Manager;
using Main.Shared;
using PJH.Runtime.Players;
using PJH.Utility.Managers;
using UnityEngine;

namespace PJH.Runtime.Core.InteractObjects
{
    public class MedicNPC : Agent, IInteractable
    {
        [field: SerializeField] public Transform UIDisplayTrm { get; private set; }
        [field: SerializeField] public string Description { get; private set; }
        [SerializeField] private float _healAmount = 30;
        public Vector3 AdditionalUIDisplayPos { get; }
        public bool CanInteract { get; private set; }
        private AlignComponent _alignComponent;
        private PlayerInputSO _playerInput;

        private MotionWarping _interactorMotionWarping;

        protected override void Awake()
        {
            base.Awake();
            CanInteract = true;
            _alignComponent = GetComponent<AlignComponent>();
            _playerInput = AddressableManager.Load<PlayerInputSO>("PlayerInputSO");
        }

        protected override void Start()
        {
            base.Start();
            transform.Find("Model").GetComponent<FLookAnimator>().ObjectToFollow =
                PlayerManager.Instance.Player.HeadTrm;
        }

        public void Interact(Transform interactor)
        {
            _playerInput.EnablePlayerInput(false);
            transform.DOLookAt(interactor.position, .2f, AxisConstraint.Y).OnComplete(() =>
            {
                _interactorMotionWarping = interactor.GetComponent<MotionWarping>();
                _interactorMotionWarping.Interact(_alignComponent);
                _interactorMotionWarping.OnAnimationFinished += HandleAnimationFinished;
                interactor.GetComponent<PlayerHealth>().HealAmountFunc += () => _healAmount;
            });
            CanInteract = false;
        }

        private void HandleAnimationFinished()
        {
            _interactorMotionWarping.OnAnimationFinished -= HandleAnimationFinished;
            _playerInput.EnablePlayerInput(true);
        }
    }
}