using Main.Core;
using Unity.Cinemachine;
using UnityEngine;

namespace Main.Scenes
{
    public class TitleScene : BaseScene
    {
        [SerializeField] private CinemachineCamera _cinemachineCamera;
        [SerializeField] private float _cameraAimSpeed;
        [SerializeField] private Vector2 _cameraAimClampMin, _cameraAimClampMax;
        private CinemachineRotationComposer _cinemachineRotationComposer;

        protected override void Awake()
        {
            base.Awake();
            _cinemachineRotationComposer = _cinemachineCamera.GetComponent<CinemachineRotationComposer>();
        }

        protected override void Start()
        {
            base.Start();
            Application.targetFrameRate = 30;
            CursorManager.EnableCursor(true);
        }

        private void LateUpdate()
        {
            if (!_cinemachineCamera.IsLive || !_playerInput) return;

            Vector2 screenPosition = _cinemachineRotationComposer.Composition.ScreenPosition;
            Vector3 mouseDelta = _playerInput.MouseDelta;
            screenPosition.x -= mouseDelta.x * Time.deltaTime * _cameraAimSpeed;
            screenPosition.y += mouseDelta.y * Time.deltaTime * _cameraAimSpeed;
            screenPosition.x = Mathf.Clamp(screenPosition.x, _cameraAimClampMin.x, _cameraAimClampMax.x);
            screenPosition.y = Mathf.Clamp(screenPosition.y, _cameraAimClampMin.y, _cameraAimClampMax.y);
            _cinemachineRotationComposer.Composition.ScreenPosition = screenPosition;
        }
    }
}