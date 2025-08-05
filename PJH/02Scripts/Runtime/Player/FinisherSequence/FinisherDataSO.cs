using Animancer;
using Kinemation.MotionWarping.Runtime.Core;
using UnityEngine;

namespace PJH.Runtime.Players.FinisherSequence
{
    public enum FinisherLookAtType
    {
        Attacker,
        Victim
    }

    [CreateAssetMenu(menuName = "SO/Finisher/Data")]
    public class FinisherDataSO : ScriptableObject
    {
        public float spaceToExecute;
        public MotionWarpingAsset executionAsset;
        public ClipTransition executedClip;


        public float sequenceDuration;
        public FinisherLookAtType lookAtTarget;
        public HumanBodyBones lookAtBone = HumanBodyBones.Spine;

        public AnimationCurve sequenceCurve;
        public Vector3[] sequenceCameraWayPoints;
    }
}