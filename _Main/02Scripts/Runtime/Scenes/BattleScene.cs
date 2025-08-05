using Cysharp.Threading.Tasks;
using DG.Tweening;
using LJS.Map;
using LJS.Utils;
using Main.Core;
using Main.Runtime.Agents;
using Main.Runtime.Core;
using Main.Runtime.Core.Events;
using Main.Runtime.Manager;
using Main.Runtime.Manager.VolumeTypes;
using PJH.Runtime.Players;
using PJH.Runtime.UI;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using YTH.Boss;
using YTH.Enemies;
using Managers = Main.Runtime.Manager.Managers;

namespace Main.Scenes
{
    public class BattleScene : BaseScene
    {
        [SerializeField] private GameObject _gameSceneUI;
        [SerializeField] private PlayableAsset _appearBossSequence, _bossDeadSequence;
        [SerializeField] private CinemachineCamera _bossDeathTimelineBossCamera;
        private GameEventChannelSO _uiEventChannel;

        protected GameEventChannelSO _gameEventChannel;

        private PlayableDirector _playableDirector;

        protected override void Awake()
        {
            base.Awake();
            _playableDirector = FindAnyObjectByType<PlayableDirector>();
            Application.targetFrameRate = 60;
            _gameEventChannel = AddressableManager.Load<GameEventChannelSO>("GameEventChannel");
            _uiEventChannel = AddressableManager.Load<GameEventChannelSO>("UIEventChannelSO");
            AddressableManager.Instantiate<TextDialogueCanvas>("TextDialogueCanvas");
        }

        protected override void Start()
        {
            base.Start();
            _gameEventChannel.AddListener<PlayerDeath>(HandlePlayerDeath);
            _gameEventChannel.AddListener<TimeSlowByPlayer>(HandlePlayerAvoidingAttack);
            _gameEventChannel.AddListener<PlayerStunned>(HandlePlayerStunned);
            _gameEventChannel.AddListener<FinishAllWave>(HandleEndBattle);
            _gameEventChannel.AddListener<StartWave>(HandleStartWave);
            _gameEventChannel.AddListener<BossDead>(HandleBossDead);
            _gameEventChannel.AddListener<EnterBossRoom>(HandleEnterBossRoom);

            RoomManager.Instance.CurrentRoomChangeActon += HandleRoomChange;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _gameEventChannel.RemoveListener<PlayerDeath>(HandlePlayerDeath);
            _gameEventChannel.RemoveListener<TimeSlowByPlayer>(HandlePlayerAvoidingAttack);
            _gameEventChannel.RemoveListener<PlayerStunned>(HandlePlayerStunned);
            _gameEventChannel.RemoveListener<FinishAllWave>(HandleEndBattle);
            _gameEventChannel.RemoveListener<StartWave>(HandleStartWave);
            _gameEventChannel.RemoveListener<BossDead>(HandleBossDead);
            _gameEventChannel.RemoveListener<EnterBossRoom>(HandleEnterBossRoom);

            RoomManager.Instance.CurrentRoomChangeActon -= HandleRoomChange;
        }

        public void StartBossBattle()
        {
            Sequence seq = DOTween.Sequence();
            seq.Append(Managers.VolumeManager.GetVolumeType<BrightnessVolumeType>().SetValue(0, .5f));
            seq.AppendInterval(.3f);
            seq.AppendCallback(() =>
            {
                var evt = GameEvents.StartBossBattle;
                _gameEventChannel.RaiseEvent(evt);
                _playableDirector.Stop();
                _playerInput.EnablePlayerInput(true);
                _gameSceneUI.SetActive(true);
                Managers.FMODManager.ResumeMainSound();
                Managers.VolumeManager.GetVolumeType<BrightnessVolumeType>().SetValue(1, .5f);
            });
        }

