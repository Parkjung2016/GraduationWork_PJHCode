using System;
using Cysharp.Threading.Tasks;
using PJH.Utility.Managers;
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


        protected override async void Awake()
        {
            base.Awake();
            try
            {
                _cinemachineRotationComposer = _cinemachineCamera.GetComponent<CinemachineRotationComposer>();

                await UniTask.WaitUntil(() => AddressableManager.isLoaded,
                    cancellationToken: gameObject.GetCancellationTokenOnDestroy());
                PlayBGM();
            }
            catch (Exception e)
            {
                // ignored
            }
        }

        protected override void Start()
        {
            base.Start();
            Application.targetFrameRate = 30;
            CursorManager.SetCursorLockMode(CursorLockMode.None);
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