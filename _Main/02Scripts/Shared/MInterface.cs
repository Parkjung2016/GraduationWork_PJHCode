using System;
using UnityEngine;

namespace Main.Shared
{
    public interface IAgent
    {
        public GameObject GameObject { get; }
        public Transform ModelTrm { get; }
        public Transform HeadTrm { get; }
        public Action<HitInfo> OnHitTarget { get; set; }
    }

    public interface IInteractable
    {
        public Transform UIDisplayTrm { get; }
        public Vector3 AdditionalUIDisplayPos { get; }
        public bool CanInteract { get; }
        public string Description { get; }
        public void Interact(Transform Interactor);
    }

    public interface IPlayer
    {
        public GameObject GameObject { get; }
        public Transform ModelTrm { get; }
        public bool CanApplyPassive { get; set; }
        public event Action<bool> OnChangedCanApplyPassive;
    }

    public interface ICamera
    {
    }

    public interface ILockOnAble
    {
        public GameObject GameObject { get; }
        public Transform LockOnUIDisplayTargetTrm { get; }
    }

    public interface IBattleZoneController
    {
        public IBattleZone CurrentBattleZone { get; }
        public event Action<int> OnChangedRemainingEnemy;
        public int RemainingEnemy { get; }
    }

    public interface IBattleZone
    {
    }

    public interface IScene
    {
        public void SettingForTimeline();
        public void SettingForEndTimeline();
    }

    public interface IBattleScene
    {
        public bool IsInBattle { get; }
        public delegate void ChangedBattleZoneControllerEvent(IBattleZoneController prevBattleZoneController,
            IBattleZoneController currentBattleZoneController);

        public event ChangedBattleZoneControllerEvent OnChangedBattleZoneController;
        public IBattleZoneController CurrentBattleZoneController { get; set; }
    }
}