using DG.Tweening;
using MoreMountains.Feedbacks;
using OccaSoftware.RadialBlur.Runtime;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Scripting.APIUpdating;

namespace MoreMountains.FeedbacksForThirdParty
{
    [AddComponentMenu("")]
    [FeedbackHelp(
        "This feedback will let you trigger a radial blur post process on the URP camera.")]
    [FeedbackPath("PostProcess/Radial Blur")]
    [MovedFrom(false, null, "MoreMountains.Feedbacks.URP")]
    public class MMF_RadialBlur : MMF_Feedback
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
            get { return ApplyTimeMultiplier(Duration); }
            set { Duration = value; }
        }

        public override bool HasChannel => true;
        public override bool HasRandomness => true;

        [MMFInspectorGroup("Radial Blur", true, 42)]
        /// the duration of the shake, in seconds
        [Tooltip("the duration of the shake, in seconds")]
        public float Duration = 0.2f;

        public float blurValue = 0.2f;


        [MMFInspectorGroup("Intensity", true, 43)]
        /// the curve to animate the intensity on
        [Tooltip("the curve to animate the intensity on")]
        public AnimationCurve Intensity =
            new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));

        private RadialBlurPostProcess _radialBlur;
        private Tween _tween;

        public override void PreInitialization(MMF_Player owner, int index)
        {
            Object.FindAnyObjectByType<Volume>().profile.TryGet(out _radialBlur);
            base.PreInitialization(owner, index);
        }

        protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1.0f)
        {
            if (!Active || !FeedbackTypeAuthorized)
            {
                return;
            }
            if (_tween != null && _tween.IsActive()) _tween.Kill();

            _radialBlur.intensity.value = 0;
            _tween=  DOTween.To(() => _radialBlur.intensity.value, x => _radialBlur.intensity.Override(x), blurValue, Duration)
                .SetEase(Intensity);
        }
    }
}