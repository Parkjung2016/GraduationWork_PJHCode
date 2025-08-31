using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace PJH.Runtime.Core
{
    public class OpenGate : MonoBehaviour
    {
        [SerializeField] private GameObject[] _leftGates;
        [SerializeField] private GameObject[] _rightGates;
        [SerializeField] private float _openDistance = 0.9f;
        [SerializeField] private float _openDuration = .5f;
        [SerializeField] private float _openDelay = .5f;


        private Collider[] _colliderComponents;
        private float[] _leftOriginX;
        private float[] _rightOriginX;

        private async void Awake()
        {
            _colliderComponents = GetComponents<Collider>();
            _leftOriginX = new float[_leftGates.Length];
            _rightOriginX = new float[_rightGates.Length];
        }

        private async void Start()
        {
            await UniTask.WaitForSeconds(_openDelay, cancellationToken: gameObject.GetCancellationTokenOnDestroy());
            for (int i = 0; i < _leftGates.Length; i++)
            {
                float xPosition = _leftGates[i].transform.localPosition.x;
                _leftOriginX[i] = xPosition;
                _leftGates[i].transform.DOLocalMoveX(xPosition + _openDistance, _openDuration);
            }

            for (int i = 0; i < _rightGates.Length; i++)
            {
                float xPosition = _rightGates[i].transform.localPosition.x;
                _rightOriginX[i] = xPosition;
                _rightGates[i].transform.DOLocalMoveX(xPosition - _openDistance, _openDuration);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                for (int i = 0; i < _colliderComponents.Length; i++)
                    _colliderComponents[i].enabled = false;
                for (int i = 0; i < _leftGates.Length; i++)
                {
                    _leftGates[i].transform.DOLocalMoveX(_leftOriginX[i], _openDuration);
                }

                for (int i = 0; i < _rightGates.Length; i++)
                {
                    _rightGates[i].transform.DOLocalMoveX(_rightOriginX[i], _openDuration);
                }
            }
        }
    }
}