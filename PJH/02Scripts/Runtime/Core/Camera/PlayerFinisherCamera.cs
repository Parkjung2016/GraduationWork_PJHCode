using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Main.Core;
using Main.Runtime.Core.Events;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Splines;
using Debug = Main.Core.Debug;

namespace PJH.Runtime.Core.PlayerCamera
{
    public class PlayerFinisherCamera : MonoBehaviour
    {
        private CinemachineCamera _cinemachineCamera;
        private CinemachineSplineDolly _cinemachineSplineDolly;
        private GameEventChannelSO _gameEventChannel;


        private CancellationTokenSource _updateLookAtTransformToken;

        private void Awake()
        {
            _cinemachineCamera = GetComponent<CinemachineCamera>();
            _cinemachineSplineDolly = GetComponent<CinemachineSplineDolly>();
            _gameEventChannel = AddressableManager.Load<GameEventChannelSO>("GameEventChannel");

            _gameEventChannel.AddListener<EnemyFinisherSequence>(HandleEnemyFinisherSequence);
            _gameEventChannel.AddListener<FinishEnemyFinisher>(HandleFinishEnemyFinisher);
        }

        private void OnDestroy()
        {
            if (_updateLookAtTransformToken is { IsCancellationRequested: false })
            {
                _updateLookAtTransformToken.Cancel();
                _updateLookAtTransformToken.Dispose();
            }

            _gameEventChannel.RemoveListener<EnemyFinisherSequence>(HandleEnemyFinisherSequence);
            _gameEventChannel.RemoveListener<FinishEnemyFinisher>(HandleFinishEnemyFinisher);
        }

        private void HandleFinishEnemyFinisher(FinishEnemyFinisher evt)
        {
            _cinemachineCamera.Priority = -1;
        }

        private void HandleEnemyFinisherSequence(EnemyFinisherSequence evt)
        {
            if (_updateLookAtTransformToken is { IsCancellationRequested: false })
            {
                _updateLookAtTransformToken.Cancel();
                _updateLookAtTransformToken.Dispose();
            }

            _cinemachineCamera.Priority = 5;
            _cinemachineSplineDolly.enabled = false;
            _cinemachineSplineDolly.Spline.Spline.Clear();
            for (int i = 0; i < evt.sequenceAsset.sequenceCameraWayPoints.Length; i++)
            {
                BezierKnot bezierKnot = new BezierKnot
                {
                    Position = evt.sequenceAsset.sequenceCameraWayPoints[i]
                };
                _cinemachineSplineDolly.Spline.Spline.Add(bezierKnot, TangentMode.AutoSmooth);
            }

            _cinemachineSplineDolly.enabled = true;

            DOVirtual.Float(0, 1, evt.sequenceAsset.sequenceDuration,
                    x => _cinemachineSplineDolly.CameraPosition = x)
                .SetEase(evt.sequenceAsset.sequenceCurve);

            HumanBodyBones lookAtBone = evt.sequenceAsset.lookAtBone;
            Transform lookAtTransform = evt.playerAnimator.GetBoneTransform(lookAtBone);
            UpdateLookAtTransform(lookAtTransform);
        }

        private async void UpdateLookAtTransform(Transform lookAtTarget)
        {
            try
            {
                _updateLookAtTransformToken = new CancellationTokenSource();
                while (true)
                {
                    if (_updateLookAtTransformToken.IsCancellationRequested) return;
                    Transform trackingTrm = _cinemachineCamera.Target.TrackingTarget;
                    trackingTrm.position = lookAtTarget.position;
                    await UniTask.Yield(cancellationToken: _updateLookAtTransformToken.Token);
                }
            }
            catch (Exception e)
            {
            }
        }
    }
}