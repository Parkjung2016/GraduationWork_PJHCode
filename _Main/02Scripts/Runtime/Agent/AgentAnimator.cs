using System;
using System.Threading;
using Animancer;
using Cysharp.Threading.Tasks;
using Main.Runtime.Animators;
using Main.Runtime.Combat.Core;
using Main.Runtime.Core;
using UnityEngine;

namespace Main.Runtime.Agents
{
    public class AgentAnimator : AnimatorRenderer, IAgentComponent, IAfterInitable
    {
        public event Action OnEndHitAnimation;
        public event Action OnKnockDown;
        [SerializeField] private ClipTransition _deathAnimationClip;
        protected Agent _agent;
        CancellationTokenSource _knockDownToken;

        public virtual void Initialize(Agent agent)
        {
            _agent = agent;
        }

        public virtual void AfterInitialize()
        {
            _agent.HealthCompo.OnApplyDamaged += HandleApplyDamaged;
            _agent.HealthCompo.OnDeath += HandleDeath;
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
            if (_agent.TryGetCompo<AgentFullMountable>(out var fullMountableCompo))
            {
                fullMountableCompo.OnFullMounted -= HandleFullMounted;
            }

            AgentAnimationTrigger agentAnimationTriggerCompo = _agent.GetCompo<AgentAnimationTrigger>(true);
            agentAnimationTriggerCompo.OnTriggerRagdoll -= HandleTriggerRagdoll;
        }

        private void HandleTriggerRagdoll()
        {
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

        protected virtual async void HandleDeath()
        {
            if (_agent.TryGetCompo(out AgentFullMountable fullMountable))
            {
                if (fullMountable.IsFullMounted)
                {
                    AgentAnimationTrigger agentAnimationTriggerCompo = _agent.GetCompo<AgentAnimationTrigger>(true);
                    agentAnimationTriggerCompo.OnTriggerRagdoll?.Invoke();
                    return;
                }
            }

            _hybridAnimancer.Play(_deathAnimationClip);
        }

        protected virtual void HandleApplyDamaged(float damage)
        {
            if (_knockDownToken != null && !_knockDownToken.IsCancellationRequested)
            {
                _knockDownToken.Cancel();
                _knockDownToken.Dispose();
            }

            GetDamagedInfo getDamagedInfo = _agent.HealthCompo.GetDamagedInfo;
            _agent.IsKnockDown = getDamagedInfo.isKnockDown;
            PlayAnimationClip(getDamagedInfo.getDamagedAnimationClip,
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