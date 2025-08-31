using System.Collections.Generic;
using Main.Core;
using Main.Runtime.Agents;
using Main.Runtime.Core;
using Main.Runtime.Core.Events;
using Main.Shared;
using MoreMountains.Feedbacks;
using TrailsFX;
using UnityEngine;
using Debug = UnityEngine.Debug;


namespace PJH.Runtime.Players
{
    public struct AttachedToBodyEffectInfo
    {
        public string key;
        public PoolEffectPlayer effectPlayer;
        public bool applyRotation;
    }

    public class PlayerEffect : AgentEffect
    {
        [SerializeField] private PoolTypeSO _parryingEffectPoolType;
        [SerializeField] private TrailEffect _meshTrailEffect;
        private Player _player;
        private PlayerAnimator _animatorCompo;
        private GameEventChannelSO _gameEventChannel;
        private MMF_Player _applyDamagedFeedback, _evasionFeedbackWhileHitting;
        private MMF_Player _counterAttackFeedback, _hitCounterAttackTargetFeedback;
        private MMF_Player _hitWarpStrikeTargetFeedback, _avoidingAttackFeedback;
        private MMF_Player _onDeathFeedbackPlayer;
        private Dictionary<HumanBodyBones, List<AttachedToBodyEffectInfo>> _attachedEffects;

        private GameObject _healPotionObject;

        public override void Initialize(Agent agent)
        {
            base.Initialize(agent);
            _attachedEffects = new();
            _onDeathFeedbackPlayer = transform.Find("OnEnemyDeathFeedbackPlayer")?.GetComponent<MMF_Player>();
            _counterAttackFeedback = transform.Find("CounterAttackFeedback").GetComponent<MMF_Player>();
            _hitCounterAttackTargetFeedback =
                transform.Find("HitCounterAttackTargetFeedback").GetComponent<MMF_Player>();
            _applyDamagedFeedback = transform.Find("ApplyDamagedFeedback").GetComponent<MMF_Player>();
            _hitWarpStrikeTargetFeedback = transform.Find("HitWarpStrikeTargetFeedback").GetComponent<MMF_Player>();
            _evasionFeedbackWhileHitting = transform.Find("EvasionFeedbackWhileHitting").GetComponent<MMF_Player>();
            _avoidingAttackFeedback = transform.Find("AvoidingAttackFeedback").GetComponent<MMF_Player>();
            _gameEventChannel = AddressableManager.Load<GameEventChannelSO>("GameEventChannel");
            _player = agent as Player;
            _animatorCompo = _player.GetCompo<PlayerAnimator>();
            _meshTrailEffect.active = false;
        }

