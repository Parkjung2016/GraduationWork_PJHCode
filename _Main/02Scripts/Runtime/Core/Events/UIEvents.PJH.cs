using Main.Shared;
using PJH.Shared;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Video;

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
        public static readonly ShowPassiveInfoUI ShowPassiveInfoUI = new ShowPassiveInfoUI();
        public static readonly ShowTextDialogueUI ShowTextDialogueUI = new ShowTextDialogueUI();

        public static readonly ShowEvasionWhileHittingInfUI ShowEvasionWhileHittingInfUI =
            new ShowEvasionWhileHittingInfUI();

        public static readonly ShowNoticeText ShowNoticeText = new ShowNoticeText();
        public static readonly ShowQuestPreviewUI ShowQuestPreviewUI = new ShowQuestPreviewUI();
        public static readonly InteractDropItem InteractDropItem = new InteractDropItem();
    }

    public enum PassiveInfoType
    {
        None,
        Buff
    }
    public class InteractDropItem : GameEvent
    {
    }
    public class ShowQuestPreviewUI : GameEvent
    {
        public bool show;
        public VideoClip previewVideo;
    }

    public class ShowNoticeText : GameEvent
    {
        public string notice;
    }

    public class ShowEvasionWhileHittingInfUI : GameEvent
    {
        public IPlayer player;
    }

    public class ShowPassiveInfoUI : GameEvent
    {
        public PlayerPieceSO passive;
        public PassiveInfoType passiveInfoType;
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

    public class ShowTextDialogueUI : GameEvent
    {
        public string dialogueText;
    }

    public class ShowFinisherTargetUI : GameEvent
    {
        public bool isShowUI;
        public Transform finisherTargetTrm;
    }
}