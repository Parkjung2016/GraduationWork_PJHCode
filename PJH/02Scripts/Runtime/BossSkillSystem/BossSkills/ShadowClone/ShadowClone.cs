using System.Collections.Generic;
using Animancer;
using DG.Tweening;
using Main.Runtime.Agents;
using Main.Runtime.Manager;
using Opsive.BehaviorDesigner.Runtime;
using PJH.Runtime.Players;
using UnityEngine;

namespace PJH.Runtime.BossSkill.BossSkills.ShadowClones
{
    public enum MoveDirection
    {
        Left,
        Right
    }

    public class ShadowClone : Agent, IPoolable
    {
        [field: SerializeField] public PoolTypeSO PoolType { get; set; }
        [SerializeField] private Dictionary<MoveDirection, ClipTransition> _avoidAnimations;
        public GameObject GameObject => gameObject;
        public BehaviorTree BT { get; private set; }
        private Pool _pool;
        private Player _player;

        public void SetUpPool(Pool pool)
        {
            _pool = pool;
            BT = GetComponent<BehaviorTree>();
            ShadowCloneAnimationTrigger animationTriggerCompo = GetCompo<ShadowCloneAnimationTrigger>();
            animationTriggerCompo.OnComboPossible += HandleComboPossible;
            animationTriggerCompo.OnLookPlayer += LookPlayer;
        }

        public void ResetItem()
        {
            _player = PlayerManager.Instance.Player as Player;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            ShadowCloneAnimationTrigger animationTriggerCompo = GetCompo<ShadowCloneAnimationTrigger>();
            animationTriggerCompo.OnComboPossible -= HandleComboPossible;
            animationTriggerCompo.OnLookPlayer -= LookPlayer;
        }

        private void HandleComboPossible()
        {
            BT.SetVariableValue("ComboPossible", true);
        }

        public void StartBehaviour(MoveDirection direction)
        {
            ShadowCloneAnimator animatorCompo = GetCompo<ShadowCloneAnimator>();
            animatorCompo.SetRootMotion(true);
            animatorCompo.PlayAnimationClip(_avoidAnimations[direction], () =>
            {
                animatorCompo.SetRootMotion(false);
                BT.StartBehavior();
            });
        }

        public void LookPlayer()
        {
            transform.DOLookAt(_player.transform.position, .25f, AxisConstraint.Y);
        }

        public void SetLifeTime(float lifeTime)
        {
            BT.SetVariableValue("LifeTime", lifeTime);
        }

        public void Disappear()
        {
            _pool.Push(this);
        }
    }
}