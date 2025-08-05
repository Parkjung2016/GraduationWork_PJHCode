using System;
using System.Collections.Generic;
using Animancer;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using INab.Dissolve;
using Main.Core;
using Main.Runtime.Agents;
using Main.Runtime.Characters.StateMachine;
using Main.Runtime.Core.Events;
using Main.Runtime.Manager;
using Opsive.BehaviorDesigner.Runtime;
using PJH.Runtime.Players;
using UnityEngine;
using YTH.Boss;

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
        public BehaviorTree BT { get; private set; }
        private Pool _pool;
        private Player _player;
        private Dissolver _dissolver;

        private GameEventChannelSO _gameEventChannel;

        public void SetUpPool(Pool pool)
        {
            _gameEventChannel = AddressableManager.Load<GameEventChannelSO>("GameEventChannel");
            _pool = pool;
            _dissolver = GetComponent<Dissolver>();
            _dissolver.materials.Add(GetComponentInChildren<SkinnedMeshRenderer>().material);
            BT = GetComponent<BehaviorTree>();
            ShadowCloneAnimationTrigger animationTriggerCompo = GetCompo<ShadowCloneAnimationTrigger>();
            animationTriggerCompo.OnComboPossible += HandleComboPossible;
            animationTriggerCompo.OnLookPlayer += LookPlayer;
            _gameEventChannel.AddListener<DestroyDeadEnemy>(ReturnPool);
        }

        private void ReturnPool(DestroyDeadEnemy obj)
        {
            _pool.Push(this);
        }

        public void ResetItem()
        {
            GetCompo<AgentWeaponManager>().CurrentWeapon.gameObject.SetActive(true);
            GetCompo<ShadowCloneStateSystem>().ChangeState(null, true);
            _dissolver.Reset();
            _player = PlayerManager.Instance.Player as Player;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            ShadowCloneAnimationTrigger animationTriggerCompo = GetCompo<ShadowCloneAnimationTrigger>();
            animationTriggerCompo.OnComboPossible -= HandleComboPossible;
            animationTriggerCompo.OnLookPlayer -= LookPlayer;
            _gameEventChannel.RemoveListener<DestroyDeadEnemy>(ReturnPool);
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

        public async void Disappear()
        {
            try
            {
                BT.StopBehavior();
                GetCompo<ShadowCloneMovement>().SetCanMove(false);
                _dissolver.Dissolve();
                GetCompo<AgentWeaponManager>().CurrentWeapon.gameObject.SetActive(false);
                await UniTask.WaitForSeconds(_dissolver.duration + .5f,
                    cancellationToken: gameObject.GetCancellationTokenOnDestroy());

                _pool.Push(this);
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}