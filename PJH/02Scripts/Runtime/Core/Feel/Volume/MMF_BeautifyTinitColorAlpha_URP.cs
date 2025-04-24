using DG.Tweening;
using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.Serialization;

namespace MoreMountains.FeedbacksForThirdParty
{
    [AddComponentMenu("")]
    [FeedbackPath("PostProcess/Beautify Tint Color Alpha URP")]
    [MovedFrom(false, null, "MoreMountains.Feedbacks.URP")]
    public class MMF_BeautifyTinitColorAlpha_URP : MMF_Feedback
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

        public override float FeedbackDuration
        {
            get { return ApplyTimeMultiplier(ShakeDuration); }
            set { ShakeDuration = value; }
        }

        public override bool HasChannel => true;
        public override bool HasRandomness => true;

        [MMFInspectorGroup("TintColorAlpha", true, 41)]
        public float ShakeDuration = 0.2f;

        public float alphaValue = 0.2f;


        [FormerlySerializedAs("alphAnimationCurve")] [MMFInspectorGroup("Alpha", true, 42)]
        /// the curve to animate the intensity on
        public AnimationCurve alphaAnimationCurve =
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
            Color color = _beautify.tintColor.value;
            color.a = 0;
            _beautify.tintColor.value = color;
            color.a = alphaValue;
            _tween = DOTween.To(() => _beautify.tintColor.value, x =>
                {
                    Color color = _beautify.tintColor.value;
                    color.a = x.a;
                    _beautify.tintColor.Override(color);
                }, color, ShakeDuration)
                .SetEase(alphaAnimationCurve);
        }
    }
}