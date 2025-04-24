using DG.Tweening;
using UnityEngine;
using MoreMountains.Feedbacks;
using UnityEngine.Rendering;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.Rendering.Universal;

namespace MoreMountains.FeedbacksForThirdParty
{
    /// <summary>
    /// This feedback allows you to control bloom intensity and threshold over time. It requires you have in your scene an object with a Volume with Bloom active, and a MMBloomShaker_URP component.
    /// </summary>
    [AddComponentMenu("")]
    [FeedbackHelp(
        "This feedback allows you to control bloom intensity and threshold over time. It requires you have in your scene an object with a Volume " +
        "with Bloom active, and a MMBeautifyBloomShaker_URP component.")]
    [FeedbackPath("PostProcess/Beautify Bloom URP")]
    [MovedFrom(false, null, "MoreMountains.Feedbacks.URP")]
    public class MMF_BeautifyBloom_URP : MMF_Feedback
    {
        /// a static bool used to disable all feedbacks of this type at once
        public static bool FeedbackTypeAuthorized = true;

        /// sets the inspector color for this feedback
#if UNITY_EDITOR
        public override Color FeedbackColor
        {
            get { return MMFeedbacksInspectorColors.PostProcessColor; }
        }

        public override bool HasCustomInspectors => true;
        public override bool HasAutomaticShakerSetup => true;
#endif

        /// the duration of this feedback is the duration of the shake
        public override float FeedbackDuration
        {
            get { return ApplyTimeMultiplier(ShakeDuration); }
            set { ShakeDuration = value; }
        }

        public override bool HasChannel => true;
        public override bool HasRandomness => true;

        [MMFInspectorGroup("Bloom", true, 41)]
        /// the duration of the feedback, in seconds
        [Tooltip("the duration of the feedback, in seconds")]
        public float ShakeDuration = 0.2f;

        public float bloomIntensity = 0.2f;

        [MMFInspectorGroup("Intensity", true, 42)]
        /// the curve to animate the intensity on
        [Tooltip("the curve to animate the intensity on")]
        public AnimationCurve ShakeIntensity =
            new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));

        private Beautify.Universal.Beautify _beautify;
        private Tween _tween;

        public override void PreInitialization(MMF_Player owner, int index)
        {
            Object.FindAnyObjectByType<Volume>().profile.TryGet(out _beautify);
            base.PreInitialization(owner, index);
        }

        protected override void CustomPlayFeedback(Vector3 position, float attenuation = 1.0f)
        {
            if (!Active || !FeedbackTypeAuthorized)
            {
                return;
            }
            if (_tween != null && _tween.IsActive()) _tween.Kill();

            _beautify.bloomIntensity.value = 0;
            _tween=  DOTween.To(() => _beautify.bloomIntensity.value,
                x => _beautify.bloomIntensity.Override(x), bloomIntensity, ShakeDuration).SetEase(ShakeIntensity);
        }
    }
}