using System;
using Unity.Cinemachine;
using UnityEngine;

namespace PJH.Runtime.Core
{
    public class ResetImpulsePosition : MonoBehaviour
    {
        private CinemachineExternalImpulseListener _impulseListener;
        private Vector3 _originPosition;

        private bool _reseted;

        private void Awake()
        {
            _impulseListener = GetComponent<CinemachineExternalImpulseListener>();
            UpdateOriginPosition();
        }

        public void UpdateOriginPosition()
        {
            _originPosition = transform.localPosition;
        }

        void LateUpdate()
        {
            bool isImpusle = CinemachineImpulseManager.Instance.GetImpulseAt(
                transform.position, _impulseListener.Use2DDistance, _impulseListener.ChannelMask,
                out Vector3 pos, out Quaternion frame);
            if (!isImpusle)
            {
                if (!_reseted)
                {
                    transform.localPosition = _originPosition;
                    _reseted = true;
                }
            }
            else if (_reseted)
            {
                _reseted = false;
            }
        }
    }
}