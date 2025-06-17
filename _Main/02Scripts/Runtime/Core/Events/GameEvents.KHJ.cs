using Main.Runtime.Core.StatSystem;
using System.Collections.Generic;
using UnityEngine;

namespace Main.Runtime.Core.Events
{
    public static partial class GameEvents
    {
        public static readonly ModifyPlayerStat ModifyPlayerStat = new ModifyPlayerStat();
        public static readonly CurrentBlockInfo CurrentBlockInfo = new CurrentBlockInfo();
        public static readonly InventoryInfo InventoryInfo = new InventoryInfo();
        public static readonly TutorialInfo TutorialInfo = new TutorialInfo();
        public static readonly ShakeInven ShakeInven = new ShakeInven();
        public static readonly ClearTutorial ClearTutorial = new ClearTutorial();
        public static readonly StartTutorial StartTutorial = new StartTutorial();
    }

    public class ModifyPlayerStat : GameEvent
    {
        public StatSO modifyPlayerStat;
        public object modifyKey;
        public float modifyPlayerStatValue;
        public bool isIncreaseStat;
    }

    public class CurrentBlockInfo : GameEvent
    {
        public int grade;
        public StatSO stat;
        public float statValue;
    }

    public class InventoryInfo : GameEvent
    {
        public List<RectTransform> childTrmList = new();
    }

    public class TutorialInfo : GameEvent
    {
        public bool IsEndQuest;
        public bool IsStartQuest = false;
        public string QuestName;
        public string Description;
        public int CurrentAmount;
        public int RequiredAmount;
    }

    public class ShakeInven : GameEvent { }

    public class ClearTutorial : GameEvent { }
    public class StartTutorial : GameEvent { }
}