        public override void AfterInitialize()
        {
            base.AfterInitialize();
            _gameEventChannel.AddListener<EnemyDead>(HandleEnemyDead);

            _player.HealthCompo.OnApplyDamaged += HandleApplyDamaged;

            _player.OnStartStun += HandleStartStun;
            _player.OnEndStun += HandleEndStun;

            PlayerHealth playerHealth = (_player.HealthCompo as PlayerHealth);
            playerHealth.OnParrying += HandleParrying;
            playerHealth.OnAvoidingAttack += HandleAvoidingAttack;
            PlayerMovement movementCompo = _player.GetCompo<PlayerMovement>();
            movementCompo.OnEvasionWhileHitting += HandleEvasionWhileHitting;
            movementCompo.OnEvasionEndWhileHitting += HandleEvasionEndWhileHitting;

            PlayerCounterAttack counterAttackCompo = _player.GetCompo<PlayerCounterAttack>();
            counterAttackCompo.OnCounterAttackWithoutAnimationClip += HandleCounterAttack;
            counterAttackCompo.OnHitCounterAttackTarget += HandleHitCounterAttackTarget;

            PlayerWarpStrike warpStrikeCompo = _player.GetCompo<PlayerWarpStrike>();
            warpStrikeCompo.OnHitWarpStrikeTarget += HandleHitWarpStrikeTarget;

            PlayerEnemyFinisher finisherCompo = _player.GetCompo<PlayerEnemyFinisher>();
            finisherCompo.OnFinisher += HandleFinisher;
            finisherCompo.OnFinisherEnd += HandleFinisherEnd;

            PlayerAnimationTrigger animationTriggerCompo = _player.GetCompo<PlayerAnimationTrigger>();
            animationTriggerCompo.OnGrabHealItem += HandleGrabHealItem;
            animationTriggerCompo.OnDestroyHealItem += HandleDestroyHealItem;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _gameEventChannel.RemoveListener<EnemyDead>(HandleEnemyDead);

            _player.HealthCompo.OnApplyDamaged -= HandleApplyDamaged;
            _player.OnStartStun -= HandleStartStun;
            _player.OnEndStun -= HandleEndStun;


            PlayerHealth playerHealth = (_player.HealthCompo as PlayerHealth);
            playerHealth.OnParrying -= HandleParrying;
            playerHealth.OnAvoidingAttack -= HandleAvoidingAttack;

            PlayerMovement movementCompo = _player.GetCompo<PlayerMovement>();
            movementCompo.OnEvasionWhileHitting -= HandleEvasionWhileHitting;
            movementCompo.OnEvasionEndWhileHitting -= HandleEvasionEndWhileHitting;

            PlayerCounterAttack counterAttackCompo = _player.GetCompo<PlayerCounterAttack>();
            counterAttackCompo.OnCounterAttackWithoutAnimationClip -= HandleCounterAttack;
            counterAttackCompo.OnHitCounterAttackTarget -= HandleHitCounterAttackTarget;

            PlayerWarpStrike warpStrikeCompo = _player.GetCompo<PlayerWarpStrike>();
            warpStrikeCompo.OnHitWarpStrikeTarget -= HandleHitWarpStrikeTarget;


            PlayerEnemyFinisher finisherCompo = _player.GetCompo<PlayerEnemyFinisher>();
            finisherCompo.OnFinisher -= HandleFinisher;
            finisherCompo.OnFinisherEnd -= HandleFinisherEnd;

            PlayerAnimationTrigger animationTriggerCompo = _player.GetCompo<PlayerAnimationTrigger>();
            animationTriggerCompo.OnGrabHealItem -= HandleGrabHealItem;
            animationTriggerCompo.OnDestroyHealItem -= HandleDestroyHealItem;
        }

        private void HandleDestroyHealItem()
        {
            Destroy(_healPotionObject);
        }

        private void HandleGrabHealItem()
        {
            _healPotionObject = AddressableManager.Instantiate("HealPotion");
            AgentEquipmentSystem agentEquipmentSystemCompo = _player.GetCompo<AgentEquipmentSystem>();
            Transform socketTransform = agentEquipmentSystemCompo.GetSocket(Define.ESocketType.LeftHand).transform;
            _healPotionObject.transform.SetParent(socketTransform, false);
        }

        private void HandleEnemyDead(EnemyDead evt)
        {
            _onDeathFeedbackPlayer?.PlayFeedbacks();
        }

        private void HandleFinisher()
        {
            DistanceFade.Locked = true;
        }

        private void HandleFinisherEnd()
        {
            DistanceFade.Locked = false;
        }

        private void LateUpdate()
        {
            foreach (var effectPair in _attachedEffects)
            {
                foreach (var effectInfo in effectPair.Value)
                {
                    Transform boneTrm = _animatorCompo.Animancer.GetBoneTransform(effectPair.Key);
                    if (effectInfo.applyRotation)
                    {
                        effectInfo.effectPlayer.transform.SetPositionAndRotation(boneTrm.position, boneTrm.rotation);
                    }
                    else
                    {
                        effectInfo.effectPlayer.transform.position = boneTrm.position;
                    }
                }
            }
        }

