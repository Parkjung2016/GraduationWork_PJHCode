using DG.Tweening;
using Main.Runtime.Manager;
using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.Serialization;

namespace MoreMountains.FeedbacksForThirdParty
{
    [AddComponentMenu("")]
    [FeedbackHelp(
        "This feedback allows you to control blood frame intensity over time. It requires you have in your scene an object with a Volume " +
        "with Frame active, and a MMF_BeautifyBloodScreen_URP component.")]
    [FeedbackPath("PostProcess/Beautify Blood Screen URP")]
    [MovedFrom(false, null, "MoreMountains.Feedbacks.URP")]
    public class MMF_BeautifyBloodScreen_URP : MMF_Feedback
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

        [MMFInspectorGroup("BloodScreen", true, 41)]
        public float ShakeDuration = 0.2f;

        public float alphaValue = 0.2f;

        [MMFInspectorGroup("Alpha", true, 42)]
        /// the curve to animate the intensity on
        public AnimationCurve alphaAnimationCurve =
            new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));

        private Tween _tween;

        protected override void CustomPlayFeedback(Vector3 position, float attenuation = 1.0f)
        {
            if (!Active || !FeedbackTypeAuthorized)
            {
                return;
            }

            if (_tween != null && _tween.IsActive()) _tween.Kill();
            Beautify.Universal.Beautify beautify = Managers.VolumeManager.beautify;
            Color color = beautify.frameColor.value;
            color.a = 0;
            beautify.frameColor.value = color;
            color.a = alphaValue;
            _tween = DOTween.To(() => beautify.frameColor.value, x =>
                {
                    Color color = beautify.frameColor.value;
                    color.a = x.a;
                    beautify.frameColor.Override(color);
                }, color, ShakeDuration)
                .SetEase(alphaAnimationCurve);
        }
    }
}