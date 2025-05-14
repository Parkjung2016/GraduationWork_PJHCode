using Animancer;
using DG.Tweening;
using Main.Runtime.Agents;
using Main.Runtime.Combat;
using Main.Runtime.Combat.Core;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace PJH.Runtime.PlayerPassive.Passives
{
    public class PlayerClone : MonoBehaviour, IPoolable
    {
        private static readonly int AlphaHash = Shader.PropertyToID("_Alpha");
        [SerializeField] private StringAsset _cloneHitTargetEvent;

        private HybridAnimancerComponent _hybridAnimancer;
        private Material[] _modelMaterials;
        private Agent _hitTarget;
        private CombatDataSO _combatData;
        private Sequence _seq;
        private MMF_Player _hitFeedback;
        [field: SerializeField] public PoolTypeSO PoolType { get; private set; }
        public GameObject GameObject => gameObject;

        private Pool _pool;

        public void SetUpPool(Pool pool)
        {
            _pool = pool;
            _hitFeedback = transform.Find("HitFeedback").GetComponent<MMF_Player>();
            _hybridAnimancer = GetComponent<HybridAnimancerComponent>();
            Renderer[] renderers = transform.GetComponentsInChildren<Renderer>();
            _modelMaterials = new Material[renderers.Length];
            for (int i = 0; i < renderers.Length; i++)
            {
                _modelMaterials[i] = renderers[i].material;
            }

            _hybridAnimancer.Events.AddTo(_cloneHitTargetEvent, HitTarget);
        }

        public void ResetItem()
        {
            _hitFeedback.StopFeedbacks();
            for (int i = 0; i < _modelMaterials.Length; i++)
            {
                _modelMaterials[i].SetFloat(AlphaHash, 1);
            }
        }

        public void Attack(Agent hitTarget, CombatDataSO combatData)
        {
            _combatData = combatData;
            _hitTarget = hitTarget;
            AnimancerState state = _hybridAnimancer.Play(_combatData.attackAnimationClip);
            state.Events(this).OnEnd ??= Hide;
        }

        private void HitTarget()
        {
            GetDamagedInfo getDamagedInfo = new()
            {
                attacker = null,
                damage = 2,
                getDamagedAnimationClip = _combatData.getDamagedAnimationClip,
                hitPoint = transform.position,
                increaseMomentumGauge = 2,
                getUpAnimationClip = _combatData.getUpAnimationClip,
                isForceAttack = _combatData.isForceAttack,
                isKnockDown = _combatData.isKnockDown,
                knockDownTime = _combatData.knockDownTime
            };
            _hitFeedback?.PlayFeedbacks();
            _hitTarget.HealthCompo.ApplyDamage(getDamagedInfo);
        }

        private void Hide()
        {
            _hybridAnimancer.States[_combatData.attackAnimationClip].Events(this).OnEnd = null;

            float duration = .2f;
            _seq = DOTween.Sequence();

            _seq.Append(_modelMaterials[0].DOFloat(0, AlphaHash, duration));
            for (int i = 1; i < _modelMaterials.Length; i++)
            {
                _seq.Join(_modelMaterials[i].DOFloat(0, AlphaHash, duration));
            }

            _seq.OnComplete(() =>
            {
                _seq.Kill();
                _pool.Push(this);
            });
        }
    }
}