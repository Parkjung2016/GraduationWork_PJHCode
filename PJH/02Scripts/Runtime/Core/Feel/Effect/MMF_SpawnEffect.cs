using Main.Runtime.Core;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace MoreMountains.Feedbacks
{
    [AddComponentMenu("")]
    [FeedbackPath("Effect/SpawnEffect")]
    [MovedFrom(false, null, "MoreMountains.Feedbacks.MMTools")]
    public class MMF_SpawnEffect : MMF_Feedback
    {
        [MMFInspectorGroup("EffectInfo", true, 61, true)]
        public PoolManagerSO poolManager;

        public PoolTypeSO effectPoolType;
        public Vector3 spawnPosition;
        public Quaternion spawnRotation;

        protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1)
        {
            if (!Active)
                return;
            PoolEffectPlayer effectPlayer = poolManager.Pop(effectPoolType) as PoolEffectPlayer;
            effectPlayer.transform.position = spawnPosition;
            effectPlayer.transform.rotation = spawnRotation;
            effectPlayer.PlayEffects();
        }
    }
}