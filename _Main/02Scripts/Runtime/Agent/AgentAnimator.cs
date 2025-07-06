using System;
using System.Threading;
using Animancer;
using Cysharp.Threading.Tasks;
using Main.Runtime.Animators;
using Main.Runtime.Combat.Core;
using Main.Runtime.Core;
using Main.Shared;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Main.Runtime.Agents
{
    public class AgentAnimator : AnimatorRenderer, IAgentComponent, IAfterInitable
    {
        public event Action OnEndHitAnimation;
        public event Action OnKnockDown;
        [SerializeField] private bool _useRagdollOnDeath;

        [SerializeField, HideIf("_useRagdollOnDeath")]
        private ClipTransition _deathAnimationClip;

        protected Agent _agent;
        CancellationTokenSource _knockDownToken;

        private float _effectiveAnimationSpeed;

        public virtual void Initialize(Agent agent)
        {
            _effectiveAnimationSpeed = 1.0f;
            _agent = agent;
        }

        public virtual void AfterInitialize()
        {
            _agent.HealthCompo.OnApplyDamaged += HandleApplyDamaged;
            _agent.HealthCompo.OnDeath += HandleDeath;
            _agent.HealthCompo.ailmentStat.OnAilmentChanged += HandleAilmentChanged;

            if (_agent.TryGetCompo<AgentFullMountable>(out var fullMountableCompo))
            {
                fullMountableCompo.OnFullMounted += HandleFullMounted;
            }

            AgentAnimationTrigger agentAnimationTriggerCompo = _agent.GetCompo<AgentAnimationTrigger>(true);
            agentAnimationTriggerCompo.OnTriggerRagdoll += HandleTriggerRagdoll;
        }

        protected virtual void OnDestroy()
        {
            if (_knockDownToken != null && !_knockDownToken.IsCancellationRequested)
            {
                _knockDownToken.Cancel();
                _knockDownToken.Dispose();
            }

            _agent.HealthCompo.OnApplyDamaged -= HandleApplyDamaged;
            _agent.HealthCompo.OnDeath -= HandleDeath;
            if (_agent.HealthCompo.ailmentStat != null)
                _agent.HealthCompo.ailmentStat.OnAilmentChanged -= HandleAilmentChanged;
            if (_agent.TryGetCompo<AgentFullMountable>(out var fullMountableCompo))
            {
                fullMountableCompo.OnFullMounted -= HandleFullMounted;
            }

            AgentAnimationTrigger agentAnimationTriggerCompo = _agent.GetCompo<AgentAnimationTrigger>(true);
            agentAnimationTriggerCompo.OnTriggerRagdoll -= HandleTriggerRagdoll;
        }

        private void HandleAilmentChanged(Ailment oldAilment, Ailment newAilment)
        {
            if ((newAilment & Ailment.Slow) > 0)
            {
                float value = _agent.HealthCompo.ailmentStat.GetAilmentValue(Ailment.Slow) * 0.01f;
                _effectiveAnimationSpeed = 1 - value;
                foreach (var state in _hybridAnimancer.States)
                {
                    state.Speed = _effectiveAnimationSpeed;
                }
                _hybridAnimancer.Controller.Speed = _effectiveAnimationSpeed;
                
            }
            else
            {
                _effectiveAnimationSpeed = 1.0f;
                foreach (var state in _hybridAnimancer.States)
                {
                    state.Speed = _effectiveAnimationSpeed;
                }

                _hybridAnimancer.Controller.Speed = _effectiveAnimationSpeed;
            }
        }

        private void HandleTriggerRagdoll()
        {
            _effectiveAnimationSpeed = 1.0f;
            Animator.enabled = false;
        }

        protected virtual void HandleFullMounted(ITransition animationClip)
        {
            if (_knockDownToken != null && !_knockDownToken.IsCancellationRequested)
            {
                _knockDownToken.Cancel();
                _knockDownToken.Dispose();
            }

            PlayAnimationClip(animationClip,
                async () =>
                {
                    // await UniTask.WaitForSeconds(.35f, cancellationToken: this.GetCancellationTokenOnDestroy());
                    PlayGetUpAnimation();
                }, false);
        }

        protected virtual void HandleDeath()
        {
            if (!_useRagdollOnDeath)
            {
                _hybridAnimancer.Play(_deathAnimationClip);
            }
            else
            {
                AgentAnimationTrigger agentAnimationTriggerCompo = _agent.GetCompo<AgentAnimationTrigger>(true);
                agentAnimationTriggerCompo.OnTriggerRagdoll?.Invoke();
            }
        }

        protected virtual void HandleApplyDamaged(float damage)
        {
            if (_knockDownToken != null && !_knockDownToken.IsCancellationRequested)
            {
                _knockDownToken.Cancel();
                _knockDownToken.Dispose();
            }

            GetDamagedInfo getDamagedInfo = _agent.HealthCompo.GetDamagedInfo;
            ITransition getDamagedAnimationClip = null;
            if (getDamagedInfo.ignoreDirection)
            {
                getDamagedAnimationClip = getDamagedInfo.getDamagedAnimationClipOnIgnoreDirection;
            }
            else if (getDamagedInfo.getDamagedAnimationClip != null)
            {
                _agent.IsKnockDown = getDamagedInfo.isKnockDown;
                Vector3 hitPoint = _agent.HealthCompo.GetDamagedInfo.attacker.transform.position;
                Vector3 agentPosition = _agent.transform.position;
                agentPosition.y = hitPoint.y;

                Vector3 hitDirection = (hitPoint - agentPosition).normalized;

                Vector3 forward = _agent.ModelTrm.forward;

                float angleHitFrom = Vector3.SignedAngle(forward, hitDirection, Vector3.up);

                if (angleHitFrom > -45 && angleHitFrom <= 45)
                    getDamagedAnimationClip = getDamagedInfo.getDamagedAnimationClip[Define.EDirection.Front];
                else if (angleHitFrom > 45 && angleHitFrom <= 135)
                    getDamagedAnimationClip = getDamagedInfo.getDamagedAnimationClip[Define.EDirection.Right];
                else if (angleHitFrom > -135 && angleHitFrom <= -45)
                    getDamagedAnimationClip = getDamagedInfo.getDamagedAnimationClip[Define.EDirection.Left];
                else
                    getDamagedAnimationClip = getDamagedInfo.getDamagedAnimationClip[Define.EDirection.Back];
            }

            if (getDamagedAnimationClip != null)
            {
                PlayAnimationClip(getDamagedAnimationClip,
                    async () =>
                    {
                        if (getDamagedInfo.isKnockDown)
                        {
                            try
                            {
                                OnKnockDown?.Invoke();
                                if (_knockDownToken != null && !_knockDownToken.IsCancellationRequested) return;
                                _knockDownToken = new CancellationTokenSource();
                                await UniTask.WaitForSeconds(getDamagedInfo.knockDownTime,
                                    cancellationToken: _knockDownToken.Token);
                                _agent.IsKnockDown = false;
                                if (_agent.TryGetCompo(out AgentFullMountable fullMountable))
                                {
                                    if (fullMountable.IsFullMounted)
                                        return;
                                }

                                PlayGetUpAnimation();
                            }
                            catch (Exception e)
                            {
                            }
                        }
                        else
                        {
                            OnEndHitAnimation?.Invoke();
                        }
                    }, !getDamagedInfo.isKnockDown);
            }
        }

        protected virtual void PlayGetUpAnimation()
        {
            GetDamagedInfo getDamagedInfo = _agent.HealthCompo.GetDamagedInfo;

            PlayAnimationClip(getDamagedInfo.getUpAnimationClip, () =>
            {
                if (_agent.HealthCompo.IsDead) return;
                AgentAnimationTrigger agentAnimationTriggerCompo = _agent.GetCompo<AgentAnimationTrigger>(true);
                agentAnimationTriggerCompo.OnGetUp?.Invoke();
            });
        }

        public void PlayAnimationClip(ITransition clip, Action EndCallBack = null,
            bool playControllerOnEnd = true)
        {
            if (clip == null) return;
            AnimancerState state = _hybridAnimancer.Play(clip, clip.FadeDuration, mode: FadeMode.FromStart);
            state.Speed *= _effectiveAnimationSpeed;
            state.Events(this).OnEnd ??= () =>
            {
                if (state.FadeGroup != null || _hybridAnimancer.Controller.State.FadeGroup != null) return;
                EndCallBack?.Invoke();
                if (playControllerOnEnd)
                    _hybridAnimancer.PlayController();
            };
        }
    }
}