        private void HandleAvoidingAttack()
        {
            if (!_avoidingAttackFeedback) return;
            if (_avoidingAttackFeedback.IsPlaying)
                _avoidingAttackFeedback.StopFeedbacks();
            _avoidingAttackFeedback.PlayFeedbacks();
        }

        private void HandleHitWarpStrikeTarget()
        {
            _hitWarpStrikeTargetFeedback?.PlayFeedbacks();
        }

        private void HandleHitCounterAttackTarget()
        {
            _hitCounterAttackTargetFeedback?.PlayFeedbacks();
        }

        private void HandleCounterAttack()
        {
            _counterAttackFeedback?.PlayFeedbacks();
        }

        private void HandleEvasionWhileHitting()
        {
            _evasionFeedbackWhileHitting?.PlayFeedbacks();
            _meshTrailEffect.active = true;
        }

        private void HandleEvasionEndWhileHitting()
        {
            _meshTrailEffect.active = false;
        }

        private void HandleStartStun()
        {
            var evt = GameEvents.PlayerStunned;
            evt.isStunned = true;
            _gameEventChannel.RaiseEvent(evt);
        }

        private void HandleEndStun()
        {
            var evt = GameEvents.PlayerStunned;
            evt.isStunned = false;
            _gameEventChannel.RaiseEvent(evt);
        }

        private void HandleParrying()
        {
            var evt = SpawnEvents.SpawnEffect;
            evt.effectType = _parryingEffectPoolType;
            evt.position = _player.HealthCompo.GetDamagedInfo.hitPoint;
            evt.rotation = Quaternion.identity;
            _spawnEventChannel.RaiseEvent(evt);
        }

        private void HandleApplyDamaged(float damage)
        {
            _applyDamagedFeedback.PlayFeedbacks();
        }

        public void PlayEffectAttachedToBody(PoolTypeSO poolTypeSO, HumanBodyBones attachedBone, float playTime,
            bool applyRotation = true)
        {
            PlayEffectAndAddEffects(poolTypeSO, attachedBone, false, playTime, applyRotation: applyRotation);
        }

        public void PlayEffectAttachedToBody(PoolTypeSO poolTypeSO, HumanBodyBones attachedBone,
            bool applyRotation = true)
        {
            PlayEffectAndAddEffects(poolTypeSO, attachedBone, true, applyRotation: applyRotation);
        }

        public void StopEffectAttachedToBody(PoolTypeSO poolTypeSO, HumanBodyBones attachedBone)
        {
            if (_attachedEffects.TryGetValue(attachedBone, out var effectInfos))
            {
                if (effectInfos.Count == 0) return;

                int idx = effectInfos.FindIndex(x => x.key == poolTypeSO.typeName);
                if (idx == -1) return;
                AttachedToBodyEffectInfo effectInfo = effectInfos[idx];
                effectInfo.effectPlayer.StopEffects();
                _attachedEffects[attachedBone].RemoveAt(idx);
                if (_attachedEffects[attachedBone].Count == 0)
                    _attachedEffects.Remove(attachedBone);
            }
        }

        private PoolEffectPlayer PlayEffectAndAddEffects(PoolTypeSO poolTypeSO, HumanBodyBones attachedBone,
            bool isLooped = true, float playTime = 0.0f, bool applyRotation = true)
        {
            PoolEffectPlayer effectPlayer = _poolManager.Pop(poolTypeSO) as PoolEffectPlayer;
            AttachedToBodyEffectInfo info = new AttachedToBodyEffectInfo
            {
                key = poolTypeSO.typeName,
                applyRotation = applyRotation,
                effectPlayer = effectPlayer
            };
            if (_attachedEffects.ContainsKey(attachedBone))
            {
                _attachedEffects[attachedBone].Add(info);
            }
            else
                _attachedEffects.Add(attachedBone, new List<AttachedToBodyEffectInfo> { info });

            effectPlayer.PlayEffects();
            return effectPlayer;
        }
    }
}