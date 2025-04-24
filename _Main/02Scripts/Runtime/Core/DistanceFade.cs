using System;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;

namespace Main.Runtime.Core
{
    [RequireComponent(typeof(BoxCollider))]
    public class DistanceFade : MonoBehaviour
    {
        private readonly int FadeAmountHash = Shader.PropertyToID("_FadeAmount");
        [SerializeField] private Vector2 _minMaxDistance;
        private Renderer[] _renderers;

        private CinemachineBrain _cinemachineBrain;
        private Transform _mainPlayerCameraTrm;
        private Collider _collider;

        private void Awake()
        {
            _collider = GetComponent<Collider>();
            _cinemachineBrain = Camera.main.GetComponent<CinemachineBrain>();
            _mainPlayerCameraTrm = GameObject.FindGameObjectWithTag("MainPlayerCamera")?.transform;
            _renderers = transform.parent.GetComponentsInChildren<Renderer>()
                .Where(x => x.material.HasFloat(FadeAmountHash)).ToArray();
        }

        private void Update()
        {
            if (!_mainPlayerCameraTrm) return;
            float value = 0;
            if (_cinemachineBrain.ActiveVirtualCamera != null &&
                (_cinemachineBrain.ActiveVirtualCamera as MonoBehaviour).transform == _mainPlayerCameraTrm)
            {
                Vector3 closestPoint = _collider.bounds.ClosestPoint(_cinemachineBrain.transform.position);
                value = Vector3.Distance(closestPoint, _cinemachineBrain.transform.position);

                value = Remap(value, _minMaxDistance.y, 0, _minMaxDistance.x, 1);
            }

            for (int i = 0; i < _renderers.Length; i++)
            {
                _renderers[i].material
                    .SetFloat(FadeAmountHash, value);
            }
        }

        private float Remap(float value, float from1, float to1, float from2, float to2)
        {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }
    }
}