        private void HandleEnterBossRoom(EnterBossRoom evt)
        {
            Managers.FMODManager.PauseMainSound();
            _gameSceneUI.SetActive(false);
            _playerInput.EnablePlayerInput(false);
            Sequence seq = DOTween.Sequence();
            seq.Append(Managers.VolumeManager.GetVolumeType<BrightnessVolumeType>().SetValue(0, .5f));
            seq.AppendInterval(1f);
            seq.AppendCallback(() =>
            {
                Managers.VolumeManager.GetVolumeType<BrightnessVolumeType>().SetValue(1, .5f);
                _playableDirector.Play(_appearBossSequence);
            });
        }


        private void HandleRoomChange(RoomComponent room)
        {
            if (room is SpecialRoomComponent)
            {
                switch (room.SpecialRoomType)
                {
                    case SpecialRoomType.Choice:
                        Managers.FMODManager.MusicEventInstance.setParameterByNameWithLabel("MusicType", "InChoice");
                        break;
                    case SpecialRoomType.Shop:
                        Managers.FMODManager.MusicEventInstance.setParameterByNameWithLabel("MusicType", "InShop");
                        break;
                }
            }
        }

        private void HandleBossDead(BossDead obj)
        {
            _playerInput.EnablePlayerInput(false);
            _playerInput.EnableUIInput(false);

            Managers.FMODManager.StopMusicSound();
            Sequence seq = DOTween.Sequence();
            seq.Append(Managers.VolumeManager.GetVolumeType<BrightnessVolumeType>().SetValue(0, 1f));
            seq.AppendInterval(1f);
            seq.AppendCallback(async () =>
            {
                var destroyDeadEnemyEvt = GameEvents.DestroyDeadEnemy;
                destroyDeadEnemyEvt.isPlayingBossDeathTimeline = true;
                _gameEventChannel.RaiseEvent(destroyDeadEnemyEvt);
                BossRoomComponent bossRoomComponent = (RoomManager.Instance.CurrentRoom as BossRoomComponent);
                Player player = PlayerManager.Instance.Player as Player;
                player.GetComponentInChildren<DistanceFade>().Locked = true;
                player.WarpingComponent.enabled = false;
                PlayerAnimator playerAnimatorCompo = player.GetCompo<PlayerAnimator>();
                playerAnimatorCompo.Animancer.enabled = false;
                PlayerIK playerIKCompo = player.GetCompo<PlayerIK>();
                playerIKCompo.LegsAnimator.enabled = false;
                playerIKCompo.LeaningAnimator.enabled = false;
                playerIKCompo.LookAnimator.enabled = false;
                player.ComponentManager.EnableComponents(false);
                Boss boss = bossRoomComponent.BossObject;
                _bossDeathTimelineBossCamera.Target.TrackingTarget = boss.transform.Find("Visual");
                boss.GetComponentInChildren<DistanceFade>().Locked = true;

                boss.WarpingComponent.enabled = false;
                AgentAnimator bossAnimatorCompo = boss.GetCompo<AgentAnimator>(true);
                bossAnimatorCompo.Animancer.enabled = false;
                Animator bossAnimator = bossAnimatorCompo.Animator;
                bossAnimator.enabled = true;
                boss.GetCompo<EnemyMovement>().AIPath.enabled = false;
                boss.BehaviorTreeCompo.enabled = false;
                AgentIK bossIK = boss.GetCompo<AgentIK>();
                bossIK.LegsAnimator.enabled = false;
                bossIK.RagdollAnimator.enabled = false;
                _playableDirector.playableAsset = _bossDeadSequence;
                var timeline = _playableDirector.playableAsset as TimelineAsset;
                foreach (var track in timeline.GetOutputTracks())
                {
                    if (track.name == "Boss Animation Track")
                    {
                        _playableDirector.SetGenericBinding(track, bossAnimator);
                    }
                }

                boss.ComponentManager.EnableComponents(false);

                Transform centerPoint = bossRoomComponent.BossDeathTimelinePoint;
                player.transform.position = centerPoint.position + Vector3.forward * 0.55f;
                boss.transform.position = centerPoint.position - Vector3.forward * 0.55f;
                await UniTask.Yield();
                player.transform.DOLookAt(boss.transform.position, 0f, AxisConstraint.Y);
                boss.transform.DOLookAt(centerPoint.position, 0f, AxisConstraint.Y);
                _playableDirector.Play();
            });
            Canvas[] canvases = FindObjectsByType<Canvas>(FindObjectsSortMode.None);
            for (int i = 0; i < canvases.Length; i++)
            {
                if (canvases[i].gameObject.name == "FadeInOutCanvas") continue;
                canvases[i].gameObject.SetActive(false);
            }

            seq.Append(Managers.VolumeManager.GetVolumeType<BrightnessVolumeType>().SetValue(1, 1f));
        }

