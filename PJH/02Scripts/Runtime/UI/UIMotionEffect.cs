using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Main.Runtime.Manager;
using PJH.Runtime.Core.PlayerCamera;
using PJH.Runtime.Players;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PJH.Runtime.UI
{
    public class UIMotionEffect : MonoBehaviour
    {
        [SerializeField] private bool _useMovementMotion = true;

        [SerializeField, ShowIf("_horizontalMultiplier")]
        private float _horizontalMultiplier = .5f;

        [SerializeField, ShowIf("_useMovementMotion")]
        private float _verticalMultiplier = .5f;

        [SerializeField, ShowIf("_useMovementMotion")]
        private float _motionSpeed = 4f;

        [SerializeField] private bool _useGetDamagedFeedback = true;

        [SerializeField, ShowIf("_useGetDamagedFeedback")]
        private float _getDamagedShakeDuration = .5f;

        [SerializeField, ShowIf("_useGetDamagedFeedback")]
        private float _getDamagedShakeLength = 8f;

        [SerializeField, ShowIf("_useGetDamagedFeedback")]
        private int _getDamagedShakeVibrato = 25;

        private Player _player;
        private PlayerCamera _playerCamera;
        private RectTransform _rectTransformCompo;
        private Vector3 _originPosition;
        private bool _shaking;

        private async void Awake()
        {
            _rectTransformCompo = transform as RectTransform;
            _originPosition = _rectTransformCompo.localPosition;

            await UniTask.WaitUntil(() => PlayerManager.Instance.Player);
            _player = PlayerManager.Instance.Player as Player;
            _playerCamera = PlayerManager.Instance.PlayerCamera;

            if (_useGetDamagedFeedback)
            {
                _player.HealthCompo.OnApplyDamaged += HandleApplyDamaged;
            }
        }

        private void OnDestroy()
        {
            if (_useGetDamagedFeedback)
            {
                _player.HealthCompo.OnApplyDamaged -= HandleApplyDamaged;
            }
        }

        private void HandleApplyDamaged(float value)
        {
            _shaking = true;
            _rectTransformCompo
                .DOShakePosition(_getDamagedShakeDuration, _getDamagedShakeLength, _getDamagedShakeVibrato)
                .OnComplete(() => _shaking = false);
        }

        private void Update()
        {
            if (_shaking || !_player)
                return;

            float horizontal = _playerCamera.CameraVelocity.x * _horizontalMultiplier;
            float vertical = _playerCamera.CameraVelocity.y * _verticalMultiplier;
            Vector3 additionalPosition = new Vector3(horizontal, vertical);
            Vector3 targetPosition = _originPosition + additionalPosition;
            Vector3 position = Vector3.Lerp(_rectTransformCompo.localPosition, targetPosition,
                Time.deltaTime * _motionSpeed);
            _rectTransformCompo.localPosition = position;
        }
    }
}