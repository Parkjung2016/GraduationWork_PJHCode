using Main.Runtime.Manager;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace MoreMountains.Feedbacks
{
    [AddComponentMenu("")]
    [FeedbackPath("Sound/SlowSound")]
    [MovedFrom(false, null, "MoreMountains.Feedbacks.MMTools")]
    public class MMF_SlowSound : MMF_Feedback
    {
        [MMFInspectorGroup("SlowSoundInfo", true, 61, true)]
        public bool isSlowSound;

        protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1)
        {
            if (!Active)
                return;
            Managers.FMODManager.MainSoundSlow(isSlowSound);
        }
    }
}