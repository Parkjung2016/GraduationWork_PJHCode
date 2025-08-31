using System;
using Main.Core;
using Main.Runtime.Agents;
using Main.Runtime.Core.Events;
using Unity.Cinemachine;
using UnityEngine;
using Debug = Main.Core.Debug;

namespace PJH.Runtime.Core.PlayerCamera
{
    public class PlayerLockOnCamera : MonoBehaviour
    {
        private GameEventChannelSO _uiEventChannel, _gameEventChannel;
        private bool _isLockOn;

        private CinemachineCamera _cinemachineCamera;

        private Transform _lockOnTargetTrm;

        [SerializeField, Range(1, 10f)] private float _lookSpeed = 1f;

        private float _originTrackingTargetY;

        private void Awake()
        {
            _cinemachineCamera = GetComponent<CinemachineCamera>();
            _uiEventChannel = AddressableManager.Load<GameEventChannelSO>("UIEventChannelSO");
            _gameEventChannel = AddressableManager.Load<GameEventChannelSO>("GameEventChannel");
            _uiEventChannel.AddListener<ShowLockOnUI>(HandleShowLockOnUI);
            _originTrackingTargetY = _cinemachineCamera.Target.TrackingTarget.localPosition.y;
            _gameEventChannel.AddListener<LockOn>(HandleLockOn);
        }

        private void OnDestroy()
        {
            _uiEventChannel.RemoveListener<ShowLockOnUI>(HandleShowLockOnUI);
            _gameEventChannel.RemoveListener<LockOn>(HandleLockOn);
        }

        private void HandleLockOn(LockOn evt)
        {
            _isLockOn = evt.isLockOn;
            bool visibleCamera = _lockOnTargetTrm && _isLockOn;
            _cinemachineCamera.Priority = visibleCamera ? 3 : -3;
            if (visibleCamera)
            {
                _cinemachineCamera.Target.TrackingTarget.localPosition = Vector3.up * .9f;
            }
            else
                _cinemachineCamera.Target.TrackingTarget.localPosition = Vector3.up * _originTrackingTargetY;
        }

        private void HandleShowLockOnUI(ShowLockOnUI evt)
        {
            _lockOnTargetTrm = evt.lockOnTarget?.GameObject.transform;
            if (!_isLockOn) return;
            if (_lockOnTargetTrm)
            {
                _cinemachineCamera.Target.TrackingTarget.localPosition = Vector3.up * .9f;
            }
            else
            {
                _cinemachineCamera.Target.TrackingTarget.localPosition = Vector3.up * _originTrackingTargetY;
            }

            _cinemachineCamera.Priority = _lockOnTargetTrm ? 3 : -3;
        }

        private void FixedUpdate()
        {
            if (!_isLockOn || !_lockOnTargetTrm) return;


            Vector3 lookDir = (_lockOnTargetTrm.position - _cinemachineCamera.Target.TrackingTarget.position)
                .normalized;
            lookDir.y = 0;
            Quaternion look = Quaternion.LookRotation(lookDir);
            Quaternion targetRot = look;
            targetRot = Quaternion.Euler(_cinemachineCamera.Target.TrackingTarget.eulerAngles.x,
                targetRot.eulerAngles.y, 0);
            _cinemachineCamera.Target.TrackingTarget.rotation =
                Quaternion.Slerp(transform.rotation, targetRot, _lookSpeed * Time.deltaTime);
        }
    }
}