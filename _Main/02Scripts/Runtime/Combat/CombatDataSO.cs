using System;
using Animancer;
using Sirenix.OdinInspector;
using Unity.Cinemachine;
using UnityEngine;

namespace Main.Runtime.Combat
{
    [CreateAssetMenu(fileName = "CombatData", menuName = "SO/Combat/CombatData")]
    public class CombatDataSO : SerializedScriptableObject
    {
        [Title("Animation Clips")] [LabelText("피격 시 타겟의 애니메이션")]
        public ClipTransition getDamagedAnimationClip;

        [LabelText("공격 애니메이션")] public ClipTransition attackAnimationClip;

        [Space(10)] [Title("Combat Options")] [LabelText("데미지 배율"), SuffixLabel("배", true)]
        public float damageMultiplier = 1.0f;


        [LabelText("넉다운 여부")] public bool isKnockDown;

        [LabelText("방어 무시 여부")] public bool isForceAttack;

        [LabelText("피격 시 콜라이더 비활성화")] public bool disableColliderOnHit = true;

        [ShowIf("isKnockDown")] [LabelText("넉다운 시간"), SuffixLabel("초", true)]
        public float knockDownTime = 1;

        [ShowIf("isKnockDown")] [LabelText("일어날 때 애니메이션")]
        public ClipTransition getUpAnimationClip;

        [Space(10)] [Title("Feedback Data")] [InlineProperty, HideLabel]
        public CombatFeedbackData combatFeedbackData;

        private float _originAttackSpeed;

        public void SetAttackOverrideSpeed(float speed)
        {
            attackAnimationClip.Speed = _originAttackSpeed * speed;
        }

        private void OnEnable()
        {
            _originAttackSpeed = attackAnimationClip.Speed;
        }
    }

    [Serializable]
    public class CombatFeedbackData
    {
        [LabelText("Shake 파워")] public float impulsePower;

        [LabelText("HitStop 시간"), SuffixLabel("초", true)]
        public float freezeFrameDuration;

        [LabelText("Shake 형태")] public CinemachineImpulseDefinition.ImpulseShapes impulseShape;
    }
}