using System;
using BIS.Manager;
using Cysharp.Threading.Tasks;
using Main.Core;
using Main.Runtime.Core.Events;
using Main.Runtime.Manager;
using Main.Shared;
using PJH.Runtime.Players;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace PJH.Runtime.Core.InteractObjects
{
    public class NextLevelInteractObject : MonoBehaviour, IInteractable
    {
        [SerializeField] private ParticleSystem _nextPointIndicatorParticle;
        [field: SerializeField] public Transform UIDisplayTrm { get; private set; }
        public Vector3 AdditionalUIDisplayPos { get; }
        public bool CanInteract => true;
        [field: SerializeField] public string Description { get; set; }
        [SerializeField] private GameObject[] _prevLevelObject, _nextLevelObject;
        [SerializeField] private Transform _nextPlayerPoint;
        private GameEventChannelSO _gameEventChannel;

        private PlayerInputSO _playerInput;

        private void Awake()
        {
            _playerInput = AddressableManager.Load<PlayerInputSO>("PlayerInputSO");
            gameObject.SetActive(false);
            SubscribeEvents();
        }

        private void OnEnable()
        {
            UnSubscribeEvents();
            SubscribeEvents();
        }

        private void OnDisable()
        {
            UnSubscribeEvents();
        }

        private void SubscribeEvents()
        {
            if (!TryGetGameEventChannel()) return;
            _gameEventChannel.AddListener<ActiveNextSession>(HandleActiveNextSession);
        }

        private void UnSubscribeEvents()
        {
            if (!TryGetGameEventChannel()) return;
            _gameEventChannel.RemoveListener<ActiveNextSession>(HandleActiveNextSession);
        }

        protected bool TryGetGameEventChannel()
        {
            if (!_gameEventChannel)
                _gameEventChannel = AddressableManager.Load<GameEventChannelSO>("GameEventChannel");
            return _gameEventChannel;
        }

        private void HandleActiveNextSession(ActiveNextSession evt)
        {
            gameObject.SetActive(true);
        }

        public void Interact(Transform Interactor)
        {
            if (TryGetGameEventChannel())
            {
                _gameEventChannel.RemoveListener<ActiveNextSession>(HandleActiveNextSession);
            }

            _nextPointIndicatorParticle?.Stop();
            Player player = PlayerManager.Instance.Player as Player;
            _playerInput.EnablePlayerInput(false);
            _playerInput.EnableUIInput(false);
            SceneControlManager.FadeOut(async () =>
            {
                PlayerMovement movementCompo = player.GetCompo<PlayerMovement>();
                PlayerIK ikCompo = player.GetCompo<PlayerIK>();
                movementCompo.CC.enabled = false;
                player.transform.position = _nextPlayerPoint.position;
                player.ModelTrm.rotation = _nextPlayerPoint.rotation;
                ikCompo.LegsAnimator.User_Teleport(_nextPlayerPoint.position);
                ikCompo.LeaningAnimator.User_AfterTeleport();
                movementCompo.CC.enabled = true;
                GameEvents.DestroyDeadEnemy.isPlayingBossDeathTimeline = false;
                var enterNextLevelEvt = GameEvents.EnterNextLevel;
                enterNextLevelEvt.playerModelTrm = player.ModelTrm;
                _gameEventChannel.RaiseEvent(enterNextLevelEvt);
                for (int i = 0; i < _prevLevelObject.Length; i++)
                    _prevLevelObject[i].SetActive(false);
                for (int i = 0; i < _nextLevelObject.Length; i++)
                    _nextLevelObject[i].SetActive(true);
                var enableCameraMovementEvt = GameEvents.EnableCameraMovement;
                enableCameraMovementEvt.enableCameraMovmement = false;
                _gameEventChannel.RaiseEvent(enableCameraMovementEvt);
                await UniTask.WaitForSeconds(1f, cancellationToken: gameObject.GetCancellationTokenOnDestroy());
                SceneControlManager.FadeIn(() =>
                {
                    _playerInput.EnablePlayerInput(true);
                    _playerInput.EnableUIInput(true);
                    enableCameraMovementEvt.enableCameraMovmement = true;
                    _gameEventChannel.RaiseEvent(enableCameraMovementEvt);
                }, false);
            }, false);
        }
    }
}