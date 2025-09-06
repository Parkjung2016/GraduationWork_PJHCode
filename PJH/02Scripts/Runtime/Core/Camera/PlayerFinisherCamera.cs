using DG.Tweening;
using Main.Runtime.Core.Events;
using PJH.Runtime.Players.FinisherSequence;
using PJH.Utility.Managers;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Splines;

namespace PJH.Runtime.Core.PlayerCamera
{
    public class PlayerFinisherCamera : MonoBehaviour
    {
        private CinemachineCamera _cinemachineCamera;
        private CinemachineSplineDolly _cinemachineSplineDolly;
        private GameEventChannelSO _gameEventChannel;

        private PositionConstraint _lookAtPositionConstraint;

        private void Awake()
        {
            _cinemachineCamera = GetComponent<CinemachineCamera>();
            _cinemachineSplineDolly = GetComponent<CinemachineSplineDolly>();
            _lookAtPositionConstraint = _cinemachineCamera.Target.TrackingTarget.GetComponent<PositionConstraint>();
            _gameEventChannel = AddressableManager.Load<GameEventChannelSO>("GameEventChannel");

            _gameEventChannel.AddListener<EnemyFinisherSequence>(HandleEnemyFinisherSequence);
            _gameEventChannel.AddListener<FinishEnemyFinisher>(HandleFinishEnemyFinisher);
        }

        private void OnDestroy()
        {
            _gameEventChannel.RemoveListener<EnemyFinisherSequence>(HandleEnemyFinisherSequence);
            _gameEventChannel.RemoveListener<FinishEnemyFinisher>(HandleFinishEnemyFinisher);
        }

        private void HandleFinishEnemyFinisher(FinishEnemyFinisher evt)
        {
            _cinemachineCamera.Priority = -1;
        }

        private void HandleEnemyFinisherSequence(EnemyFinisherSequence evt)
        {
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
            Transform lookAtTransform = null;
            switch (evt.sequenceAsset.lookAtTarget)
            {
                case FinisherLookAtType.Attacker:
                    lookAtTransform = evt.playerAnimator.GetBoneTransform(lookAtBone);
                    break;
                case FinisherLookAtType.Victim:
                    lookAtTransform = evt.targetAnimator.GetBoneTransform(lookAtBone);
                    break;
            }

            UpdateLookAtTransform(lookAtTransform);
        }

        private void UpdateLookAtTransform(Transform lookAtTarget)
        {
            ConstraintSource source = _lookAtPositionConstraint.GetSource(0);
            source.sourceTransform = lookAtTarget;
            _lookAtPositionConstraint.SetSource(0, source);
        }
    }
}