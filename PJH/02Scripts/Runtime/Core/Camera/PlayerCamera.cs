using DG.Tweening;
using Main.Core;
using Main.Runtime.Core.Events;
using Main.Shared;
using Sirenix.OdinInspector;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PJH.Runtime.Core.PlayerCamera
{
    public class PlayerCamera : MonoBehaviour, ICamera
    {
        public PlayerCameraTargetDetection CameraTargetDetectionCompo { get; private set; }
        private GameEventChannelSO _gameEventChannel;

        [TabGroup("CameraOffset", Icon = SdfIconType.Apple)] [SerializeField]
        private float
            _whenRunningCameraDistance;

        [TabGroup("CameraOffset", Icon = SdfIconType.Apple)] [SerializeField]
        private float _changeCameraDistanceDuration = .5f;

        [TabGroup("CameraOffset", Icon = SdfIconType.Apple)] [SerializeField]
        private float
            _whenRunningVerticalArmLength = .5f;

        [TabGroup("CameraOffset", Icon = SdfIconType.Apple)] [SerializeField]
        private float _changeVerticalArmLengthDuration = 4f;

        [TabGroup("CameraInput", Icon = SdfIconType.Alarm)] [SerializeField]
        private Vector2 _rotationPower;

        [TabGroup("CameraInput", Icon = SdfIconType.Alarm)] [SerializeField]
        private InputActionReference _lookInput;

        [TabGroup("CameraInput", Icon = SdfIconType.Alarm)] [SerializeField]
        private bool _invertYAxis = false, _invertXAxis;

        [TabGroup("CameraInput", Icon = SdfIconType.Alarm)] [SerializeField]
        private Vector2 _xAngleRange;

        private Sequence _cameraViewConfigSequence, _cameraFOVSequence;
        private CinemachineCamera _cinemachineCamera;
        private CinemachineThirdPersonFollow _thirdPersonFollow;
        private float _originCameraDistance;
        private float _originVerticalArmLength;
        private float _originCameraFOV;

        private bool _updateIgnoreTimeScale;

        private Camera _uiCamera;

        private void Awake()
        {
            _uiCamera = Camera.main.transform.Find("WorldCanvasCamera").GetComponent<Camera>();
            CameraTargetDetectionCompo = GetComponent<PlayerCameraTargetDetection>();
            _gameEventChannel = AddressableManager.Load<GameEventChannelSO>("GameEventChannel");

            _cinemachineCamera = GetComponent<CinemachineCamera>();
            _thirdPersonFollow = GetComponent<CinemachineThirdPersonFollow>();
            _originCameraDistance = _thirdPersonFollow.CameraDistance;
            _originVerticalArmLength = _thirdPersonFollow.VerticalArmLength;
            _originCameraFOV = _cinemachineCamera.Lens.FieldOfView;

            _gameEventChannel.AddListener<CameraViewConfig>(HandleCameraViewConfig);
            _gameEventChannel.AddListener<CameraInvertInput>(HandleCameraInvertInput);
            _gameEventChannel.AddListener<EnemyFinisherSequence>(HandleEnemyFinisherSequence);
            _gameEventChannel.AddListener<FinishTimeline>(HandleFinishTimeline);
            _gameEventChannel.AddListener<ChangeCameraFOV>(HandleChangeCameraFOV);
            _gameEventChannel.AddListener<ChangeCameraUpdate>(HandleChangeCameraUpdate);
        }

        private void OnDestroy()
        {
            _gameEventChannel.RemoveListener<CameraViewConfig>(HandleCameraViewConfig);
            _gameEventChannel.RemoveListener<CameraInvertInput>(HandleCameraInvertInput);
            _gameEventChannel.RemoveListener<EnemyFinisherSequence>(HandleEnemyFinisherSequence);
            _gameEventChannel.RemoveListener<FinishTimeline>(HandleFinishTimeline);
            _gameEventChannel.RemoveListener<ChangeCameraFOV>(HandleChangeCameraFOV);
            _gameEventChannel.RemoveListener<ChangeCameraUpdate>(HandleChangeCameraUpdate);
        }

        private void HandleChangeCameraUpdate(ChangeCameraUpdate evt)
        {
            _updateIgnoreTimeScale = evt.updateIgnoreTimeScale;
        }

        private void HandleChangeCameraFOV(ChangeCameraFOV evt)
        {
            if (_cameraFOVSequence != null && _cameraFOVSequence.IsActive())
                _cameraFOVSequence.Kill();
            _cameraFOVSequence = DOTween.Sequence();
            if (!evt.resetFOV)
            {
                _cameraFOVSequence.Append(DOTween.To(() => _cinemachineCamera.Lens.FieldOfView,
                    x => _cinemachineCamera.Lens.FieldOfView = x,
                    evt.fovValue,
                    evt.changeDuration));
            }
            else
            {
                _cameraFOVSequence.Append(DOTween.To(() => _cinemachineCamera.Lens.FieldOfView,
                    x => _cinemachineCamera.Lens.FieldOfView = x,
                    _originCameraFOV,
                    evt.changeDuration));
            }

            _cameraFOVSequence.SetUpdate(true);
        }

        private void HandleCameraInvertInput(CameraInvertInput evt)
        {
            _invertXAxis = evt.isInvertXAxis;
            _invertYAxis = evt.isInvertYAxis;
        }


        private void HandleFinishTimeline(FinishTimeline evt)
        {
            enabled = true;
        }

        private void HandleEnemyFinisherSequence(EnemyFinisherSequence evt)
        {
            enabled = false;
        }


        private void HandleCameraViewConfig(CameraViewConfig evt)
        {
            if (_cameraViewConfigSequence != null && _cameraViewConfigSequence.IsActive())
                _cameraViewConfigSequence.Kill();
            _cameraViewConfigSequence = DOTween.Sequence();

            if (evt.isChangeConfig)
            {
                _cameraViewConfigSequence.Append(DOTween.To(() => _thirdPersonFollow.CameraDistance,
                    x => _thirdPersonFollow.CameraDistance = x,
                    _whenRunningCameraDistance,
                    _changeCameraDistanceDuration));
                _cameraViewConfigSequence.Join(DOTween.To(() => _thirdPersonFollow.VerticalArmLength,
                    x => _thirdPersonFollow.VerticalArmLength = x,
                    _whenRunningVerticalArmLength,
                    _changeVerticalArmLengthDuration));
            }
            else
            {
                _cameraViewConfigSequence.Append(DOTween.To(() => _thirdPersonFollow.CameraDistance,
                    x => _thirdPersonFollow.CameraDistance = x,
                    _originCameraDistance,
                    _changeCameraDistanceDuration));
                _cameraViewConfigSequence.Join(DOTween.To(() => _thirdPersonFollow.VerticalArmLength,
                    x => _thirdPersonFollow.VerticalArmLength = x,
                    _originVerticalArmLength,
                    _changeVerticalArmLengthDuration));
            }
        }

        private void Update()
        {
            _uiCamera.fieldOfView = _cinemachineCamera.Lens.FieldOfView;
        }

        private void LateUpdate()
        {
            if (!_updateIgnoreTimeScale && Time.timeScale == 0) return;
            Vector2 look = _lookInput.action.ReadValue<Vector2>();
            Transform trackingTarget = _cinemachineCamera.Target.TrackingTarget;
            trackingTarget.rotation *=
                Quaternion.AngleAxis(look.x * (_invertXAxis ? -1 : 1) * _rotationPower.x, Vector3.up);
            if (_cinemachineCamera.IsLive)
                trackingTarget.rotation *=
                    Quaternion.AngleAxis(look.y * (_invertYAxis ? 1 : -1) * _rotationPower.y, Vector3.right);

            Vector3 angles = trackingTarget.eulerAngles;
            angles.z = 0;
            float angleX = trackingTarget.eulerAngles.x;
            if (angleX > 180 && angleX < _xAngleRange.x)
                angles.x = _xAngleRange.x;
            else if (angleX < 180 && angleX > _xAngleRange.y)
            {
                angles.x = _xAngleRange.y;
            }

            trackingTarget.eulerAngles = angles;
        }
    }
}