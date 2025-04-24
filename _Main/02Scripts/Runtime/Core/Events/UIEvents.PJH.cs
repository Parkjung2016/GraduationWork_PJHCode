using System;
using DG.Tweening;
using Main.Shared;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Main.Runtime.Core.Events
{
    public static partial class UIEvents
    {
        public static readonly ShowInteractUI ShowInteractUIEventChannel = new ShowInteractUI();
        public static readonly ShowFinisherTargetUI ShowFinisherTargetUI = new ShowFinisherTargetUI();
        public static readonly ShowActUI ShowActUI = new ShowActUI();
        public static readonly ShowDeathUI ShowDeathUI = new ShowDeathUI();
        public static readonly ShowLockOnUI ShowLockOnUI = new ShowLockOnUI();
        public static readonly ShowWarpStrikeTargetUI ShowWarpStrikeTargetUI = new ShowWarpStrikeTargetUI();
    }

    public class ShowInteractUI : GameEvent
    {
        public bool isShowUI;
        public IInteractable interactableTarget;
        public string interactDescription;
    }

    public class ShowWarpStrikeTargetUI : GameEvent
    {
        public bool isShowUI;
        public IAgent target;
    }

    public class ShowLockOnUI : GameEvent
    {
        public bool isShowUI;
        public ILockOnAble lockOnTarget;
    }

    public class ShowActUI : GameEvent
    {
        public bool isShowUI;
        public InputActionReference actInputActionReference;
        public Transform displayTargetTrm;
    }

    public class ShowDeathUI : GameEvent
    {
        public bool isShowUI;
    }

    public class ShowFinisherTargetUI : GameEvent
    {
        public bool isShowUI;
        public Transform finisherTargetTrm;
    }
}