        private void HandleStartWave(StartWave obj)
        {
            if (RoomManager.Instance.CurrentRoom is BossRoomComponent)
            {
                Managers.FMODManager.MusicEventInstance.setParameterByNameWithLabel("MusicType", "InBoss");
                AddressableManager.Instantiate<BossUI>("BossUI");
            }
            else
            {
                Managers.FMODManager.MusicEventInstance.setParameterByName("InBattleMusicNumber", Random.Range(0, 2));
                Managers.FMODManager.MusicEventInstance.setParameterByNameWithLabel("MusicType", "InBattle");
            }
        }

        private void HandleEndBattle(FinishAllWave evt)
        {
            Managers.FMODManager.MusicEventInstance.setParameterByNameWithLabel("MusicType", "Default");
        }

        private void HandlePlayerStunned(PlayerStunned evt)
        {
            if (evt.isStunned)
            {
                float duration = .4f;
                Managers.VolumeManager.GetVolumeType<SaturateVolumeType>().SetValue(-2, duration);
                Managers.VolumeManager.GetVolumeType<BrightnessVolumeType>().SetValue(.3f, duration);
                Managers.VolumeManager.GetVolumeType<VignettingFadeVolumeType>().SetValue(.4f, duration);
            }
            else
            {
                float duration = .2f;
                Managers.VolumeManager.GetVolumeType<SaturateVolumeType>().ResetValue(duration);
                Managers.VolumeManager.GetVolumeType<BrightnessVolumeType>().ResetValue(duration);
                Managers.VolumeManager.GetVolumeType<VignettingFadeVolumeType>().ResetValue(duration);
            }
        }

        private void HandlePlayerAvoidingAttack(TimeSlowByPlayer evt)
        {
            if (evt.isEnabledEffect)
            {
                float duration = .2f;
                Managers.VolumeManager.GetVolumeType<SaturateVolumeType>().SetValue(-2, 2f);
                Managers.VolumeManager.GetVolumeType<SepiaVolumeType>().SetValue(-2, 2f);
            }
            else
            {
                float duration = .2f;
                Managers.VolumeManager.GetVolumeType<SaturateVolumeType>().ResetValue(duration);
                Managers.VolumeManager.GetVolumeType<SepiaVolumeType>().ResetValue(duration);
            }
        }

        private void HandlePlayerDeath(PlayerDeath evt)
        {
            _playerInput.EnablePlayerInput(false);
            _playerInput.EnableUIInput(false);
            Sequence seq = DOTween.Sequence();
            seq.AppendCallback(() =>
            {
                var destroyDeadEnemyEvt = GameEvents.DestroyDeadEnemy;
                _gameEventChannel.RaiseEvent(destroyDeadEnemyEvt);
                var showDeathUIEvt = UIEvents.ShowDeathUI;
                showDeathUIEvt.isShowUI = true;
                _uiEventChannel.RaiseEvent(showDeathUIEvt);
            });
            seq.AppendInterval(5);
            seq.AppendCallback(() => { SceneManagerEx.LoadScene("Lobby", true); });
        }

        public void GoToLobby()
        {
            Sequence seq = DOTween.Sequence();
            seq.Append(Managers.VolumeManager.GetVolumeType<BrightnessVolumeType>().SetValue(0, 2f));
            seq.AppendInterval(1f);
            seq.AppendCallback(async () => { SceneManagerEx.LoadScene("Lobby", true); });
        }
    }
}