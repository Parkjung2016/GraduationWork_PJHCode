using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Main.Runtime.Agents;
using UnityEngine;

namespace PJH.Runtime.Core.PlayerCamera
{
    public class PlayerCameraTargetDetection : MonoBehaviour
    {
        [SerializeField, Range(0, .1f)] private float _detectRate = .05f;
        [SerializeField, Range(0, 30f)] private float _detectDistance = 5f;
        [SerializeField] private LayerMask _whatIsTarget;

        private CancellationTokenSource _updateVisibleTargetsTokenSource;

        public Agent Target { get; private set; }

        private void Awake()
        {
            _updateVisibleTargetsTokenSource = new();
            UpdateVisibleTargets();
        }

        private async void UpdateVisibleTargets()
        {
            try
            {
                while (true)
                {
                    if (_updateVisibleTargetsTokenSource.IsCancellationRequested) return;
                    await UniTask.WaitUntil(() => gameObject.activeSelf);
                    await UniTask.WaitUntil(() => enabled);
                    Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
                    if (Physics.Raycast(ray, out RaycastHit hitInfo, _detectDistance,
                            _whatIsTarget))
                    {
                        Target = hitInfo.transform.GetComponent<Agent>();
                    }

                    await UniTask.WaitForSeconds(_detectRate,
                        cancellationToken: _updateVisibleTargetsTokenSource.Token, ignoreTimeScale: true);
                }
            }
            catch (Exception e)
            {
                
            }
        }

        private void OnDestroy()
        {
            if (_updateVisibleTargetsTokenSource is { IsCancellationRequested: false })
            {
                _updateVisibleTargetsTokenSource.Cancel();
                _updateVisibleTargetsTokenSource.Dispose();
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.DrawRay(transform.position, transform.forward * _detectDistance);
        }
#endif
    }
}