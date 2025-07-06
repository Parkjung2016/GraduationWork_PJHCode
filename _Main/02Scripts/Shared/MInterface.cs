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
        public string Description { get; set; }
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
        public Vector3 AdditionalUIDisplayPos { get; }
    }

    public interface IScene
    {
    }
}