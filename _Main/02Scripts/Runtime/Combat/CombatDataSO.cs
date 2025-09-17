using System;
using System.Collections;
using System.Collections.Generic;
using Animancer;
using Main.Runtime.Combat.Core;
using Main.Runtime.Core.StatSystem;
using Main.Shared;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Unity.Cinemachine;
using UnityEngine;

namespace Main.Runtime.Combat
{
    public class GetDamagedAnimationClipInfo : IEnumerable<KeyValuePair<Define.EDirection, ClipTransition>>
    {
        [SerializeField, DictionaryDrawerSettings(KeyLabel = "Direction", ValueLabel = "Clip")]
        private Dictionary<Define.EDirection, ClipTransition> _getDamagedAnimationClips = new();

        public ClipTransition this[Define.EDirection direction] => _getDamagedAnimationClips[direction];

        public IEnumerator<KeyValuePair<Define.EDirection, ClipTransition>> GetEnumerator()
        {
            return _getDamagedAnimationClips.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    [CreateAssetMenu(fileName = "CombatData", menuName = "SO/Combat/CombatData")]
    public class CombatDataSO : SerializedScriptableObject
    {
        [OdinSerialize] [Title("Animation Clips")] [LabelText("피격 시 타겟의 애니메이션")]
        public List<GetDamagedAnimationClipInfo> getDamagedAnimationClips = new();

        [LabelText("공격 애니메이션")] public ClipTransition attackAnimationClip;

        [Space(10)] [Title("Combat Options")] [LabelText("데미지 배율"), SuffixLabel("배", true)]
        public float damageMultiplier = 1.0f;

        public float increaseMomentumGaugeMultiplier = 1;
        [HideInInspector] public int currentGetDamagedAnimationClipIndex = 0;


        [LabelText("넉다운 여부")] public bool isKnockDown;

        [LabelText("방어 무시 여부")] public bool isForceAttack;

        [LabelText("피격 시 콜라이더 비활성화")] public bool disableColliderOnHit = true;

        [ShowIf("isKnockDown")] [LabelText("넉다운 시간"), SuffixLabel("초", true)]
        public float knockDownTime = 1;

        [ShowIf("isKnockDown")] [LabelText("일어날 때 애니메이션")]
        public ClipTransition getUpAnimationClip;

        public Vector3 boxCastHalfExtentsWhenAttacking = new Vector3(0.6f, 1f, 0.5f);
        public float castDistanceWhenAttacking = 0.2f;

        [Space(10)] [Title("Feedback Data")] [InlineProperty, HideLabel]
        public CombatFeedbackData combatFeedbackData;

        private float _originAttackSpeed;

        public void SetAttackOverrideSpeed(float speed)
        {
            attackAnimationClip.Speed = _originAttackSpeed * speed;
        }

        public float GetIncreaseMomentumGauge(StatSO increaseMomentumGaugeStat) =>
            increaseMomentumGaugeStat.Value * increaseMomentumGaugeMultiplier;

        public float GetPower(StatSO powerStat) => powerStat.Value * damageMultiplier;

        public GetDamagedAnimationClipInfo GetDamagedAnimationClip() =>
            getDamagedAnimationClips[currentGetDamagedAnimationClipIndex];

        private void OnEnable()
        {
            _originAttackSpeed = attackAnimationClip.Speed;
        }


#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            foreach (var info in getDamagedAnimationClips)
            {
                foreach (var pair in info)
                {
                    pair.Value.FadeDuration = .03f;
                }
            }
        }
#endif
    }

    [Serializable]
    public class CombatFeedbackData
    {
        [LabelText("Shake 파워")] public float impulsePower;

        [LabelText("HitStop 시간"), SuffixLabel("초", true)]
        public float freezeFrameDuration;

        [LabelText("Shake 형태")] public CinemachineImpulseDefinition.ImpulseShapes impulseShape;

        [LabelText("ChromaticAberration 강도")] public float chromaticAberrationIntensity;
    }
}